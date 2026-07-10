# Complete SEO Audit — LinkBT (link.bt)

**Date:** July 8, 2026
**Audited by:** Automated LinkBT Project Analysis 

---

## 🏎 Overall SEO Score: **62 / 100**

| Category | Score / Max | Status |
|---|---|---|
| Technical SEO | 18 / 25 | 🟡 Fair |
| On-Page SEO | 19 / 20 | 🟢 Excellent |
| Content & Keywords | 6 / 15 | 🔴 Poor |
| Structured Data | 8 / 10 | 🟢 Good |
| Crawlability & Indexing | 8 / 15 | 🔴 Poor |
| Off-Page & Authority | 3 / 15 | 🔴 Poor |
| **Total** | **62 / 100** | Needs Attention |

---

## 1. Technical SEO Correctness & Implementation

### ✅ What is Technically Correct:
1. **Dynamic Page Titles & Meta Tags:** The `react-helmet-async` configuration works seamlessly across the SPA. 
2. **Canonical Links:** Set efficiently through the `<SEO>` component to prevent duplicate content issues.
3. **Open Graph & Twitter Cards:** Configured correctly on root (`index.html`) and recursively driven through `SEO.tsx` on individual pages.
4. **Prerendering Configuration:** The SSR pipeline efficiently builds static payloads for your routes inside `dist-ssr/` via `scripts/prerender.mjs` and `src/entry-prerender.tsx`. All routes are documented properly here!
5. **XML Sitemap Setup:** `public/sitemap.xml` correctly references your 7 core public pages and now appropriately supplies `<lastmod>` properties.
6. **Robots Configuration:** Good job hiding private user endpoints (`/dashboard`, `/analytics`, `/profile`, `/admin`, `/tags/`) inside `public/robots.txt`.

### 🔴 What is Missing or Flawed (Technical SEO Components):
1. **No Web App Manifest:** Missing `manifest.json` / `site.webmanifest`. This limits ranking indicators on mobile and impacts PWA capabilities.
2. **Unsupported iOS Icons:** The `apple-touch-icon` points to an SVG (`favicon.svg`). iOS requires a `180x180` PNG.
3. **Bloated Favicon:** The `favicon.svg` is quite large (approx 414KB), negatively shifting page performance and Core Web Vitals on cold loads. 

---

## 2. React SPA Rendering Issues impact on SEO

### 🔴 Prerender Output Caching vs Deployment Error
Your SPA uses a robust static pre-render process (`scripts/prerender.mjs`). This script essentially "snapshots" the DOM structure of your react code and dumps it into unique `dist/route/index.html` pathways. 
**The Issue:** Ensure that your hosting provider (like Vercel, Netlify, or Nginx configuration) actually routes external requests direct to these pre-rendered folders instead of just throwing all web traffic down the universal `index.html` fallback!
**Risk:** If not configured properly on the server side, Googlebot encounters a blank `<div id="root"></div>` shell and may defer your Javascript execution phase for up to multiple weeks, blocking indexation for an indefinite length of time.

### 🔴 The Soft 404 Problem
```tsx
// App.tsx
<Route path="*" element={<Navigate to="/" />} />
```
Currently, any failed access on the site automatically resolves a `200 OK` status before redirecting to the root `/` URL in the SPA environment. Google perceives this as thousands of real paths having identical information instead of rejecting them. 
**Fix:** You need to throw an explicit `NotFoundPage.tsx` interface and emit proper `404 Status Codes` natively or through Cloudflare / Host routing configurations if users fail a route ping (`/:shortCode` is particularly vulnerable to ghost URLs).

---

## 3. Crawlability & Indexing Factors

### Missing Crucial GSC Connection
While I cannot verify the internet connection from here, there is **0 internal metadata indicating Google Search Console verification** (no `google-site-verification` header tokens or root verification `.html` files).
If Search Console is missing, Google won’t even know you exist. You must request a manual crawl and deploy your newly fixed sitemap for them to review, otherwise ranking is left indefinitely to natural bot discoveries which may never materialize due to lack of domain authority.

---

## 4. Current Keyword Strategy & Competition

With title tags recently optimized (`"Free URL Shortener – Shorten Links & Track Clicks | LinkBT"`), the on-page targeting relies heavily on the phrases: **"Free URL Shortener", "Brand Custom Aliases", "Link Tracking"**.

**Is it competitive?** Extremely!
Keywords like `"URL Shortener"` or `"Free URL Shortener"` are globally saturated with competitors generating 1.2M+ search volume per month holding **DA (Domain Authority) of 85+ (Bitly, TinyURL, Rebrandly).**

### 🔴 The Thin Content Trap
Currently, the `/` landing page serves approximately 60-80 visible body words. 
Google evaluates page hierarchy heavily based on the substantive word count. Anything beneath ~400 words is considered "Thin Text Content" for these kinds of utilities, blocking ranking capabilities against entrenched competitors boasting massive user blogs, FAQ blocks, and API tutorials. 

---

## 5. Structural Data Implementation

### ✅ Good Execution:
The schema implementation (`@type: WebSite`, `@type: Organization`, `@type: SoftwareApplication`) inside `LandingPage.tsx` successfully highlights context metadata. 

### 🟡 Mild Corrections Needed:
1. **No `<SearchAction>` property:** Without `"potentialAction": {"@type": "SearchAction"}`, you disqualify yourself from the Sitelinks Search Box directly on Google results.
2. **Missing `aggregateRating`:** The Software module dictates no public software ratings, meaning your search card won't manifest the ⭐️⭐️⭐️⭐️⭐️ golden stars natively in search rows.

---

## 6. Action Plan & Prioritized Fixes

Here is the exact step-by-step roadmap ordered by maximum organic impact:

### Priority 1: Ensure Crawl Visibility (CRITICAL)
1. **Connect & verify Google Search Console (GSC).**
2. **Ensure Prerender Alignment:** Manually test `view-source:https://link.bt/login` and verify HTML outputs content inside `<div id="root">`, ensuring the host server recognizes dynamic SSG routing instead of just basic SPA fallbacks. 
3. **Destroy Soft 404s:** Stop redirecting `*` to the landing page. Display a genuine 404 component instead, guaranteeing Google prunes dead short-link queries instead of cannibalizing the root domain. 

### Priority 2: Fix Content Depth & Relevance (HIGH)
4. **Expand Landing Page:** Break out of the thin text penalty loop. Add 500+ words to the landing page through an extensive **FAQ grid** (`How do URL shorteners work?`, `Does LinkBT charge for analytics?`). Include `FAQPage` JSON-LD schema metadata on top of this.
5. **Establish a `/blog` branch:** Begin writing Long-tail ranking content that Bitly overlooks (e.g. "How to use branded links in Real Estate Marketing"). 

### Priority 3: Eliminate Technical Friction (MEDIUM)
6. **Implement Breadcrumbs Schema:** Create `"@type": "BreadcrumbList"` JSON bindings for `Privacy Policy`, `Contact` and `Terms`. 
7. **Downsize Media / Export Mobile Configs:** Shrink your heavyweight `400KB+` `favicon.svg` into small 128x128 byte chunks, and build out `manifest.json`. Map Apple Safari interfaces directly to `.png` equivalent `apple-touch-icon`.