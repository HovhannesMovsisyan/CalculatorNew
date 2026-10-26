# Web Calculator

A static (HTML/CSS/JS, no build step) version of the MAUI calculator. Same layout, colors and logic as `MainPage.xaml` / `MainPage.xaml.cs`, plus keyboard input.

| File | Purpose |
|---|---|
| `index.html` | Display and button grid |
| `style.css` | Colors from `Resources/Styles/Colors.xaml`, light and dark mode |
| `calculator.js` | Calculator logic and keyboard shortcuts |

Keyboard: `0-9`, `.`, `+ - * /`, `Enter` or `=`, `Backspace`, `Esc` / `C` to clear, `%`.

## Run locally

Open `index.html` in a browser. No server needed.

## Deploy (free)

Any of these work; the site is just the files in this folder.

### GitHub Pages (automatic)

The workflow `.github/workflows/deploy-web.yml` publishes this folder on every push to `main`.

1. Create a GitHub repository and push the project (`git init`, commit, add remote, push to `main`).
2. In the repository, go to **Settings → Pages** and set **Source** to **GitHub Actions**.
3. Push again (or run the workflow from the **Actions** tab). The site appears at `https://<user>.github.io/<repo>/`.

### Netlify

Sign in at [app.netlify.com](https://app.netlify.com), choose **Add new site → Deploy manually**, and drag this `web` folder onto the page.

### Cloudflare Pages

In the Cloudflare dashboard go to **Workers & Pages → Create → Pages → Upload assets** and upload this `web` folder.
