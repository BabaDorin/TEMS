/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.{html,ts}',
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        // iOS-style accent colors
        accent: {
          DEFAULT: '#007aff',
          hover: '#0051d5',
        },
        // Dark mode specific colors
        dark: {
          bg: {
            primary: '#1c1c1e',
            secondary: '#2c2c2e',
            tertiary: '#3a3a3c',
            elevated: '#2c2c2e',
          },
          surface: {
            DEFAULT: '#1c1c1e',
            raised: '#2c2c2e',
            overlay: '#3a3a3c',
          },
          border: {
            DEFAULT: '#38383a',
            subtle: '#2c2c2e',
          },
        },
      },
    },
  },
  plugins: [],
};
