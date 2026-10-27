# Web Calculator

A static (HTML/CSS/JS, no build step) version of the MAUI calculator. Same layout, colors and logic as `MainPage.xaml` / `MainPage.xaml.cs`, plus keyboard input.

| File | Purpose |
|---|---|
| `index.html` | Display and button grid |
| `style.css` | Card layout, green operator keys, light/dark themes, responsive (phone portrait/landscape, desktop) |
| `calculator.js` | Calculator logic, keyboard shortcuts, result auto-shrink, theme toggle |

Keyboard: `0-9`, `.`, `+ - * /`, `Enter` or `=`, `Backspace`, `Esc` / `C` to clear, `%`.

Theme: follows the system setting until you click the sun/moon button; the choice is remembered in `localStorage`.

## Run locally

Open `index.html` in a browser. No server needed.

## Deploy

The site is hosted at https://calculatoor.click (Hetzner server, files in `/var/www/calculator`).
Upload only the three site files (not this README):

```
scp index.html style.css calculator.js deploy@2.31.79.19:/var/www/calculator/
```

When `style.css` or `calculator.js` change, bump the `?v=` number on their links in `index.html` so browsers don't keep an old cached copy.
