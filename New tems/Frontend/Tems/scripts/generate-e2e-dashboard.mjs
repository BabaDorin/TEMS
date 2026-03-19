import fs from 'node:fs';
import path from 'node:path';

const resultsPath = path.resolve('test-results/results.json');
const outputPath = path.resolve('test-results/dashboard.md');

function safeNumber(value) {
  return Number.isFinite(value) ? value : 0;
}

function collectTests(suite, inheritedPath = [], all = []) {
  const suiteTitle = suite.title ? [...inheritedPath, suite.title] : inheritedPath;

  for (const spec of suite.specs ?? []) {
    for (const test of spec.tests ?? []) {
      all.push({
        title: spec.title,
        titlePath: [...suiteTitle, spec.title].filter(Boolean),
        status: test.status ?? 'unknown',
        duration: safeNumber(test.results?.reduce((sum, r) => sum + safeNumber(r.duration), 0)),
        file: spec.file ?? 'unknown',
        project: test.projectName ?? 'unknown',
      });
    }
  }

  for (const child of suite.suites ?? []) {
    collectTests(child, suiteTitle, all);
  }

  return all;
}

if (!fs.existsSync(resultsPath)) {
  console.error(`Missing Playwright JSON report at ${resultsPath}`);
  process.exit(1);
}

const raw = JSON.parse(fs.readFileSync(resultsPath, 'utf8'));
const suites = raw.suites ?? [];
const tests = suites.flatMap((suite) => collectTests(suite));

const totals = {
  total: tests.length,
  passed: tests.filter((t) => t.status === 'expected' || t.status === 'passed').length,
  failed: tests.filter((t) => t.status === 'unexpected' || t.status === 'failed').length,
  skipped: tests.filter((t) => t.status === 'skipped').length,
  flaky: tests.filter((t) => t.status === 'flaky').length,
};

const totalDurationMs = tests.reduce((sum, t) => sum + t.duration, 0);
const passRate = totals.total === 0 ? 0 : (totals.passed / totals.total) * 100;

const byFile = new Map();
for (const t of tests) {
  const key = t.file;
  if (!byFile.has(key)) {
    byFile.set(key, { total: 0, passed: 0, failed: 0, skipped: 0 });
  }
  const item = byFile.get(key);
  item.total += 1;
  if (t.status === 'expected' || t.status === 'passed') item.passed += 1;
  if (t.status === 'unexpected' || t.status === 'failed') item.failed += 1;
  if (t.status === 'skipped') item.skipped += 1;
}

const failedTests = tests.filter((t) => t.status === 'unexpected' || t.status === 'failed');
const now = new Date().toISOString();

const lines = [];
lines.push('# E2E Testing Dashboard');
lines.push('');
lines.push(`Generated at: ${now}`);
lines.push('');
lines.push('## Summary');
lines.push('');
lines.push(`- Total tests: ${totals.total}`);
lines.push(`- Passed: ${totals.passed}`);
lines.push(`- Failed: ${totals.failed}`);
lines.push(`- Skipped: ${totals.skipped}`);
lines.push(`- Flaky: ${totals.flaky}`);
lines.push(`- Pass rate: ${passRate.toFixed(1)}%`);
lines.push(`- Cumulative duration: ${(totalDurationMs / 1000).toFixed(1)}s`);
lines.push('');
lines.push('## Coverage By Spec File');
lines.push('');
lines.push('| Spec file | Total | Passed | Failed | Skipped |');
lines.push('| --- | ---: | ---: | ---: | ---: |');
for (const [file, stats] of [...byFile.entries()].sort((a, b) => a[0].localeCompare(b[0]))) {
  lines.push(`| ${file} | ${stats.total} | ${stats.passed} | ${stats.failed} | ${stats.skipped} |`);
}
lines.push('');
lines.push('## Failing Tests');
lines.push('');
if (failedTests.length === 0) {
  lines.push('- No failing tests in this run.');
} else {
  for (const failed of failedTests) {
    lines.push(`- [${failed.project}] ${failed.titlePath.join(' > ')}`);
  }
}
lines.push('');

fs.mkdirSync(path.dirname(outputPath), { recursive: true });
fs.writeFileSync(outputPath, lines.join('\n'));
console.log(`Dashboard generated: ${outputPath}`);
