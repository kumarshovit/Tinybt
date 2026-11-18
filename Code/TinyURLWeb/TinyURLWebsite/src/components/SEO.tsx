import { Helmet } from "react-helmet-async";

const SITE_URL = "https://link.bt";
const SITE_NAME = "LinkBT";
const DEFAULT_IMAGE = `${SITE_URL}/og-image.png`;

export interface SEOProps {
  title: string;
  description: string;
  canonical?: string;
  noindex?: boolean;
  nofollow?: boolean;
  ogImage?: string;
  ogType?: "website" | "article";
  keywords?: string;
  jsonLd?: object | object[];
}

export default function SEO({
  title,
  description,
  canonical,
  noindex = false,
  nofollow = false,
  ogImage = DEFAULT_IMAGE,
  ogType = "website",
  keywords,
  jsonLd,
}: SEOProps) {
  const fullTitle = title.includes(SITE_NAME)
    ? title
    : `${title} | ${SITE_NAME}`;

  // Normalize canonical URL
  let canonicalUrl: string | undefined = undefined;
  if (canonical) {
    let cleanPath = canonical;
    if (cleanPath.startsWith(SITE_URL)) {
      cleanPath = cleanPath.slice(SITE_URL.length);
    }
    if (!cleanPath.startsWith("/")) {
      cleanPath = `/${cleanPath}`;
    }
    // Trailing slash normalization: if not root, not login, and not a file extension, ensure trailing slash
    if (cleanPath !== "/" && cleanPath !== "/login" && !cleanPath.endsWith("/") && !cleanPath.includes(".")) {
      cleanPath = `${cleanPath}/`;
    }
    canonicalUrl = `${SITE_URL}${cleanPath}`;
  }

  // Determine robots directive
  let robotsDirective = "index, follow";
  if (noindex && nofollow) {
    robotsDirective = "noindex, nofollow";
  } else if (noindex) {
    robotsDirective = "noindex, follow";
  } else if (nofollow) {
    robotsDirective = "index, nofollow";
  }

  const jsonLdArray = Array.isArray(jsonLd) ? jsonLd : jsonLd ? [jsonLd] : [];

  return (
    <Helmet>
      {/* Primary Meta Tags */}
      <title>{fullTitle}</title>
      <meta name="description" content={description} />
      <meta name="robots" content={robotsDirective} />
      {keywords && <meta name="keywords" content={keywords} />}
      {canonicalUrl && <link rel="canonical" href={canonicalUrl} />}
      {canonicalUrl && <link rel="alternate" hrefLang="en" href={canonicalUrl} />}
      {canonicalUrl && <link rel="alternate" hrefLang="x-default" href={canonicalUrl} />}

      {/* Open Graph / Facebook */}
      <meta property="og:type" content={ogType} />
      <meta property="og:site_name" content={SITE_NAME} />
      <meta property="og:title" content={fullTitle} />
      <meta property="og:description" content={description} />
      {canonicalUrl && <meta property="og:url" content={canonicalUrl} />}
      <meta property="og:image" content={ogImage} />
      <meta property="og:image:width" content="1200" />
      <meta property="og:image:height" content="630" />

      {/* Twitter Card */}
      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:title" content={fullTitle} />
      <meta name="twitter:description" content={description} />
      <meta name="twitter:image" content={ogImage} />

      {/* JSON-LD Structured Data */}
      {jsonLdArray.map((schema, index) => (
        <script key={index} type="application/ld+json">
          {JSON.stringify(schema)}
        </script>
      ))}
    </Helmet>
  );
}
