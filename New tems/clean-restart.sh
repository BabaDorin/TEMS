#!/usr/bin/env bash
# TEMS - First run bootstrap and clean restart script
#
# Default behavior (no flags): starts full stack in Docker (infra + backend + frontend),
# provisions Keycloak realm/client/roles, and verifies endpoints.
#
# Usage:
#   ./clean-restart.sh                # Full bootstrap in Docker (recommended first run)
#   ./clean-restart.sh --all-docker   # Same as default
#   ./clean-restart.sh --all          # Start infra in Docker + backend/frontend locally
#   ./clean-restart.sh --infra-only   # Start only infrastructure (MongoDB, Keycloak, Identity Server)
#   ./clean-restart.sh --help

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
BACKEND_DIR="${ROOT_DIR}/Backend/Tems"
FRONTEND_DIR="${ROOT_DIR}/Frontend/Tems"
KEYCLOAK_SETUP_SCRIPT="${ROOT_DIR}/Infrastructure/Keycloak/configure-keycloak.sh"

MODE="all-docker"
ALL_DOCKER_COMPOSE_FILE="compose.dev-all-docker.generated.yaml"

print_help() {
  cat <<'USAGE'
Usage:
  ./clean-restart.sh                Full bootstrap in Docker (default)
  ./clean-restart.sh --all-docker   Full bootstrap in Docker
  ./clean-restart.sh --all          Infra in Docker + backend/frontend locally
  ./clean-restart.sh --infra-only   Infrastructure only
  ./clean-restart.sh --help
USAGE
}

for arg in "$@"; do
  case "$arg" in
    --all)
      MODE="all-local"
      ;;
    --all-docker)
      MODE="all-docker"
      ;;
    --infra-only)
      MODE="infra-only"
      ;;
    --help|-h)
      print_help
      exit 0
      ;;
    *)
      echo "Unknown argument: $arg"
      print_help
      exit 1
      ;;
  esac
done

command_exists() {
  command -v "$1" >/dev/null 2>&1
}

require_cmd() {
  if ! command_exists "$1"; then
    echo "❌ Missing required command: $1"
    exit 1
  fi
}

wait_for_http() {
  local name="$1"
  local url="$2"
  local timeout="$3"

  local elapsed=0
  local step=5
  while true; do
    if curl -fsS "$url" >/dev/null 2>&1; then
      echo "✅ ${name}"
      return 0
    fi

    if [ "$elapsed" -ge "$timeout" ]; then
      echo "❌ Timeout waiting for ${name} (${url})"
      return 1
    fi

    sleep "$step"
    elapsed=$((elapsed + step))
    echo "   ⏳ Waiting for ${name}... (${elapsed}s/${timeout}s)"
  done
}

wait_for_docker() {
  local timeout=180
  local elapsed=0

  while ! docker info >/dev/null 2>&1; do
    if [ "$elapsed" -ge "$timeout" ]; then
      echo "❌ Docker is not ready after ${timeout}s. Please start Docker and retry."
      exit 1
    fi
    sleep 5
    elapsed=$((elapsed + 5))
    echo "   ⏳ Waiting for Docker... (${elapsed}s/${timeout}s)"
  done
}

start_docker_if_needed() {
  if docker info >/dev/null 2>&1; then
    return
  fi

  echo "⚠️  Docker is not running. Attempting to start Docker Desktop..."

  if [ "$(uname -s)" = "Darwin" ] && command_exists open; then
    open -a Docker || true
  fi

  wait_for_docker
}

start_local_backend() {
  local logs_dir="${ROOT_DIR}/.runlogs"
  mkdir -p "$logs_dir"

  if lsof -ti:5158 >/dev/null 2>&1; then
    echo "ℹ️  Backend already running on port 5158"
    return
  fi

  echo "🚀 Starting backend locally..."
  (
    cd "${BACKEND_DIR}/Tems.Host"
    nohup bash -lc "dotnet restore ../Tems.sln && dotnet run --urls http://0.0.0.0:5158" \
      >"${logs_dir}/backend.log" 2>&1 &
    echo $! >"${logs_dir}/backend.pid"
  )
}

start_local_frontend() {
  local logs_dir="${ROOT_DIR}/.runlogs"
  mkdir -p "$logs_dir"

  if lsof -ti:4200 >/dev/null 2>&1; then
    echo "ℹ️  Frontend already running on port 4200"
    return
  fi

  echo "🚀 Starting frontend locally..."
  (
    cd "${FRONTEND_DIR}"
    nohup bash -lc "npm install && npx ng serve --host 0.0.0.0 --port 4200" \
      >"${logs_dir}/frontend.log" 2>&1 &
    echo $! >"${logs_dir}/frontend.pid"
  )
}

# Core prerequisites
require_cmd docker
require_cmd curl
require_cmd jq

if [ "$MODE" = "all-local" ]; then
  require_cmd dotnet
  require_cmd npm
  require_cmd lsof
fi

echo "🧹 TEMS clean restart (${MODE})"
echo "================================"

echo "🔧 Checking Docker..."
start_docker_if_needed

cd "$BACKEND_DIR"

COMPOSE_ARGS=(-f compose.yaml)
if [ "$MODE" = "all-docker" ]; then
  if [ ! -f "${BACKEND_DIR}/${ALL_DOCKER_COMPOSE_FILE}" ]; then
    echo "❌ Missing ${ALL_DOCKER_COMPOSE_FILE} in ${BACKEND_DIR}"
    exit 1
  fi
  COMPOSE_ARGS+=(-f "${ALL_DOCKER_COMPOSE_FILE}")
fi

echo "🛑 Step 1: Stopping running containers..."
docker compose "${COMPOSE_ARGS[@]}" down --remove-orphans 2>/dev/null || true

echo "🗑️  Step 2: Removing TEMS containers..."
docker rm -f tems-keycloak tems-mongodb tems-identity-server tems-sqlserver tems-app tems-backend-api tems-frontend 2>/dev/null || true

echo "🧽 Step 3: Removing TEMS images..."
TEMS_IMAGE_IDS="$(docker images | awk '/tems/ {print $3}')"
if [ -n "$TEMS_IMAGE_IDS" ]; then
  echo "$TEMS_IMAGE_IDS" | xargs docker rmi -f 2>/dev/null || true
else
  echo "ℹ️  No TEMS images to remove"
fi

echo "💨 Step 4: Pruning Docker build cache and dangling resources..."
docker system prune -af

echo "🏗️  Step 5: Building images without cache..."
docker compose "${COMPOSE_ARGS[@]}" build --no-cache

echo "🚀 Step 6: Starting containers..."
docker compose "${COMPOSE_ARGS[@]}" up -d

printf "\n📊 Container status\n"
docker compose "${COMPOSE_ARGS[@]}" ps

printf "\n🔍 Step 7: Waiting for infrastructure readiness...\n"
wait_for_http "Keycloak ready (8080)" "http://localhost:8080/health/ready" 300
wait_for_http "Identity Server discovery (5001)" "http://localhost:5001/.well-known/openid-configuration" 240

if docker exec tems-mongodb mongosh --eval "db.adminCommand('ping')" >/dev/null 2>&1; then
  echo "✅ MongoDB healthy (27017)"
else
  echo "⚠️  MongoDB health probe failed once, continuing. Check with: docker logs tems-mongodb"
fi

printf "\n⚙️  Step 8: Running Keycloak provisioning...\n"
if [ ! -f "$KEYCLOAK_SETUP_SCRIPT" ]; then
  echo "❌ Missing Keycloak setup script: $KEYCLOAK_SETUP_SCRIPT"
  exit 1
fi

bash "$KEYCLOAK_SETUP_SCRIPT"

printf "\n🔍 Step 9: Verifying Keycloak TEMS realm discovery...\n"
wait_for_http "Keycloak TEMS realm discovery" "http://localhost:8080/realms/tems/.well-known/openid-configuration" 120

if [ "$MODE" = "all-local" ]; then
  printf "\n🚀 Step 10: Starting backend/frontend locally...\n"
  start_local_backend
  start_local_frontend

  printf "\n🔍 Waiting for local app endpoints...\n"
  wait_for_http "Backend API (5158)" "http://localhost:5158/swagger/index.html" 240
  wait_for_http "Frontend (4200)" "http://localhost:4200" 300
fi

if [ "$MODE" = "all-docker" ]; then
  printf "\n🔍 Step 10: Waiting for app endpoints (Docker mode)...\n"
  wait_for_http "Backend API (5158)" "http://localhost:5158/swagger/index.html" 300
  wait_for_http "Frontend (4200)" "http://localhost:4200" 360
fi

printf "\n✨ TEMS is ready\n"
printf "\nURLs:\n"
echo "  Frontend:        http://localhost:4200"
echo "  Backend API:     http://localhost:5158/swagger"
echo "  Keycloak Admin:  http://localhost:8080 (admin/admin)"
echo "  Identity Server: http://localhost:5001"

printf "\nUseful commands:\n"
echo "  Check status:    ./check-services.sh"
echo "  Infra logs:      cd Backend/Tems && docker compose ${COMPOSE_ARGS[*]} logs -f"
echo "  Stop containers: cd Backend/Tems && docker compose ${COMPOSE_ARGS[*]} down"
