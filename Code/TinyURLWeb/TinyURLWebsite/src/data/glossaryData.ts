import type { SeoPageContent } from "./marketingData";

export interface GlossaryTermContent extends SeoPageContent {
  termCode: string;
  definition: string;
  pronunciation?: string;
}

export const GLOSSARY_PAGES: Record<string, GlossaryTermContent> = {
  "/glossary/url-shortener/": {
    slug: "/glossary/url-shortener/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "URL_SHORTENER",
    definition: "A URL shortener is an online service that translates a long, complex Uniform Resource Locator (URL) into an abbreviated alphanumeric short link, redirecting the user to the destination via HTTP redirect codes.",
    title: "URL Shortener: Definition & Technical Architecture",
    subtitle: "Complete definition of a URL shortener, explaining HTTP 301 redirects, database hash lookups, custom aliases, and link tracking mechanics.",
    introParagraphs: [
      "A URL shortener (also known as a link compressor or redirect generator) is a web application that takes an arbitrary long web address and generates a compact alias pointing to the same resource. When a user requests the short code, the server looks up the corresponding long URL in a database and returns an HTTP redirect response (typically HTTP 301 Moved Permanently).",
      "Modern URL shorteners extend this basic redirection function to provide real-time click telemetry, geographic attribution, custom branded slugs, password security gates, and automated threat scanning."
    ],
    keyBenefits: [
      {
        title: "HTTP 301 Permanent Redirection",
        description: "Informs web browsers and search engine crawlers that the link has moved, preserving search engine link equity."
      },
      {
        title: "Space Efficiency",
        description: "Reduces URLs from 100+ characters down to approximately 20 characters, ideal for SMS and print media."
      },
      {
        title: "Telemetry Capture",
        description: "Measures click velocity, country of origin, referrers, and user-agent classifications."
      }
    ],
    deepDiveSections: [
      {
        heading: "Technical Architecture",
        content: "When a short link is created, the system stores the long URL and assigns a 6-character Base62 identifier. The database index on ShortCode must be strictly unique to prevent collision."
      }
    ],
    faqs: [
      {
        question: "Does a URL shortener change the content of my website?",
        answer: "No. A URL shortener only manages the initial redirect; once the browser reaches the destination website, the page renders exactly as designed."
      }
    ]
  },

  "/glossary/custom-alias/": {
    slug: "/glossary/custom-alias/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "CUSTOM_ALIAS",
    definition: "A custom alias (also called a vanity URL or custom slug) is a user-defined keyword chosen to replace an automatically generated random alphanumeric short code.",
    title: "Custom Alias (Vanity URL): Definition & Guide",
    subtitle: "Definition of a custom alias, explaining vanity slugs, character constraints, collision prevention, and impact on click-through rates.",
    introParagraphs: [
      "By default, URL shorteners generate pseudo-random strings (such as link.bt/a7K8p2). A custom alias allows the user to specify a recognizable keyword, such as link.bt/spring-sale or link.bt/podcast.",
      "Custom aliases communicate contextual information about the target page, reassuring users about where the link will take them and driving higher click engagement."
    ],
    keyBenefits: [
      {
        title: "Increased Click-Through Rate",
        description: "Users are up to 34% more likely to click a link that clearly describes its destination."
      },
      {
        title: "High Memorability",
        description: "Easy to pronounce aloud during presentations, podcasts, and radio advertisements."
      },
      {
        title: "Normalized Slugs",
        description: "Case-insensitive resolution ensures links work whether typed in uppercase, lowercase, or mixed case."
      }
    ],
    deepDiveSections: [
      {
        heading: "Allowed Characters",
        content: "Custom aliases typically support alphanumeric characters [a-z, 0-9] and hyphens (-). Special characters, spaces, and reserved words are prohibited to ensure web safety."
      }
    ],
    faqs: [
      {
        question: "Can two people use the same custom alias?",
        answer: "No. Custom aliases are strictly unique across the entire platform on a first-come, first-served basis."
      }
    ]
  },

  "/glossary/branded-link/": {
    slug: "/glossary/branded-link/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "BRANDED_LINK",
    definition: "A branded link is a shortened URL that incorporates a verified, reputable domain or company brand name to foster trust and brand consistency.",
    title: "Branded Link: Definition, Architecture & Benefits",
    subtitle: "Learn the definition of a branded link, why domain reputation protects deliverability, and how branded URLs improve marketing performance.",
    introParagraphs: [
      "A branded link is a specialized short URL that replaces anonymous, generic shortener domains with a recognized, trusted brand domain name. Branded links serve as a digital signature, signaling to visitors that the content is legitimate and secure.",
      "In corporate marketing, branded links prevent brand dilution, improve email deliverability, and safeguard organizations from being associated with low-reputation public shortener pools."
    ],
    keyBenefits: [
      {
        title: "Brand Consistency",
        description: "Reinforces brand visibility across social bios, SMS alerts, and marketing emails."
      },
      {
        title: "Reduced Phishing Suspicion",
        description: "Visitors trust verified domain brands significantly more than anonymous generic short links."
      },
      {
        title: "Deliverability Protection",
        description: "Avoids spam-filter penalties associated with shared, unmonitored shortener domains."
      }
    ],
    deepDiveSections: [
      {
        heading: "Domain Reputation Mechanics",
        content: "Internet service providers and corporate firewalls evaluate the historical safety of redirect domains. LinkBT preserves clean domain reputation through proactive 3-engine malware quarantine."
      }
    ],
    faqs: [
      {
        question: "How do branded links improve email marketing?",
        answer: "They keep URLs clean and compact, avoiding line wrapping and spam score triggers in major email clients."
      }
    ]
  },

  "/glossary/utm-parameters/": {
    slug: "/glossary/utm-parameters/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "UTM_PARAMETERS",
    definition: "UTM (Urchin Tracking Module) parameters are standardized query string tags appended to destination URLs to allow web analytics tools to track campaign traffic sources.",
    title: "UTM Parameters: Definition & Tagging Breakdown",
    subtitle: "Comprehensive definition of UTM parameters, explaining utm_source, utm_medium, utm_campaign, utm_term, and utm_content.",
    introParagraphs: [
      "UTM parameters were created by Urchin Software Corporation (acquired by Google in 2005) and remain the gold standard for tracking online marketing campaigns. When a link with UTM parameters is clicked, analytics platforms like Google Analytics 4 extract the query values and attribute the session to the designated channel.",
      "The five standard UTM parameters are utm_source, utm_medium, utm_campaign, utm_term, and utm_content. Using these tags consistently prevents campaign data from being lumped into generic referral or direct buckets."
    ],
    keyBenefits: [
      {
        title: "Accurate Traffic Attribution",
        description: "Clearly distinguish between paid ads, email blasts, partner referrals, and organic social posts."
      },
      {
        title: "Standardized Taxonomy",
        description: "Enforces consistent data modeling across diverse marketing departments and agencies."
      },
      {
        title: "Pairing with Short Links",
        description: "Shorten long UTM-tagged URLs with LinkBT to maintain clean aesthetic presentation."
      }
    ],
    deepDiveSections: [
      {
        heading: "Parameter Definitions",
        content: "Source (where traffic originates), Medium (channel type), Campaign (initiative name), Term (paid search keywords), Content (creative or A/B test variation)."
      }
    ],
    faqs: [
      {
        question: "Do UTM parameters affect SEO rankings?",
        answer: "No. Search engines recognize UTM parameters as tracking tags and typically consolidate link equity via rel='canonical' tags."
      }
    ]
  },

  "/glossary/qr-code/": {
    slug: "/glossary/qr-code/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "QR_CODE",
    definition: "A QR code (Quick Response Code) is a two-dimensional matrix barcode consisting of black modules arranged in a square grid on a white background, designed to be read by optical scanners and smartphone cameras.",
    title: "QR Code (Quick Response Code): Definition & Technology",
    subtitle: "Explore the definition of QR codes, explaining two-dimensional matrix encoding, Reed-Solomon error correction, and dynamic vs. static links.",
    introParagraphs: [
      "Invented in 1994 by Masahiro Hara of Denso Wave to track automotive components, the QR code has become the universal standard for bridging offline physical environments to digital web destinations.",
      "Unlike traditional one-dimensional barcodes that hold only 20 digits, a standard QR code can store over 7,000 numeric or 4,000 alphanumeric characters, including complex web URLs, Wi-Fi credentials, and contact cards."
    ],
    keyBenefits: [
      {
        title: "2D Matrix Encoding",
        description: "Encodes data both horizontally and vertically, packing massive data density into compact visual dimensions."
      },
      {
        title: "Reed-Solomon Error Correction",
        description: "Allows QR codes to be successfully scanned even if up to 30% of the symbol is obscured or damaged."
      },
      {
        title: "Dynamic Cloud Routing",
        description: "Pairing QR codes with LinkBT short URLs enables changing the destination target after printing."
      }
    ],
    deepDiveSections: [
      {
        heading: "Finder Patterns & Alignment",
        content: "The three prominent squares in the corners of a QR code allow camera sensors to detect the code's orientation and skew angle in milliseconds."
      }
    ],
    faqs: [
      {
        question: "Can QR codes be customized with colors?",
        answer: "Yes, as long as sufficient contrast is maintained between the dark foreground modules and light background."
      }
    ]
  },

  "/glossary/link-analytics/": {
    slug: "/glossary/link-analytics/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "LINK_ANALYTICS",
    definition: "Link analytics refers to the collection, aggregation, and analysis of quantitative telemetry generated when visitors click on a shortened or tracked hyperlink.",
    title: "Link Analytics: Definition, Measurement & KPIs",
    subtitle: "Definition of link analytics, explaining click tracking metrics, geographic telemetry, user-agent parsing, and referrer attribution.",
    introParagraphs: [
      "Link analytics is the quantitative measurement of how audiences interact with shortened hyperlinks across digital channels. When a short link is requested, the server logs metadata including timestamp, client IP (for geo-IP resolution), HTTP Referer header, and User-Agent string.",
      "These raw data points are aggregated into actionable marketing metrics, providing marketers with insights into click velocity, regional audience concentration, device preference, and campaign ROI."
    ],
    keyBenefits: [
      {
        title: "Total vs. Unique Clicks",
        description: "Distinguish between total redirect volume and unique individual visitors engaged by a campaign."
      },
      {
        title: "Geographic Distribution",
        description: "Map clicks to country and regional markets to evaluate international marketing expansion."
      },
      {
        title: "Device & Browser Breakdown",
        description: "Understand whether audiences engage predominantly via mobile iOS, Android, or desktop browsers."
      }
    ],
    deepDiveSections: [
      {
        heading: "Privacy Standards in Analytics",
        content: "Modern link analytics platforms anonymize IP addresses and avoid third-party cookies, ensuring full compliance with GDPR and CCPA privacy standards."
      }
    ],
    faqs: [
      {
        question: "How is geographic location determined?",
        answer: "Location is resolved via non-intrusive Geo-IP database lookups at the country or city level without tracking exact GPS coordinates."
      }
    ]
  },

  "/glossary/http-301-redirect/": {
    slug: "/glossary/http-301-redirect/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "HTTP_301_REDIRECT",
    definition: "An HTTP 301 Moved Permanently status code is a standard HTTP response sent by a web server indicating that the requested resource has been permanently relocated to the URL specified in the Location header.",
    title: "HTTP 301 Redirect: Definition, Caching & SEO Impact",
    subtitle: "Complete definition of an HTTP 301 redirect, explaining browser caching, Location response headers, and SEO link equity transfer.",
    introParagraphs: [
      "In the HTTP protocol specification, 3xx status codes indicate redirection. Specifically, HTTP 301 Moved Permanently tells user-agents (browsers, crawlers, and bots) that the target resource has permanently relocated to a new address.",
      "URL shorteners utilize HTTP 301 redirects to seamlessly forward incoming requests to the original destination URL while informing search engine crawlers that all link equity (PageRank) should pass through to the destination."
    ],
    keyBenefits: [
      {
        title: "SEO Link Equity Preservation",
        description: "Search engines pass 90-99% of link equity through a 301 redirect, avoiding SEO penalties."
      },
      {
        title: "Browser Caching Support",
        description: "Standard web browsers cache 301 redirects, reducing server round trips on subsequent visits."
      },
      {
        title: "Location Header Standard",
        description: "The new target URL is specified in the standard HTTP Location response header."
      }
    ],
    deepDiveSections: [
      {
        heading: "301 vs 302 Redirection",
        content: "An HTTP 302 redirect indicates a temporary relocation, telling crawlers not to update their index. For permanent URL shorteners, HTTP 301 is the standard choice."
      }
    ],
    faqs: [
      {
        question: "Does an HTTP 301 redirect slow down the user?",
        answer: "No. With LinkBT's edge network, the 301 response is generated in under 50ms, resulting in an imperceptible transition."
      }
    ]
  },

  "/glossary/click-through-rate/": {
    slug: "/glossary/click-through-rate/",
    category: "glossary",
    badge: "Glossary Definition",
    termCode: "CLICK_THROUGH_RATE",
    definition: "Click-Through Rate (CTR) is a digital marketing metric measuring the percentage of people who click on a specific hyperlink or advertisement compared to the total number who viewed it (impressions).",
    title: "Click-Through Rate (CTR): Definition & Link Optimization",
    subtitle: "Understand Click-Through Rate (CTR), its mathematical formula, industry benchmarks, and proven techniques for optimizing short link clicks.",
    introParagraphs: [
      "Click-Through Rate (CTR) is one of the most fundamental performance metrics in digital marketing. Expressed as a percentage, it evaluates the compelling nature of your link or call-to-action by dividing total clicks by total impressions: CTR = (Clicks ÷ Impressions) × 100%.",
      "In link management, factors such as domain trust, custom alias clarity, and visual presentation have a direct, measurable impact on CTR."
    ],
    keyBenefits: [
      {
        title: "Mathematical Formula",
        description: "CTR (%) = (Total Clicks ÷ Total Impressions) × 100."
      },
      {
        title: "Measure Campaign Resonance",
        description: "Higher CTRs indicate that copy, creative, and placement are highly relevant to the targeted audience."
      },
      {
        title: "Vanity Alias Lift",
        description: "Using recognizable, descriptive custom slugs increases link CTR by up to 34% over random character strings."
      }
    ],
    deepDiveSections: [
      {
        heading: "Proven Techniques to Boost Link CTR",
        content: "Use descriptive custom aliases, ensure the short link domain has high reputation, place links prominently before the fold, and test clear call-to-action copy."
      }
    ],
    faqs: [
      {
        question: "What is a good CTR for social media links?",
        answer: "Average CTR across organic social media typically ranges from 1% to 3%, while targeted email campaigns often achieve 3% to 6%."
      }
    ]
  }
};

