import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';

export type Theme = 'light' | 'dark';

const LEGACY_THEME_KEY = 'tems-theme-preference';
const GUEST_THEME_KEY = 'tems-theme-preference:guest';
const THEME_KEY_PREFIX = 'tems-theme-preference:';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly isBrowser: boolean;
  private themeSubject: BehaviorSubject<Theme>;
  
  readonly theme$: Observable<Theme>;

  constructor(
    @Inject(PLATFORM_ID) platformId: object,
    private authService: AuthService,
    private tokenService: TokenService
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
    const initialTheme = this.getStoredTheme();
    this.themeSubject = new BehaviorSubject<Theme>(initialTheme);
    this.theme$ = this.themeSubject.asObservable();
    this.applyTheme(initialTheme);

    if (this.isBrowser) {
      this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
        this.syncThemeForCurrentUser(isAuthenticated);
      });
    }
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

    const currentKey = this.getStorageKey();
    const storageKeys = [currentKey];
    if (currentKey !== GUEST_THEME_KEY) {
      storageKeys.push(GUEST_THEME_KEY);
    }
    storageKeys.push(LEGACY_THEME_KEY);

    for (const key of storageKeys) {
      const stored = localStorage.getItem(key);
      if (stored === 'dark' || stored === 'light') {
        return stored;
      }
    }

    return 'light';
  }

  private storeTheme(theme: Theme): void {
    if (this.isBrowser) {
      localStorage.setItem(this.getStorageKey(), theme);
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

  private syncThemeForCurrentUser(isAuthenticated: boolean): void {
    if (!this.isBrowser) {
      return;
    }

    const previousTheme = this.currentTheme;
    const currentKey = this.getStorageKey();
    const storedTheme = this.getStoredTheme();

    if (storedTheme !== previousTheme) {
      this.themeSubject.next(storedTheme);
      this.applyTheme(storedTheme);
      localStorage.setItem(currentKey, storedTheme);
      return;
    }

    if (isAuthenticated) {
      localStorage.setItem(currentKey, previousTheme);
    }
  }

  private getStorageKey(): string {
    const userId = this.tokenService.getUserId();
    return userId ? `${THEME_KEY_PREFIX}${userId}` : GUEST_THEME_KEY;
  }
}
