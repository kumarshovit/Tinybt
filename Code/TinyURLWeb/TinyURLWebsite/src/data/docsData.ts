import type { SeoPageContent } from "./marketingData";

export const DOCS_PAGES: Record<string, SeoPageContent> = {
  "/docs/api-overview/": {
    slug: "/docs/api-overview/",
    category: "docs",
    badge: "API Architecture",
    title: "API Architecture & Technical Overview",
    subtitle: "Learn about LinkBT REST API architecture, base endpoints, JSON request/response conventions, and global server response headers.",
    introParagraphs: [
      "The LinkBT REST API provides programmatic access to our core URL shortening engine, custom alias validation, dynamic QR generation, and link analytics telemetry. It is designed around REST principles with predictable resource URLs and standard HTTP response codes.",
      "All API requests must be transmitted over secure HTTPS. Non-HTTPS requests are automatically rejected to protect credentials and data integrity."
    ],
    keyBenefits: [
      {
        title: "Base URL & Versioning",
        description: "The primary base URL for all developer API interactions is https://link.bt/api/developer. Future revisions will adhere to semver standards."
      },
      {
        title: "JSON Request & Response Bodies",
        description: "All POST and PUT requests expect a Content-Type: application/json header and return structured JSON payloads."
      },
      {
        title: "Standard Timestamp Formatting",
        description: "All dates and timestamps are formatted in ISO 8601 UTC (e.g. 2026-11-15T22:00:00Z)."
      }
    ],
    deepDiveSections: [
      {
        heading: "Global Response Headers",
        content: "Every API response includes diagnostic headers including X-RateLimit-Limit, X-RateLimit-Remaining, and X-Request-ID for tracing requests."
      }
    ],
    faqs: [
      {
        question: "Is there a sandbox environment for testing?",
        answer: "You can test endpoints in our staging environment or use test API keys in development mode."
      }
    ]
  },

  "/docs/authentication/": {
    slug: "/docs/authentication/",
    category: "docs",
    badge: "Security & Auth",
    title: "Authenticating API Requests with Bearer Keys",
    subtitle: "Authenticate API requests securely using LinkBT API keys. Review authorization header formats, token generation, and security recommendations.",
    introParagraphs: [
      "Access to the LinkBT developer API is authenticated via secret API keys. Each API key is associated with your LinkBT developer account and carries your account's rate limit quotas and permissions.",
      "API keys must be treated with the same confidentiality as account passwords. Never commit API keys to public Git repositories or expose them in client-side JavaScript."
    ],
    keyBenefits: [
      {
        title: "Authorization Header Standard",
        description: "Pass your API key in the standard HTTP Authorization header: Authorization: Bearer <your_api_key>."
      },
      {
        title: "Instant Key Revocation",
        description: "If an API key is accidentally exposed, you can immediately revoke and regenerate it from your Developer Dashboard."
      },
      {
        title: "Custom Key Descriptions",
        description: "Assign descriptive labels to keys (e.g. 'Production Billing Server' or 'Staging Test Runner') for clear audit logging."
      }
    ],
    deepDiveSections: [
      {
        heading: "Best Practices for API Key Management",
        content: "Store your API keys in environment variables (e.g. LINKBT_API_KEY) or secure key vaults. Rotate keys every 90 days and use distinct keys for development and production workloads."
      }
    ],
    faqs: [
      {
        question: "What happens if a request lacks an API key?",
        answer: "The server returns an HTTP 401 Unauthorized status with a message prompting for valid credentials."
      }
    ]
  },

  "/docs/create-short-link/": {
    slug: "/docs/create-short-link/",
    category: "docs",
    badge: "Endpoints Reference",
    title: "Programmatic Link Creation Endpoint Reference",
    subtitle: "Endpoint reference for programmatically creating shortened URLs, custom aliases, expiration dates, and password protection via the LinkBT API.",
    introParagraphs: [
      "The primary endpoint for creating short links is POST /api/developer/urls. This endpoint validates the destination URL against our 3-engine security scanner, checks custom alias availability, and generates a Base62 short code in under 50ms.",
      "Review the request schema, parameter types, and example responses below to integrate LinkBT into your codebase."
    ],
    keyBenefits: [
      {
        title: "HTTP Method & URL",
        description: "POST https://link.bt/api/developer/urls"
      },
      {
        title: "Optional Custom Alias Parameter",
        description: "Pass customAlias to request a vanity slug. If omitted, LinkBT generates an optimal 6-character Base62 code."
      },
      {
        title: "Optional Expiration & Password",
        description: "Pass expirationDate (ISO 8601) or password to enforce automated access gates on the link."
      }
    ],
    deepDiveSections: [
      {
        heading: "Sample Request Payload",
        content: "{\n  \"longUrl\": \"https://example.com/product-launch\",\n  \"customAlias\": \"product-v2\",\n  \"expirationDate\": \"2026-12-31T23:59:59Z\"\n}"
      },
      {
        heading: "Sample Successful Response (200 OK)",
        content: "{\n  \"success\": true,\n  \"id\": 1042,\n  \"shortCode\": \"product-v2\",\n  \"shortUrl\": \"https://link.bt/product-v2\",\n  \"longUrl\": \"https://example.com/product-launch\",\n  \"createdAt\": \"2026-11-15T22:30:00Z\"\n}"
      }
    ],
    faqs: [
      {
        question: "What error is returned if the alias is taken?",
        answer: "If the requested custom alias is already reserved or occupied, the endpoint returns HTTP 400 Bad Request with message: 'Alias already exists.' or 'This alias is reserved and cannot be used.'"
      }
    ]
  },

  "/docs/rate-limits/": {
    slug: "/docs/rate-limits/",
    category: "docs",
    badge: "System Governance",
    title: "Rate Limits, Request Throttling & Backoff Handling",
    subtitle: "Understand LinkBT API rate limits, hourly burst policies, HTTP 429 Too Many Requests responses, and exponential backoff retry strategies.",
    introParagraphs: [
      "To ensure high availability, prevent distributed denial-of-service attempts, and protect system resources for all developers, LinkBT enforces token-bucket rate limiting on all API endpoints.",
      "When rate limits are exceeded, the API responds with standard HTTP 429 Too Many Requests status codes alongside header metadata indicating when your client may retry."
    ],
    keyBenefits: [
      {
        title: "Standard Rate Limit Headers",
        description: "Every response includes X-RateLimit-Limit (max requests allowed per window) and X-RateLimit-Remaining."
      },
      {
        title: "Retry-After Header",
        description: "When an HTTP 429 is received, inspect the Retry-After response header, which specifies the number of seconds to sleep before retrying."
      },
      {
        title: "Exponential Backoff Recommendation",
        description: "Implement exponential backoff with full jitter in your client libraries to gracefully recover from temporary concurrency spikes."
      }
    ],
    deepDiveSections: [
      {
        heading: "Client-Side Backoff Implementation",
        content: "When building high-volume applications, catch HTTP 429 responses and wait for Math.pow(2, attempt) * 1000 ms before executing a bounded retry loop."
      }
    ],
    faqs: [
      {
        question: "Can rate limits be increased for enterprise needs?",
        answer: "Yes, enterprise customers can request custom burst quotas and dedicated throughput allocations."
      }
    ]
  },

  "/docs/error-handling/": {
    slug: "/docs/error-handling/",
    category: "docs",
    badge: "API Best Practices",
    title: "Standard Error Response Format & HTTP Status Codes",
    subtitle: "Review LinkBT API status codes, error payload schemas, validation failure formats, and best practices for resilient client-side error handling.",
    introParagraphs: [
      "Predictable error handling is the hallmark of a developer-friendly API. LinkBT uses standard HTTP 1.1 status codes to indicate the outcome of every request, paired with clear JSON error payloads explaining the root cause.",
      "Whether a request failed due to missing authorization, invalid URL syntax, or a threat scanner block, our structured responses make programmatic debugging straightforward."
    ],
    keyBenefits: [
      {
        title: "400 Bad Request",
        description: "Returned for malformed payloads, invalid URL formats, or occupied/reserved custom aliases."
      },
      {
        title: "401 Unauthorized",
        description: "Returned when an API key is missing, invalid, or expired."
      },
      {
        title: "422 Unprocessable Entity",
        description: "Returned when the destination URL fails safety inspection by our 3-engine malware scanner."
      }
    ],
    deepDiveSections: [
      {
        heading: "Consistent Error Payload Structure",
        content: "{\n  \"success\": false,\n  \"message\": \"The destination URL has been identified as unsafe by threat intelligence.\",\n  \"errorCode\": \"URL_SAFETY_VIOLATION\"\n}"
      }
    ],
    faqs: [
      {
        question: "How do I handle threat scanner rejections?",
        answer: "Check the 'message' field in the 400 or 422 error response. If you believe a clean domain was falsely flagged, contact our security team at info@link.bt."
      }
    ]
  }
};

