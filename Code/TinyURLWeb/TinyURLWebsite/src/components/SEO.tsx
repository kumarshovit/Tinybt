import { Helmet } from "react-helmet-async";

const SITE_URL = "https://link.bt";
const SITE_NAME = "LinkBT";
const DEFAULT_IMAGE = `${SITE_URL}/og-image.png`;

interface SEOProps {
  title: string;
  description: string;
  canonical?: string;
  noindex?: boolean;
  ogImage?: string;
  ogType?: "website" | "article";
  jsonLd?: object;
}

export default function SEO({
  title,
  description,
  canonical,
  noindex = false,
  ogImage = DEFAULT_IMAGE,
  ogType = "website",
  jsonLd,
}: SEOProps) {
  const fullTitle = title.includes(SITE_NAME)
    ? title
    : `${title} | ${SITE_NAME}`;

  const canonicalUrl = canonical
    ? `${SITE_URL}${canonical}`
    : undefined;

  return (
    <Helmet>
      {/* Primary */}
      <title>{fullTitle}</title>
      <meta name="description" content={description} />
      <meta name="robots" content={noindex ? "noindex, nofollow" : "index, follow"} />
      {canonicalUrl && <link rel="canonical" href={canonicalUrl} />}

      {/* Open Graph */}
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
      {jsonLd && (
        <script type="application/ld+json">
          {JSON.stringify(jsonLd)}
        </script>
      )}
    </Helmet>
  );
}
