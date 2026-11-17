export type RouteLifecycle = 'planned' | 'implemented' | 'verified' | 'deprecated';

export type RouteCategory =
  | 'core'
  | 'features'
  | 'tools'
  | 'use-cases'
  | 'audience'
  | 'guides'
  | 'docs'
  | 'compare'
  | 'countries'
  | 'glossary'
  | 'trust'
  | 'auth';

export interface RouteMeta {
  path: string;
  title: string;
  description: string;
  category: RouteCategory;
  lifecycle: RouteLifecycle;
  isIndexable: boolean;
  canonical: string;
  h1: string;
  keywords?: string;
  ogType?: 'website' | 'article';
}

export const SITE_URL = "https://link.bt";

export const ROUTE_REGISTRY: RouteMeta[] = [
  // ==========================================
  // 1. CORE RETAINED PAGES (9 Routes)
  // ==========================================
  {
    path: "/",
    title: "LinkBT – Smart, Fast & Secure URL Shortener Platform",
    description: "Shorten long links, create custom aliases, generate scannable QR codes, and monitor click analytics with LinkBT's privacy-focused link management system.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/",
    h1: "Shorten, Secure & Measure Every Link",
    keywords: "url shortener, free link shortener, custom alias, qr code generator, link analytics"
  },
  {
    path: "/features/",
    title: "Features – Powerful URL Shortening & Security Tools | LinkBT",
    description: "Explore LinkBT's core features including custom vanity aliases, dynamic QR codes, password protection, expiration timers, and 3-engine link security scanning.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/features/",
    h1: "Engineered for Speed, Precision & Link Security",
    keywords: "url shortener features, password protected links, link expiration, dynamic qr codes"
  },
  {
    path: "/pricing/",
    title: "Pricing – Simple, Transparent Link Management Plans | LinkBT",
    description: "Choose the right LinkBT plan for your team. Free URL shortening with essential analytics, or upgrade for higher rate limits and extended retention.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/pricing/",
    h1: "Transparent Plans for Creators, Marketers & Developers",
    keywords: "link shortener pricing, free url shortener, enterprise link management"
  },
  {
    path: "/security/",
    title: "Security & Threat Protection – 3-Engine URL Scanning | LinkBT",
    description: "Discover how LinkBT protects users and visitors with automated Google Safe Browsing, URLhaus threat feeds, and Cloudflare Scanner multi-layer inspection.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/security/",
    h1: "Enterprise Security Built Into Every Redirect",
    keywords: "url shortener security, safe browsing, phishing protection, safe redirect"
  },
  {
    path: "/contact/",
    title: "Contact Us – Technical Support & Link Inquiries | LinkBT",
    description: "Get in touch with the LinkBT technical and security team for inquiries, API support, abuse reporting, or custom integration assistance.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/contact/",
    h1: "We're Here to Help Your Team Scale Securely",
    keywords: "contact linkbt, url shortener support, linkbt team"
  },
  {
    path: "/terms/",
    title: "Terms of Service – Usage Guidelines & Policies | LinkBT",
    description: "Review LinkBT's Terms of Service governing platform usage, API rate limits, prohibited content policies, and intellectual property terms.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/terms/",
    h1: "Terms of Service",
    keywords: "terms of service, acceptable use policy, linkbt terms"
  },
  {
    path: "/privacy-policy/",
    title: "Privacy Policy – Data Protection & Visitor Privacy | LinkBT",
    description: "Learn how LinkBT collects, protects, and handles user account data, link click analytics, and privacy compliance in accordance with global standards.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/privacy-policy/",
    h1: "Privacy Policy",
    keywords: "privacy policy, data privacy, visitor privacy protection"
  },
  {
    path: "/faq/",
    title: "Frequently Asked Questions (FAQ) – LinkBT Help Center",
    description: "Find clear answers to common questions about link shortening, custom aliases, QR codes, password protection, API authentication, and security scanners.",
    category: "core",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/faq/",
    h1: "Frequently Asked Questions",
    keywords: "url shortener faq, how to shorten links, linkbt help"
  },
  {
    path: "/login",
    title: "Sign In – LinkBT Account & Dashboard Access",
    description: "Sign in to your LinkBT dashboard to manage your shortened links, custom aliases, API keys, and campaign click performance analytics.",
    category: "auth",
    lifecycle: "verified",
    isIndexable: false, // Per strict requirements: noindex, follow, excluded from sitemap
    canonical: "https://link.bt/login",
    h1: "Sign In to Your LinkBT Account",
    keywords: "linkbt login, sign in, link management dashboard"
  },

  // ==========================================
  // 2. FEATURE DEEP-DIVES (7 Routes)
  // ==========================================
  {
    path: "/custom-aliases/",
    title: "Custom URL Aliases – Create Memorable Vanity Links | LinkBT",
    description: "Replace random short codes with memorable, branded vanity slugs on link.bt to increase link click-through rates and brand recognition.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/custom-aliases/",
    h1: "Custom URL Aliases That Drive Higher Click-Through Rates",
    keywords: "custom url alias, vanity url, memorable short links, custom slug shortener"
  },
  {
    path: "/branded-links/",
    title: "Branded Short Links – Build Trust with Every Click | LinkBT",
    description: "Enhance consumer trust and eliminate suspicious link appearances with clean, verified short URLs powered by LinkBT's high-reputation domain.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/branded-links/",
    h1: "Build Brand Trust with Professional Short Links",
    keywords: "branded links, trusted url shortener, professional short urls"
  },
  {
    path: "/link-expiration/",
    title: "Link Expiration – Set Automatic Expiry on Short URLs | LinkBT",
    description: "Schedule automatic link expiration dates and times for flash sales, event RSVPs, temporary promotions, and sensitive document sharing.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/link-expiration/",
    h1: "Time-Sensitive Links with Automated Expiration",
    keywords: "expiring links, temporary url shortener, timed link redirect, url expiration date"
  },
  {
    path: "/password-protected-links/",
    title: "Password Protected Links – Secure Sensitive URL Redirects | LinkBT",
    description: "Protect confidential presentations, client deliverables, and private file downloads with BCrypt-hashed password gates before redirecting.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/password-protected-links/",
    h1: "Secure Redirects with Password Protection",
    keywords: "password protect url, secure short link, private link redirect, gated short url"
  },
  {
    path: "/link-analytics/",
    title: "Link Analytics – Real-Time Click & Geographic Tracking | LinkBT",
    description: "Measure campaign ROI with live click analytics, country-level geographic breakdown, device/browser distribution, and referrer attribution.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/link-analytics/",
    h1: "Real-Time Click Insights and Geographic Performance",
    keywords: "link analytics, url click tracker, link tracking software, referrer analytics"
  },
  {
    path: "/bulk-shortener/",
    title: "Bulk URL Shortener – Shorten Multiple Links Simultaneously | LinkBT",
    description: "Save time by batch shortening up to 5 URLs in a single workflow. Clean sequential processing with rate-limit protection and instant CSV export.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/bulk-shortener/",
    h1: "Batch URL Shortening for Campaigns and Catalogs",
    keywords: "bulk url shortener, batch link shortener, shorten multiple links at once"
  },
  {
    path: "/qr-code-generator/",
    title: "Dynamic QR Code Generator – Track Scans and Attribution | LinkBT",
    description: "Generate crisp, trackable QR codes paired with your short links. Monitor scan counts and update destination URLs without reprinting materials.",
    category: "features",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/qr-code-generator/",
    h1: "Connect Offline Media to Online Campaigns with QR Codes",
    keywords: "qr code generator, dynamic qr codes, trackable qr code, qr scan analytics"
  },

  // ==========================================
  // 3. FREE ONLINE TOOLS & HUB (5 Routes)
  // ==========================================
  {
    path: "/tools/",
    title: "Free Link Management & Marketing Tools – LinkBT Tools Hub",
    description: "Access LinkBT's free suite of online utilities: UTM Campaign Builder, WhatsApp Chat Link Generator, QR Code Generator, and Bulk Shortener.",
    category: "tools",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/tools/",
    h1: "Free Web Marketing & Link Management Tools",
    keywords: "free marketing tools, url tools, link utilities, campaign tools"
  },
  {
    path: "/tools/utm-builder/",
    title: "Free UTM Builder – Google Analytics Campaign URL Generator | LinkBT",
    description: "Build clean, standard UTM-tagged campaign URLs for Google Analytics 4. Input source, medium, campaign name, term, and content with instant copy.",
    category: "tools",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/tools/utm-builder/",
    h1: "Free UTM Campaign URL Builder",
    keywords: "utm builder, google analytics campaign url builder, utm code generator"
  },
  {
    path: "/tools/whatsapp-link-generator/",
    title: "Free WhatsApp Link Generator – Click to Chat with Pre-Filled Text",
    description: "Create direct wa.me chat links with custom pre-filled messages. Share on social profiles, bios, and marketing pages for instant customer chats.",
    category: "tools",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/tools/whatsapp-link-generator/",
    h1: "Free WhatsApp Click-to-Chat Link Generator",
    keywords: "whatsapp link generator, click to chat whatsapp, wa me link creator"
  },
  {
    path: "/tools/qr-code-generator/",
    title: "Free Online QR Code Generator – Instant High-Res QR Downloads",
    description: "Generate free, high-contrast QR codes for any website URL, Wi-Fi network, or contact info. Download SVG/PNG formats with no signup required.",
    category: "tools",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/tools/qr-code-generator/",
    h1: "Instant Online QR Code Generator",
    keywords: "free qr code generator, online qr generator, create qr code free"
  },
  {
    path: "/tools/bulk-url-shortener/",
    title: "Free Bulk URL Shortener – Shorten Up to 5 Links Sequentially",
    description: "Shorten multiple URLs in batch with automatic rate limiting and copy-all functionality. Fast, secure, and completely free.",
    category: "tools",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/tools/bulk-url-shortener/",
    h1: "Free Sequential Bulk URL Shortener",
    keywords: "batch link shortener, bulk shorten urls, multiple link creator"
  },

  // ==========================================
  // 4. USE CASES & HUB (8 Routes)
  // ==========================================
  {
    path: "/use-cases/",
    title: "Use Cases – How Modern Teams Use LinkBT for Growth",
    description: "Explore practical use cases for short links and QR codes across social media, email campaigns, SMS marketing, print advertising, and affiliates.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/",
    h1: "Purpose-Built Link Management for Every Channel",
    keywords: "url shortener use cases, link marketing applications, link management scenarios"
  },
  {
    path: "/use-cases/social-media/",
    title: "URL Shortener for Social Media – Instagram, X, TikTok & LinkedIn",
    description: "Optimize character counts and track link clicks across social platforms with memorable custom aliases and bio links.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/social-media/",
    h1: "Maximize Click-Through Rates on Social Media",
    keywords: "social media url shortener, twitter short links, instagram bio link, tiktok short url"
  },
  {
    path: "/use-cases/email-marketing/",
    title: "Short Links for Email Marketing – Clean Links & High Deliverability",
    description: "Prevent link wrapping, avoid spam filters, and track subscriber engagement with secure, HTTPS-encrypted short URLs in marketing newsletters.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/email-marketing/",
    h1: "Clean, Trackable Links for Email Campaigns",
    keywords: "email marketing links, newsletter link shortener, email click tracking"
  },
  {
    path: "/use-cases/sms-marketing/",
    title: "URL Shortener for SMS Marketing – Save Characters & Boost Conversions",
    description: "Fit long destination URLs into standard 160-character SMS limits while tracking real-time delivery and click conversion metrics.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/sms-marketing/",
    h1: "Compact, High-Converting Short Links for SMS",
    keywords: "sms link shortener, text message short urls, sms marketing link tracking"
  },
  {
    path: "/use-cases/print-media/",
    title: "Short Links & QR Codes for Print Media – Billboards, Flyers & Menus",
    description: "Bridge physical collateral to digital destinations with concise, memorable URLs and high-resolution QR codes on print flyers and packaging.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/print-media/",
    h1: "Connect Print Advertising Directly to Digital Landing Pages",
    keywords: "print media qr codes, billboard short links, flyers qr code"
  },
  {
    path: "/use-cases/affiliate-marketing/",
    title: "Affiliate Link Cloaking & Tracking – Clean URLs with Attribution",
    description: "Clean up messy affiliate IDs and tracking tags with professional custom slugs while maintaining full attribution and referral analytics.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/affiliate-marketing/",
    h1: "Clean & Trackable Short URLs for Affiliate Marketers",
    keywords: "affiliate link cloaking, affiliate url shortener, referral link tracking"
  },
  {
    path: "/use-cases/event-management/",
    title: "Short Links for Events & Conferences – Ticketing & RSVP Tracking",
    description: "Simplify event promotions, ticket sales, and badge check-ins with short links and scannable QR codes configured with automatic expiration.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/event-management/",
    h1: "Streamline Event Registration & Attendee Check-Ins",
    keywords: "event short links, conference qr codes, ticketing url tracking"
  },
  {
    path: "/use-cases/customer-support/",
    title: "Short Links for Customer Support – Fast Help Desk Deliverables",
    description: "Provide quick, clean knowledge base links and support portal shortcuts over live chat, ticketing systems, and email conversations.",
    category: "use-cases",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/use-cases/customer-support/",
    h1: "Deliver Instant Support Solutions with Clean Links",
    keywords: "customer support links, helpdesk short links, ticketing url shortener"
  },

  // ==========================================
  // 5. AUDIENCE-SPECIFIC SOLUTIONS (7 Routes)
  // ==========================================
  {
    path: "/for/",
    title: "LinkBT Solutions – Tailored Link Tools for Every Professional",
    description: "Discover how marketers, developers, small businesses, content creators, and enterprise teams leverage LinkBT's secure shortening platform.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/",
    h1: "Tailored Solutions for Modern Professionals & Teams",
    keywords: "link solutions, url shortener by industry, linkbt for teams"
  },
  {
    path: "/for/marketers/",
    title: "LinkBT for Marketers – Multi-Channel Campaign Link Attribution",
    description: "Empower your marketing team with UTM tagging, custom brand aliases, campaign grouping, and real-time geographic click tracking.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/marketers/",
    h1: "Actionable Link Attribution for Performance Marketers",
    keywords: "url shortener for marketers, campaign link attribution, marketing short links"
  },
  {
    path: "/for/developers/",
    title: "LinkBT for Developers – High-Performance REST API & Automation",
    description: "Integrate programmatic link shortening into your software applications with simple JSON REST endpoints, API keys, and comprehensive docs.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/developers/",
    h1: "Robust REST APIs Built for High-Throughput Developer Workflows",
    keywords: "url shortener api, developer link api, programmatic link shortening"
  },
  {
    path: "/for/small-business/",
    title: "LinkBT for Small Business – Professional Marketing on a Budget",
    description: "Help your small business project enterprise-level professionalism with clean custom URLs, print-ready QR codes, and zero hidden fees.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/small-business/",
    h1: "Cost-Effective Link Management for Growing Businesses",
    keywords: "small business url shortener, affordable link tracker, local business qr codes"
  },
  {
    path: "/for/content-creators/",
    title: "LinkBT for Content Creators – Bio Links, Drops & Merch Tracking",
    description: "Drive traffic from YouTube, TikTok, and podcasts with clean custom slugs that viewers can easily type, remember, and click.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/content-creators/",
    h1: "Grow and Monetize Your Audience with Memorable Links",
    keywords: "link shortener for creators, podcast short links, youtube description links"
  },
  {
    path: "/for/enterprises/",
    title: "LinkBT for Enterprise – Secure, Compliant & Scalable Short Links",
    description: "Protect brand integrity with high-volume link infrastructure, multi-layer malware scanning, role-based controls, and 99.9% uptime SLA.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/enterprises/",
    h1: "Enterprise-Scale Link Governance & Security Infrastructure",
    keywords: "enterprise url shortener, scalable link platform, link compliance"
  },
  {
    path: "/for/educators/",
    title: "LinkBT for Educators & Non-Profits – Simple Resource Sharing",
    description: "Share classroom materials, research papers, and charity donation pages with simple short links that students and donors can easily access.",
    category: "audience",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/for/educators/",
    h1: "Simple, Accessible Resource Sharing for Schools & Non-Profits",
    keywords: "link shortener for schools, education url shortener, nonprofit link tracking"
  },

  // ==========================================
  // 6. EDUCATIONAL GUIDES & HUB (8 Routes)
  // ==========================================
  {
    path: "/guides/",
    title: "Guides & Tutorials – Mastering URL Shortening & Link Security",
    description: "Learn link optimization strategies, UTM tracking frameworks, QR code design best practices, and enterprise link security techniques.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/",
    h1: "Link Management Learning Center & Practical Guides",
    keywords: "url shortener guides, link optimization tutorials, link security best practices"
  },
  {
    path: "/guides/what-is-a-url-shortener/",
    title: "What is a URL Shortener? How Link Shortening Works | LinkBT Guide",
    description: "A comprehensive guide explaining HTTP 301 redirects, database hash lookups, custom aliases, and the mechanics of URL shortening services.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/what-is-a-url-shortener/",
    h1: "What is a URL Shortener? The Complete Technical & Practical Guide",
    keywords: "what is a url shortener, how url shortener works, 301 redirect explanation"
  },
  {
    path: "/guides/how-to-create-custom-short-links/",
    title: "How to Create Custom Short Links: Step-by-Step Tutorial | LinkBT",
    description: "Learn how to format, validate, and publish custom short aliases that protect brand trust and maximize engagement across marketing channels.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/how-to-create-custom-short-links/",
    h1: "How to Create Custom Short Links That Maximize CTR",
    keywords: "how to create custom short link, vanity link tutorial, customize url slug"
  },
  {
    path: "/guides/qr-code-best-practices/",
    title: "QR Code Best Practices: Sizing, Contrast & Print Guidelines",
    description: "Avoid unreadable QR codes. Read our complete guide to error correction levels, quiet zones, color contrast, and physical print sizing.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/qr-code-best-practices/",
    h1: "The Essential Guide to QR Code Design, Sizing & Printing",
    keywords: "qr code best practices, qr code print size, qr code quiet zone, dynamic qr guide"
  },
  {
    path: "/guides/utm-parameters-guide/",
    title: "Complete Guide to UTM Parameters: Tracking Campaign ROI",
    description: "Master campaign tracking. Understand utm_source, utm_medium, utm_campaign, utm_term, and utm_content with practical real-world examples.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/utm-parameters-guide/",
    h1: "Mastering UTM Parameters for Multi-Channel Attribution",
    keywords: "utm parameters guide, how to track marketing campaigns, google analytics utm tags"
  },
  {
    path: "/guides/link-management-best-practices/",
    title: "10 Link Management Best Practices for Marketing & IT Teams",
    description: "Prevent broken redirects, eliminate security vulnerabilities, and establish clear naming conventions across your company's link portfolio.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/link-management-best-practices/",
    h1: "10 Essential Link Management Best Practices for Modern Teams",
    keywords: "link management best practices, broken link prevention, url governance"
  },
  {
    path: "/guides/url-shortener-security/",
    title: "URL Shortener Security: Detecting Phishing, Malware & Scams",
    description: "Learn how modern URL shorteners utilize automated threat scanners, DNS blocklists, and preview interstitials to protect users from malicious redirects.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/url-shortener-security/",
    h1: "URL Shortener Security: Protecting Users from Harmful Redirects",
    keywords: "url shortener security, safe browsing api, phishing short links, malware redirect protection"
  },
  {
    path: "/guides/short-links-for-social-media/",
    title: "Optimizing Short Links for Social Media Bios, Posts & Ads",
    description: "Discover proven techniques for leveraging short URLs across Instagram, TikTok, LinkedIn, and X to maximize bio clicks and track ad attribution.",
    category: "guides",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/guides/short-links-for-social-media/",
    h1: "How to Optimize Short Links Across Social Media Platforms",
    keywords: "social media links guide, bio link optimization, social media click tracking"
  },

  // ==========================================
  // 7. DEVELOPER DOCUMENTATION & HUB (6 Routes)
  // ==========================================
  {
    path: "/docs/",
    title: "Developer REST API Reference & Documentation – LinkBT",
    description: "Comprehensive API documentation for integrating LinkBT URL shortening into your web applications, mobile apps, and automated workflows.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/",
    h1: "LinkBT Developer REST API Documentation",
    keywords: "url shortener api docs, rest api reference, developer documentation"
  },
  {
    path: "/docs/api-overview/",
    title: "API Architecture & Overview – Base URLs & Endpoints | LinkBT",
    description: "Learn about LinkBT API architecture, base endpoints, JSON request/response conventions, and global server response headers.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/api-overview/",
    h1: "API Architecture & Technical Overview",
    keywords: "api overview, rest api architecture, linkbt endpoints"
  },
  {
    path: "/docs/authentication/",
    title: "API Authentication – API Keys & Bearer Token Headers | LinkBT",
    description: "Authenticate API requests securely using LinkBT API keys. Review authorization header formats, token generation, and security recommendations.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/authentication/",
    h1: "Authenticating API Requests with Bearer Keys",
    keywords: "api authentication, api key header, bearer token auth"
  },
  {
    path: "/docs/create-short-link/",
    title: "Create Short Link Endpoint Reference – POST /api/developer/urls",
    description: "Endpoint reference for programmatically creating shortened URLs, custom aliases, expiration dates, and password protection via the LinkBT API.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/create-short-link/",
    h1: "Programmatic Link Creation Endpoint Reference",
    keywords: "create short url api, post request url shortener, programmatic link creation"
  },
  {
    path: "/docs/rate-limits/",
    title: "API Rate Limits & Quotas – HTTP 429 & Retry-After Policies",
    description: "Understand LinkBT API rate limits, hourly burst policies, HTTP 429 Too Many Requests responses, and exponential backoff retry strategies.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/rate-limits/",
    h1: "Rate Limits, Request Throttling & Backoff Handling",
    keywords: "api rate limits, 429 too many requests, retry-after header, api throttling"
  },
  {
    path: "/docs/error-handling/",
    title: "Error Handling & Status Codes – Standard API Error Codes | LinkBT",
    description: "Review LinkBT API status codes, error payload schemas, validation failure formats, and best practices for resilient client-side error handling.",
    category: "docs",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/docs/error-handling/",
    h1: "Standard Error Response Format & HTTP Status Codes",
    keywords: "api error handling, http status codes, rest api errors"
  },

  // ==========================================
  // 8. COMPETITOR COMPARISONS & HUB (5 Routes)
  // ==========================================
  {
    path: "/compare/",
    title: "URL Shortener Comparisons & Alternatives – LinkBT vs Competitors",
    description: "Compare LinkBT with popular URL shorteners including Bitly, TinyURL, Rebrandly, and Short.io on security features, pricing, and API reliability.",
    category: "compare",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/compare/",
    h1: "Fair, Feature-by-Feature URL Shortener Comparisons",
    keywords: "url shortener comparison, bitly alternative, tinyurl alternative, best link shortener"
  },
  {
    path: "/compare/linkbt-vs-bitly/",
    title: "LinkBT vs Bitly: Complete 2026 Comparison & Feature Breakdown",
    description: "Detailed comparison between LinkBT and Bitly. Compare free tier capabilities, custom alias limits, security threat scanning, and enterprise pricing.",
    category: "compare",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/compare/linkbt-vs-bitly/",
    h1: "LinkBT vs Bitly: Modern Link Management Compared",
    keywords: "linkbt vs bitly, bitly alternative, bitly pricing comparison"
  },
  {
    path: "/compare/linkbt-vs-tinyurl/",
    title: "LinkBT vs TinyURL: Which Link Shortener is Best for You in 2026?",
    description: "Analyze the differences between LinkBT and TinyURL across link analytics, password security, dynamic QR codes, and modern REST API support.",
    category: "compare",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/compare/linkbt-vs-tinyurl/",
    h1: "LinkBT vs TinyURL: Features, Analytics & Security",
    keywords: "linkbt vs tinyurl, tinyurl alternative, tinyurl vs modern shortener"
  },
  {
    path: "/compare/linkbt-vs-rebrandly/",
    title: "LinkBT vs Rebrandly: Comparing Custom Slugs, Pricing & Features",
    description: "Evaluate LinkBT and Rebrandly for link shortening, vanity alias support, team collaboration, and click tracking metrics.",
    category: "compare",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/compare/linkbt-vs-rebrandly/",
    h1: "LinkBT vs Rebrandly: Choosing the Right Platform",
    keywords: "linkbt vs rebrandly, rebrandly alternative, branded link tool comparison"
  },
  {
    path: "/compare/linkbt-vs-shortio/",
    title: "LinkBT vs Short.io: URL Shortening Features & Pricing Compared",
    description: "Compare LinkBT and Short.io on redirect speed, API simplicity, click metrics, automated threat prevention, and pricing tiers.",
    category: "compare",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/compare/linkbt-vs-shortio/",
    h1: "LinkBT vs Short.io: Detailed Technical Comparison",
    keywords: "linkbt vs shortio, short.io alternative, link platform comparison"
  },

  // ==========================================
  // 9. REGIONAL COUNTRY PAGES & HUB (9 Routes)
  // ==========================================
  {
    path: "/countries/",
    title: "Global Link Infrastructure – Low-Latency Shortening Worldwide",
    description: "Explore LinkBT's global edge network delivering sub-50ms link redirects and regional compliance for businesses across major international markets.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/",
    h1: "Global Edge Network for Instant Link Redirects",
    keywords: "global url shortener, worldwide link redirect, international link management"
  },
  {
    path: "/countries/url-shortener-india/",
    title: "URL Shortener in India – Fast, Secure Link Shortening for Indian Teams",
    description: "Optimized link shortening for digital businesses, e-commerce brands, and agencies in India with local UPI/WhatsApp marketing workflows and low latency.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-india/",
    h1: "Fast, Reliable URL Shortener for Businesses in India",
    keywords: "url shortener india, link shortener for indian businesses, whatsapp marketing short links"
  },
  {
    path: "/countries/url-shortener-us/",
    title: "URL Shortener in the United States – Enterprise Link Tracking",
    description: "High-performance link management for US marketing agencies, e-commerce retailers, and tech startups requiring robust analytics and 99.9% uptime.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-us/",
    h1: "High-Throughput Link Shortening for US Businesses",
    keywords: "url shortener us, link shortener usa, american marketing link tracker"
  },
  {
    path: "/countries/url-shortener-uk/",
    title: "URL Shortener in the United Kingdom – GDPR-Compliant Link Tracking",
    description: "UK-focused link management with privacy-first analytics, GDPR compliance, and fast redirect speeds across London and British networks.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-uk/",
    h1: "Privacy-Focused URL Shortener for UK Enterprises",
    keywords: "url shortener uk, gdpr compliant link shortener, british link management"
  },
  {
    path: "/countries/url-shortener-singapore/",
    title: "URL Shortener in Singapore – High-Speed Gateway for Southeast Asia",
    description: "Low-latency URL shortening and dynamic QR codes for fintech, retail, and tech enterprises operating across Singapore and the APAC region.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-singapore/",
    h1: "Ultra-Fast URL Shortening for Singapore & Southeast Asia",
    keywords: "url shortener singapore, apac link shortener, singapore link tracking"
  },
  {
    path: "/countries/url-shortener-uae/",
    title: "URL Shortener in UAE & Dubai – Smart Links for Middle East Businesses",
    description: "Secure, high-reliability link shortening and QR code generation for businesses, real estate firms, and retail brands in Dubai and the UAE.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-uae/",
    h1: "Smart URL Shortening for Businesses in UAE & GCC",
    keywords: "url shortener uae, dubai link shortener, middle east url tracker"
  },
  {
    path: "/countries/url-shortener-germany/",
    title: "URL Shortener in Germany – Privacy-First & DSGVO-Ready Short Links",
    description: "DSGVO-aligned link shortening and privacy-compliant analytics engineered for German companies and security-conscious European teams.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-germany/",
    h1: "Sicherer & DSGVO-Konformer URL Shortener für Deutschland",
    keywords: "url shortener germany, dsgvo link shortener, link kürzen deutschland"
  },
  {
    path: "/countries/url-shortener-australia/",
    title: "URL Shortener in Australia – Fast Sydney & Melbourne Edge Redirects",
    description: "Blazing fast link redirects across Sydney, Melbourne, and Brisbane with privacy protection and transparent campaign analytics for Australian brands.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-australia/",
    h1: "Fast, Reliable Link Shortening for Australian Businesses",
    keywords: "url shortener australia, aussie link shortener, sydney url tracking"
  },
  {
    path: "/countries/url-shortener-canada/",
    title: "URL Shortener in Canada – PIPEDA-Compliant Link Management",
    description: "Secure Canadian link management supporting bilingual marketing campaigns, PIPEDA compliance, and low-latency North American routing.",
    category: "countries",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/countries/url-shortener-canada/",
    h1: "Secure URL Shortening for Canadian Organizations",
    keywords: "url shortener canada, canadian link tracker, pipeda compliant shortener"
  },

  // ==========================================
  // 10. GLOSSARY TERMS & HUB (9 Routes)
  // ==========================================
  {
    path: "/glossary/",
    title: "Link Management & URL Shortening Glossary – Essential Terms",
    description: "Authoritative definitions and explanations of key concepts in link management, web redirects, QR codes, UTM parameters, and click analytics.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/",
    h1: "The Essential Link Management & URL Glossary",
    keywords: "url shortening glossary, link terminology, redirect definitions"
  },
  {
    path: "/glossary/url-shortener/",
    title: "What is a URL Shortener? Glossary Definition & How It Works | LinkBT",
    description: "Definition of a URL shortener: an online service that translates long web addresses into concise, trackable short URLs using HTTP redirects.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/url-shortener/",
    h1: "URL Shortener: Definition & Technical Architecture",
    keywords: "url shortener definition, what does url shortener mean, link shortener term"
  },
  {
    path: "/glossary/custom-alias/",
    title: "Custom Alias (Vanity URL) – Definition & Business Benefits | LinkBT",
    description: "Definition of a custom alias: a personalized slug selected by the user to replace an automatically generated random Base62 alphanumeric code.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/custom-alias/",
    h1: "Custom Alias (Vanity URL): Definition & Guide",
    keywords: "custom alias definition, vanity url meaning, custom slug term"
  },
  {
    path: "/glossary/branded-link/",
    title: "Branded Link – Definition, Trust Factors & Brand Recognition",
    description: "Definition of a branded link: a shortened link that incorporates a verified, recognizable domain or brand name to instill consumer confidence.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/branded-link/",
    h1: "Branded Link: Definition, Architecture & Benefits",
    keywords: "branded link definition, branded url meaning, custom domain link term"
  },
  {
    path: "/glossary/utm-parameters/",
    title: "UTM Parameters – Definition, Components & Campaign Tracking",
    description: "Definition of UTM parameters: standardized query string tags added to URLs to track marketing campaign effectiveness in analytics platforms.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/utm-parameters/",
    h1: "UTM Parameters: Definition & Tagging Breakdown",
    keywords: "utm parameters definition, what are utm tags, urchin tracking module"
  },
  {
    path: "/glossary/qr-code/",
    title: "QR Code (Quick Response Code) – Definition & Mechanics | LinkBT",
    description: "Definition of a QR code: a two-dimensional matrix barcode containing encoded alphanumeric data readable by smartphone cameras and optical scanners.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/qr-code/",
    h1: "QR Code (Quick Response Code): Definition & Technology",
    keywords: "qr code definition, quick response code meaning, 2d matrix barcode term"
  },
  {
    path: "/glossary/link-analytics/",
    title: "Link Analytics – Definition, Key Metrics & Attribution Models",
    description: "Definition of link analytics: quantitative measurement of click events, geographic distributions, referrers, and device types over time.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/link-analytics/",
    h1: "Link Analytics: Definition, Measurement & KPIs",
    keywords: "link analytics definition, click tracking metrics, url performance analysis"
  },
  {
    path: "/glossary/http-301-redirect/",
    title: "HTTP 301 Permanent Redirect – Definition & SEO Significance",
    description: "Definition of HTTP 301 Moved Permanently: the standard HTTP response status code informing browsers and search engines that a URL has moved.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/http-301-redirect/",
    h1: "HTTP 301 Redirect: Definition, Caching & SEO Impact",
    keywords: "http 301 redirect definition, 301 moved permanently meaning, permanent redirect seo"
  },
  {
    path: "/glossary/click-through-rate/",
    title: "Click-Through Rate (CTR) – Definition & Optimization for Short Links",
    description: "Definition of Click-Through Rate (CTR): the percentage of users who click a specific link compared to the total number of individuals who viewed it.",
    category: "glossary",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/glossary/click-through-rate/",
    h1: "Click-Through Rate (CTR): Definition & Link Optimization",
    keywords: "click through rate definition, ctr meaning, link ctr optimization"
  },

  // ==========================================
  // 11. TRUST, POLICY & CORPORATE (2 Routes)
  // ==========================================
  {
    path: "/about/",
    title: "About LinkBT – Our Mission, Security Architecture & Team",
    description: "Learn about LinkBT: our commitment to link privacy, multi-engine security scanning, high-uptime global infrastructure, and transparent tools.",
    category: "trust",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/about/",
    h1: "About LinkBT: Building a Safer, Faster Web for Everyone",
    keywords: "about linkbt, linkbt team, url shortener company, linkbt mission"
  },
  {
    path: "/report-abuse/",
    title: "Report Abuse – Report Malicious Links, Phishing & Malware | LinkBT",
    description: "Help keep the internet safe. Submit suspicious or malicious LinkBT short links for immediate automated scanning and administrative deactivation.",
    category: "trust",
    lifecycle: "verified",
    isIndexable: true,
    canonical: "https://link.bt/report-abuse/",
    h1: "Report Malicious or Abusive Short Links",
    keywords: "report abuse, report phishing link, flag malicious url, report linkbt link"
  }
];

export const ROUTE_BY_PATH: Record<string, RouteMeta> = Object.fromEntries(
  ROUTE_REGISTRY.map(r => [r.path, r])
);

/**
 * Returns list of paths that should be prerendered during build.
 * Includes both 'implemented' and 'verified' routes.
 */
export function getPrerenderRoutes(): string[] {
  return ROUTE_REGISTRY
    .filter(r => r.lifecycle === 'implemented' || r.lifecycle === 'verified')
    .map(r => r.path);
}

/**
 * Returns list of routes eligible for sitemap inclusion.
 * Strictly requires: lifecycle === 'verified' AND isIndexable === true.
 */
export function getSitemapRoutes(): RouteMeta[] {
  return ROUTE_REGISTRY.filter(r => r.lifecycle === 'verified' && r.isIndexable);
}

/**
 * Helper to query routes by category
 */
export function getCategoryRoutes(category: RouteCategory): RouteMeta[] {
  return ROUTE_REGISTRY.filter(r => r.category === category);
}

