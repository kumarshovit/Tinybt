import type { SeoPageContent } from "./marketingData";

export const AUDIENCE_PAGES: Record<string, SeoPageContent> = {
  "/for/marketers/": {
    slug: "/for/marketers/",
    category: "audience",
    badge: "For Marketers",
    title: "Actionable Link Attribution for Performance Marketers",
    subtitle: "Empower your growth marketing team with UTM tagging, custom brand aliases, campaign grouping, and real-time geographic click tracking.",
    introParagraphs: [
      "Modern marketing teams run campaigns across dozens of disparate channels: paid search, organic social, influencer partnerships, email blasts, and affiliate networks. Tracking which specific channel drove a high-value conversion is essential for optimizing ad spend.",
      "LinkBT provides performance marketers with a unified link intelligence platform. Build standard UTM campaign links in seconds, wrap them in trustworthy custom aliases, and monitor live click velocity and geographic distribution in real time."
    ],
    keyBenefits: [
      {
        title: "Multi-Channel Attribution",
        description: "Accurately compare engagement across Facebook, TikTok, Google Ads, and email newsletters using standardized parameters."
      },
      {
        title: "Higher Conversion Rates",
        description: "Custom vanity URLs communicate trust and encourage more clicks than unbranded, suspicious strings."
      },
      {
        title: "Integrated QR Generator",
        description: "Connect offline experiential marketing, event booths, and print collateral directly to your digital conversion funnels."
      }
    ],
    deepDiveSections: [
      {
        heading: "Campaign Organization at Scale",
        content: "Organize links by product line, quarterly campaign, or acquisition channel. Use tags and custom aliases to keep your link library structured as your team grows."
      }
    ],
    faqs: [
      {
        question: "Can I export click data for reports?",
        answer: "Yes, LinkBT provides exportable analytics reports and raw data access for marketing intelligence dashboards."
      }
    ]
  },

  "/for/developers/": {
    slug: "/for/developers/",
    category: "audience",
    badge: "For Developers",
    title: "Robust REST APIs Built for High-Throughput Developer Workflows",
    subtitle: "Integrate programmatic link shortening into your applications with simple JSON REST endpoints, API keys, and comprehensive documentation.",
    introParagraphs: [
      "Software engineers building transactional email systems, automated SMS notifications, user-generated content platforms, and e-commerce checkouts need reliable, low-latency programmatic link shortening.",
      "LinkBT's developer platform is built on modern .NET 10 architecture with asynchronous I/O and strict rate-limiting policies. Generate short links, configure custom aliases, and retrieve click stats with standard HTTP requests."
    ],
    keyBenefits: [
      {
        title: "Simple Bearer Key Authentication",
        description: "Authenticate API calls with straightforward API key headers without complex OAuth handshakes for backend microservices."
      },
      {
        title: "Sub-50ms Response Latency",
        description: "Optimized database indices and connection pooling ensure lightning-fast short-code generation."
      },
      {
        title: "Predictable JSON Payloads",
        description: "Clean, strongly-typed JSON responses make client-side deserialization seamless in Node.js, Python, Go, and C#."
      }
    ],
    deepDiveSections: [
      {
        heading: "Production Reliability Standards",
        content: "Our backend utilizes SQL Server unique constraint enforcement with bounded exponential retries, guaranteeing zero short-code collision failures even during high-concurrency bursts."
      }
    ],
    faqs: [
      {
        question: "Where can I find the API documentation?",
        answer: "Comprehensive REST API docs with request schemas, cURL examples, and error codes are available in our Developer Docs."
      }
    ]
  },

  "/for/small-business/": {
    slug: "/for/small-business/",
    category: "audience",
    badge: "For Small Businesses",
    title: "Cost-Effective Link Management for Growing Businesses",
    subtitle: "Help your small business project enterprise-level professionalism with clean custom URLs, print-ready QR codes, and zero hidden fees.",
    introParagraphs: [
      "Small business owners often get priced out of legacy link shorteners that hide basic features like custom aliases or QR code downloads behind expensive enterprise contracts.",
      "LinkBT provides growing businesses with enterprise-grade link tools at accessible pricing. Build customer trust with clean links, print QR codes for your storefront, and track which promotions drive the most customers."
    ],
    keyBenefits: [
      {
        title: "Professional Brand Image",
        description: "Look polished and established with clean, HTTPS-encrypted short links on your invoices, receipts, and social profiles."
      },
      {
        title: "Print-Ready QR Codes for Stores",
        description: "Create scannable QR codes for your store window, table tents, flyers, or business cards that direct customers to your menu or Google reviews."
      },
      {
        title: "Zero Hidden Fees",
        description: "Access essential features like custom slugs, password protection, and click tracking without surprise billing tiers."
      }
    ],
    deepDiveSections: [
      {
        heading: "Local Business Marketing Ideas",
        content: "Use dynamic QR codes on restaurant tables to display digital menus. Update the destination URL daily with fresh specials without reprinting physical acrylic holders."
      }
    ],
    faqs: [
      {
        question: "Is there a free plan for small businesses?",
        answer: "Yes, LinkBT offers a generous free tier with link shortening, basic analytics, and QR code generation."
      }
    ]
  },

  "/for/content-creators/": {
    slug: "/for/content-creators/",
    category: "audience",
    badge: "For Creators",
    title: "Grow and Monetize Your Audience with Memorable Links",
    subtitle: "Drive traffic from YouTube, TikTok, podcasts, and newsletters with clean custom slugs that viewers can easily type, remember, and click.",
    introParagraphs: [
      "As a content creator, your audience engages with your content across various mediums: listening in their cars, watching on televisions, or scrolling on mobile phones. Long, unpronounceable URLs lose traffic because viewers cannot type them easily.",
      "LinkBT allows YouTubers, podcasters, streamers, and authors to create concise vanity slugs like link.bt/gear or link.bt/book that can be stated out loud in audio recordings or featured prominently on screen."
    ],
    keyBenefits: [
      {
        title: "Podcast & Video Friendly",
        description: "Create memorable links that listeners can easily recall and type into their browsers after listening to your show."
      },
      {
        title: "Bio Link & Merch Attribution",
        description: "Measure exactly how many sales your latest merch drop or sponsorship generated using clean LinkBT short links."
      },
      {
        title: "Single Link Updating",
        description: "Keep your historical video descriptions accurate by updating the redirect destination whenever your affiliate partners change."
      }
    ],
    deepDiveSections: [
      {
        heading: "Optimizing Video Descriptions",
        content: "Place your key LinkBT short links above the 'Show More' fold in YouTube descriptions to capture immediate clicks without requiring viewers to expand the text."
      }
    ],
    faqs: [
      {
        question: "Can I use LinkBT for my affiliate sponsors?",
        answer: "Yes, you can shorten and track all your sponsor links while preserving all affiliate tracking cookies upon redirect."
      }
    ]
  },

  "/for/enterprises/": {
    slug: "/for/enterprises/",
    category: "audience",
    badge: "Enterprise Grade",
    title: "Enterprise-Scale Link Governance & Security Infrastructure",
    subtitle: "Protect brand integrity with high-volume link infrastructure, multi-layer malware scanning, role-based controls, and 99.9% uptime SLA.",
    introParagraphs: [
      "Large enterprise organizations manage hundreds of thousands of links across global departments, brands, and customer touchpoints. Unmonitored link shortening creates compliance risks, broken redirects, and brand vulnerabilities.",
      "LinkBT provides enterprises with a secure, centralized link governance platform. Safeguard your brand reputation with automated 3-engine malware detection, role-based access controls, and carrier-grade 99.9% redirect uptime."
    ],
    keyBenefits: [
      {
        title: "Automated Threat Defense",
        description: "Every link is continuously screened against Google Safe Browsing, URLhaus feeds, and Cloudflare Scanner to prevent brand association with malicious content."
      },
      {
        title: "Carrier-Grade 99.9% Uptime",
        description: "Geographically distributed edge nodes and redundant SQL Server clusters guarantee immediate, reliable redirects worldwide."
      },
      {
        title: "Centralized Audit Logs & Governance",
        description: "Maintain full visibility into who created each short link, when targets were modified, and team activity logs."
      }
    ],
    deepDiveSections: [
      {
        heading: "Enterprise Compliance & Data Privacy",
        content: "LinkBT complies with global privacy mandates including GDPR and CCPA. IP addresses are truncated to protect consumer anonymity, and data is never sold to third-party ad brokers."
      }
    ],
    faqs: [
      {
        question: "Does LinkBT support dedicated enterprise SLAs?",
        answer: "Yes, enterprise agreements include custom rate limits, priority support, and 99.9% uptime service level guarantees."
      }
    ]
  },

  "/for/educators/": {
    slug: "/for/educators/",
    category: "audience",
    badge: "Education & Non-Profit",
    title: "Simple, Accessible Resource Sharing for Schools & Non-Profits",
    subtitle: "Share classroom materials, research papers, and charity donation pages with simple short links that students and donors can easily access.",
    introParagraphs: [
      "Educators, researchers, and charitable organizations often need to share complex academic documents, survey forms, and donation portals with diverse audiences who may not be tech-savvy.",
      "LinkBT simplifies communication by turning lengthy cloud storage links (like Google Drive or OneDrive URLs) into simple, clean short codes that students and supporters can easily open on any device."
    ],
    keyBenefits: [
      {
        title: "Classroom Whiteboard Friendly",
        description: "Write simple short links like link.bt/homework-may on blackboards or display them on lecture slides."
      },
      {
        title: "Fundraising QR Codes",
        description: "Print trackable QR codes on charity flyers, banners, and brochures directing donors straight to secure contribution pages."
      },
      {
        title: "Free Educational Tools",
        description: "Access LinkBT's core features without costly subscription overheads, saving educational budgets for essential resources."
      }
    ],
    deepDiveSections: [
      {
        heading: "Simplifying Cloud Document Sharing",
        content: "Google Docs and Microsoft 365 links are often over 100 characters long. Shorten them with LinkBT and set an expiration date when assignment submission deadlines close."
      }
    ],
    faqs: [
      {
        question: "Can teachers password-protect exam reviews?",
        answer: "Yes! You can attach a password to your short link and share the key only with enrolled students."
      }
    ]
  }
};

