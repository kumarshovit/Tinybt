export interface SeoPageContent {
  slug: string;
  category: string;
  badge: string;
  title: string;
  subtitle: string;
  introParagraphs: string[];
  keyBenefits: { title: string; description: string }[];
  deepDiveSections: { heading: string; content: string }[];
  faqs: { question: string; answer: string }[];
  relatedSlugs?: string[];
}

export const MARKETING_PAGES: Record<string, SeoPageContent> = {
  // ==========================================
  // FEATURE DEEP-DIVES
  // ==========================================
  "/custom-aliases/": {
    slug: "/custom-aliases/",
    category: "features",
    badge: "Core Feature",
    title: "Custom URL Aliases That Drive Higher Click-Through Rates",
    subtitle: "Replace confusing random short codes with memorable, brand-aligned slugs on link.bt to increase CTR and brand recognition.",
    introParagraphs: [
      "When users see a generic shortened link with random alphanumeric characters like bit.ly/3xJ9qZ, hesitation is natural. In an era where phishing attacks are increasingly sophisticated, visitors want to know what they are clicking before they commit.",
      "Custom URL aliases solve this problem by transforming unreadable strings into descriptive, meaningful links such as link.bt/spring-sale or link.bt/webinar-replay. By communicating context upfront, custom vanity URLs build immediate trust and dramatically improve engagement."
    ],
    keyBenefits: [
      {
        title: "Up to 34% Higher Click-Through Rates",
        description: "Industry benchmarks consistently demonstrate that descriptive custom links achieve up to 34% higher CTR compared to unbranded random strings."
      },
      {
        title: "Immediate Search & Visual Recognition",
        description: "Custom slugs make links easy to remember, pronounce aloud on podcasts, and type into a mobile browser without errors."
      },
      {
        title: "Strict Uniqueness & Collision Protection",
        description: "LinkBT enforces case-insensitive uniqueness and reserved keyword protection to guarantee your alias is permanently assigned to your link."
      }
    ],
    deepDiveSections: [
      {
        heading: "How to Choose an Effective Custom Alias",
        content: "Keep your custom slug between 5 and 25 characters. Use hyphens to separate words for improved readability on mobile devices. Avoid ambiguous numbers like '0' and letters like 'O', and ensure the slug clearly reflects the target content."
      },
      {
        heading: "Automated Collision Protection & Validation",
        content: "LinkBT validates custom aliases against reserved system words (e.g. login, api, security) and checks live database indices before confirming creation. If an alias is already in use, our API immediately provides feedback so you can select an alternate variation."
      }
    ],
    faqs: [
      {
        question: "Can I edit my custom alias after creating it?",
        answer: "Yes, authenticated LinkBT users can update their custom alias from the dashboard at any time, provided the new alias is not already taken."
      },
      {
        question: "Are custom aliases case-sensitive?",
        answer: "No. LinkBT custom aliases are normalized to lowercase and resolved case-insensitively, ensuring links work regardless of how users type them."
      }
    ]
  },

  "/branded-links/": {
    slug: "/branded-links/",
    category: "features",
    badge: "Brand Identity",
    title: "Build Brand Trust with Professional Short Links",
    subtitle: "Elevate your marketing touchpoints with clean, verified short URLs powered by LinkBT's high-reputation domain.",
    introParagraphs: [
      "Every link your organization shares is an extension of your brand identity. Long, cluttered URLs packed with tracking tokens, affiliate parameters, and session IDs degrade professionalism and reduce user confidence.",
      "With LinkBT, you transform complex web addresses into sleek, credible short links. Our domain reputation and strict spam-filtering policies ensure your links are welcomed across social networks, messaging apps, and email gateways."
    ],
    keyBenefits: [
      {
        title: "Protect Brand Integrity",
        description: "Eliminate spam-like link appearances and maintain consistent visual branding across all marketing channels."
      },
      {
        title: "Improved Email Deliverability",
        description: "Clean, HTTPS-encrypted short links avoid spam filters and link-wrapping issues in popular email clients."
      },
      {
        title: "Consistent Multi-Channel Attribution",
        description: "Track performance across SMS, social media, and digital advertising without altering the underlying landing page."
      }
    ],
    deepDiveSections: [
      {
        heading: "Why Domain Reputation Matters",
        content: "Email providers and social platforms penalize links from domains associated with spam. LinkBT actively polices its redirect index using automated 3-engine security scanning, preserving optimal domain health for all legitimate users."
      }
    ],
    faqs: [
      {
        question: "How do branded short links help email marketing?",
        answer: "They prevent long tracking URLs from breaking across lines and keep message formatting clean, reducing spam score penalties."
      }
    ]
  },

  "/link-expiration/": {
    slug: "/link-expiration/",
    category: "features",
    badge: "Link Governance",
    title: "Time-Sensitive Links with Automated Expiration",
    subtitle: "Schedule automatic expiration dates and times for flash sales, event RSVPs, temporary promotions, and sensitive document sharing.",
    introParagraphs: [
      "Not every link should live forever. Flash sales end, event tickets sell out, and confidential draft documents must not remain accessible indefinitely.",
      "LinkBT provides granular link expiration controls. You can assign an exact UTC date and time to any short link. Once the expiration threshold is reached, LinkBT automatically reroutes visitors to a dedicated expired page or an alternate target URL."
    ],
    keyBenefits: [
      {
        title: "Automated Flash Sale Management",
        description: "Ensure customers never see outdated prices or expired discount codes after a promotion has concluded."
      },
      {
        title: "Confidential Document Sharing",
        description: "Set contracts, proposals, and press releases to expire automatically after a specified review window."
      },
      {
        title: "Prevent Broken Dead-Ends",
        description: "Visitors to expired links receive clear context rather than confusing 404 errors or broken page states."
      }
    ],
    deepDiveSections: [
      {
        heading: "Handling Expired Redirects Gracefully",
        content: "When an expired link is clicked, LinkBT serves an HTTP 410 Gone status or redirects to a user-friendly interstitial informing the visitor that the campaign has ended, protecting your brand reputation."
      }
    ],
    faqs: [
      {
        question: "Can I reactivate an expired link?",
        answer: "Yes, account owners can edit the expiration date in their dashboard to extend link validity at any time."
      }
    ]
  },

  "/password-protected-links/": {
    slug: "/password-protected-links/",
    category: "features",
    badge: "Access Security",
    title: "Secure Redirects with Password Protection",
    subtitle: "Protect confidential presentations, client deliverables, and private file downloads with BCrypt-hashed password gates before redirecting.",
    introParagraphs: [
      "Sharing sensitive files, staging environments, or internal team resources over public messaging channels poses serious security risks if a link is accidentally forwarded.",
      "LinkBT's password protection adds a secure authentication gate directly in front of your redirect. Visitors must enter the correct passphrase before being forwarded to the destination URL."
    ],
    keyBenefits: [
      {
        title: "BCrypt Cryptographic Hashing",
        description: "Passwords are never stored in plain text. LinkBT secures credentials using salted, computationally robust BCrypt hashing."
      },
      {
        title: "Brute-Force Rate Limiting",
        description: "Automated lockout mechanisms prevent brute-force passcode guessing on protected links."
      },
      {
        title: "Zero Client Leaks",
        description: "The destination URL is never exposed in the browser DOM until the correct password has been successfully verified."
      }
    ],
    deepDiveSections: [
      {
        heading: "Ideal Scenarios for Password Protection",
        content: "Common use cases include sharing confidential pitch decks with investors, distributing pre-release press kits to journalists, and granting temporary client access to staging web servers."
      }
    ],
    faqs: [
      {
        question: "Is the password visible in link analytics?",
        answer: "No. Passwords are strictly hashed and excluded from all click logs, analytics dashboards, and API responses."
      }
    ]
  },

  "/link-analytics/": {
    slug: "/link-analytics/",
    category: "features",
    badge: "Performance Intelligence",
    title: "Real-Time Click Insights and Geographic Performance",
    subtitle: "Measure campaign ROI with live click analytics, country-level geographic breakdown, device/browser distribution, and referrer attribution.",
    introParagraphs: [
      "Data is the foundation of effective marketing. Without visibility into who is clicking your links, when they engage, and which channels convert best, marketing budgets are spent blindly.",
      "LinkBT provides real-time, privacy-focused link analytics. Every redirect records essential performance telemetry—including country of origin, referrer domain, operating system, and device type—without compromising visitor privacy."
    ],
    keyBenefits: [
      {
        title: "Real-Time Telemetry",
        description: "Monitor click velocity instantly as your campaign launches on social media, email, or digital billboards."
      },
      {
        title: "Geographic Mapping",
        description: "Identify top-performing regional markets to refine localization strategies and ad targeting."
      },
      {
        title: "Referrer Attribution",
        description: "Compare engagement across platforms like X, LinkedIn, Instagram, and direct messaging channels."
      }
    ],
    deepDiveSections: [
      {
        heading: "Privacy-Preserving Analytics",
        content: "LinkBT does not track individual visitor identities across the web. We record aggregate metrics and truncate IP addresses, fully complying with GDPR, CCPA, and global privacy standards."
      }
    ],
    faqs: [
      {
        question: "How fast do clicks appear in the dashboard?",
        answer: "Click events are processed asynchronously and typically appear in your dashboard within seconds."
      }
    ]
  },

  "/bulk-shortener/": {
    slug: "/bulk-shortener/",
    category: "features",
    badge: "Workflow Efficiency",
    title: "Batch URL Shortening for Campaigns and Catalogs",
    subtitle: "Save time by batch shortening multiple URLs in a single workflow. Clean sequential processing with rate-limit protection and instant CSV export.",
    introParagraphs: [
      "Shortening dozens of product URLs or campaign links individually is tedious and prone to human error. E-commerce managers, content publishers, and media agencies need streamlined batch tools.",
      "LinkBT's Bulk Shortener allows you to paste multiple long URLs and generate short links sequentially with built-in rate-limit protection, error recovery, and copy-all export."
    ],
    keyBenefits: [
      {
        title: "Sequential Rate-Limit Protection",
        description: "Built-in throttling prevents 429 Too Many Requests errors and ensures 100% completion reliability."
      },
      {
        title: "Partial Failure Recovery",
        description: "If an individual URL in the batch is invalid, the remaining valid links continue processing uninterrupted."
      },
      {
        title: "One-Click Copy & Export",
        description: "Copy all generated short links to your clipboard with a single click or export them to spreadsheets."
      }
    ],
    deepDiveSections: [
      {
        heading: "Optimized for High-Volume Marketers",
        content: "Ideal for SMS marketing blasts, e-commerce catalog drops, and multi-variant ad testing where multiple unique links are required simultaneously."
      }
    ],
    faqs: [
      {
        question: "What is the maximum batch size?",
        answer: "The free web tool processes up to 5 URLs per batch. For unlimited batch creation, developers can leverage our REST API."
      }
    ]
  },

  "/qr-code-generator/": {
    slug: "/qr-code-generator/",
    category: "features",
    badge: "Offline to Online",
    title: "Connect Offline Media to Online Campaigns with QR Codes",
    subtitle: "Generate crisp, trackable QR codes paired with your short links. Monitor scan counts and update destination URLs without reprinting materials.",
    introParagraphs: [
      "Printed materials like business cards, product boxes, restaurant menus, and trade show banners have a major flaw: once printed, mistakes cannot be fixed and engagement cannot be measured.",
      "Dynamic QR codes from LinkBT solve both problems. By routing your QR code through a short redirect link, you can track scan counts in real time and change the target URL whenever needed without reprinting a single flyer."
    ],
    keyBenefits: [
      {
        title: "Dynamic Target Updating",
        description: "Update the destination webpage anytime from your dashboard while keeping the physical printed QR code unchanged."
      },
      {
        title: "Scan Attribution & Metrics",
        description: "Separate QR code scans from direct link clicks to measure the true effectiveness of offline marketing collateral."
      },
      {
        title: "High-Resolution Vector SVG",
        description: "Download scalable vector graphics suitable for everything from tiny business cards to giant roadside billboards."
      }
    ],
    deepDiveSections: [
      {
        heading: "Distinct QR Routing Architecture",
        content: "LinkBT routes QR scans through specialized /q/:code handlers, allowing your analytics dashboard to clearly distinguish between direct web clicks and physical camera scans."
      }
    ],
    faqs: [
      {
        question: "Will my QR code expire?",
        answer: "No, LinkBT dynamic QR codes remain active indefinitely as long as your account and link remain in good standing."
      }
    ]
  }
};

