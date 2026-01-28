import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';

export type Theme = 'light' | 'dark';

const THEME_KEY = 'tems-theme-preference';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly isBrowser: boolean;
  private themeSubject: BehaviorSubject<Theme>;
  
  readonly theme$: Observable<Theme>;

  constructor(@Inject(PLATFORM_ID) platformId: object) {
    this.isBrowser = isPlatformBrowser(platformId);
    const initialTheme = this.getStoredTheme();
    this.themeSubject = new BehaviorSubject<Theme>(initialTheme);
    this.theme$ = this.themeSubject.asObservable();
    this.applyTheme(initialTheme);
  }

  get currentTheme(): Theme {
    return this.themeSubject.value;
  }

  get isDarkMode(): boolean {
    return this.currentTheme === 'dark';
  }

  toggleTheme(): void {
    const newTheme: Theme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.setTheme(newTheme);
  }

  setTheme(theme: Theme): void {
    this.themeSubject.next(theme);
    this.applyTheme(theme);
    this.storeTheme(theme);
  }

  private getStoredTheme(): Theme {
    if (!this.isBrowser) {
      return 'light';
    }
    
    const stored = localStorage.getItem(THEME_KEY);
    if (stored === 'dark' || stored === 'light') {
      return stored;
    }
    
    return 'light';
  }

  private storeTheme(theme: Theme): void {
    if (this.isBrowser) {
      localStorage.setItem(THEME_KEY, theme);
    }
  }

  private applyTheme(theme: Theme): void {
    if (!this.isBrowser) {
      return;
    }

    const html = document.documentElement;
    
    if (theme === 'dark') {
      html.classList.add('dark');
    } else {
      html.classList.remove('dark');
    }
  }
}
