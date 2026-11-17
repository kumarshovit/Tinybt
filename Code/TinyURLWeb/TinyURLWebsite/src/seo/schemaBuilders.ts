export const SITE_URL = "https://link.bt";
export const SITE_NAME = "LinkBT";

export interface BreadcrumbItem {
  name: string;
  url: string;
}

export interface FAQItem {
  question: string;
  answer: string;
}

/**
 * Builds schema.org Organization JSON-LD
 */
export function buildOrganizationSchema() {
  return {
    "@context": "https://schema.org",
    "@type": "Organization",
    "name": SITE_NAME,
    "url": SITE_URL,
    "logo": `${SITE_URL}/logo.png`,
    "description": "Enterprise-grade URL shortening, dynamic QR code generation, and link analytics platform.",
    "sameAs": [
      "https://twitter.com/linkbt",
      "https://github.com/linkbt"
    ],
    "contactPoint": {
      "@type": "ContactPoint",
      "contactType": "customer support",
      "email": "info@link.bt",
      "url": `${SITE_URL}/contact/`
    }
  };
}

/**
 * Builds schema.org WebSite JSON-LD with PotentialAction SearchAction
 */
export function buildWebSiteSchema() {
  return {
    "@context": "https://schema.org",
    "@type": "WebSite",
    "name": SITE_NAME,
    "url": SITE_URL,
    "potentialAction": {
      "@type": "SearchAction",
      "target": {
        "@type": "EntryPoint",
        "urlTemplate": `${SITE_URL}/glossary/?q={search_term_string}`
      },
      "query-input": "required name=search_term_string"
    }
  };
}

/**
 * Builds schema.org SoftwareApplication JSON-LD for LinkBT tool services
 */
export function buildSoftwareApplicationSchema(name: string, description: string, applicationCategory: string = "UtilitiesApplication") {
  return {
    "@context": "https://schema.org",
    "@type": "SoftwareApplication",
    "name": name,
    "operatingSystem": "All",
    "applicationCategory": applicationCategory,
    "offers": {
      "@type": "Offer",
      "price": "0",
      "priceCurrency": "USD"
    },
    "description": description,
    "url": SITE_URL
  };
}

/**
 * Builds schema.org BreadcrumbList JSON-LD
 */
export function buildBreadcrumbSchema(items: BreadcrumbItem[]) {
  return {
    "@context": "https://schema.org",
    "@type": "BreadcrumbList",
    "itemListElement": items.map((item, index) => ({
      "@type": "ListItem",
      "position": index + 1,
      "name": item.name,
      "item": item.url.startsWith("http") ? item.url : `${SITE_URL}${item.url}`
    }))
  };
}

/**
 * Builds schema.org FAQPage JSON-LD
 */
export function buildFAQSchema(faqs: FAQItem[]) {
  return {
    "@context": "https://schema.org",
    "@type": "FAQPage",
    "mainEntity": faqs.map(faq => ({
      "@type": "Question",
      "name": faq.question,
      "acceptedAnswer": {
        "@type": "Answer",
        "text": faq.answer
      }
    }))
  };
}

/**
 * Builds schema.org Article / TechArticle JSON-LD
 */
export function buildArticleSchema(opts: {
  headline: string;
  description: string;
  url: string;
  datePublished?: string;
  dateModified?: string;
  authorName?: string;
}) {
  return {
    "@context": "https://schema.org",
    "@type": "Article",
    "headline": opts.headline,
    "description": opts.description,
    "url": opts.url.startsWith("http") ? opts.url : `${SITE_URL}${opts.url}`,
    "datePublished": opts.datePublished || "2026-01-01",
    "dateModified": opts.dateModified || "2026-11-15",
    "mainEntityOfPage": {
      "@type": "WebPage",
      "@id": opts.url.startsWith("http") ? opts.url : `${SITE_URL}${opts.url}`
    },
    "publisher": {
      "@type": "Organization",
      "name": SITE_NAME,
      "logo": {
        "@type": "ImageObject",
        "url": `${SITE_URL}/logo.png`
      }
    },
    "author": {
      "@type": "Organization",
      "name": opts.authorName || "LinkBT Security & Engineering Team"
    }
  };
}

/**
 * Builds schema.org DefinedTerm JSON-LD for Glossary entries
 */
export function buildDefinedTermSchema(opts: {
  name: string;
  description: string;
  termCode: string;
  url: string;
}) {
  return {
    "@context": "https://schema.org",
    "@type": "DefinedTerm",
    "name": opts.name,
    "description": opts.description,
    "termCode": opts.termCode,
    "url": opts.url.startsWith("http") ? opts.url : `${SITE_URL}${opts.url}`,
    "inDefinedTermSet": `${SITE_URL}/glossary/`
  };
}

