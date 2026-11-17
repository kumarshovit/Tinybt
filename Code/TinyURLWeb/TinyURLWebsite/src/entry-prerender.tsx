import React, { StrictMode } from "react";
import { renderToString } from "react-dom/server";
import { HelmetProvider } from "react-helmet-async";
import { MemoryRouter } from "react-router-dom";

// Core Pages
import LandingPage from "./pages/LandingPage";
import ContactPage from "./pages/ContactPage";
import TermsPage from "./pages/TermsPage";
import PrivacyPolicyPage from "./pages/PrivacyPolicyPage";
import FaqPage from "./pages/FaqPage";
import FeaturesPage from "./pages/FeaturesPage";
import PricingPage from "./pages/PricingPage";
import SecurityPage from "./pages/SecurityPage";
import Login from "./pages/Login";

// Marketing & Hub Pages
import AboutPage from "./pages/marketing/AboutPage";
import ReportAbusePage from "./pages/marketing/ReportAbusePage";
import CategoryHubPage from "./pages/marketing/CategoryHubPage";
import SeoDetailPage from "./pages/marketing/SeoDetailPage";
import DocsPage from "./pages/marketing/DocsPage";
import GlossaryTermPage from "./pages/marketing/GlossaryTermPage";

// Tools
import ToolsHubPage from "./pages/tools/ToolsHubPage";
import UtmBuilderPage from "./pages/tools/UtmBuilderPage";
import WhatsAppLinkGeneratorPage from "./pages/tools/WhatsAppLinkGeneratorPage";
import QrCodeGeneratorToolPage from "./pages/tools/QrCodeGeneratorToolPage";
import BulkShortenerToolPage from "./pages/tools/BulkShortenerToolPage";

export { getPrerenderRoutes, getSitemapRoutes } from "./seo/routeRegistry";

(HelmetProvider as any).canUseDOM = false;

function resolveComponent(url: string): React.ReactElement {
  // Normalize trailing slash (except for / and /login)
  const normalized = url === "/" || url === "/login"
    ? url
    : url.endsWith("/")
    ? url
    : `${url}/`;

  switch (normalized) {
    case "/":
      return <LandingPage />;
    case "/features/":
      return <FeaturesPage />;
    case "/pricing/":
      return <PricingPage />;
    case "/security/":
      return <SecurityPage />;
    case "/contact/":
      return <ContactPage />;
    case "/terms/":
      return <TermsPage />;
    case "/privacy-policy/":
      return <PrivacyPolicyPage />;
    case "/faq/":
      return <FaqPage />;
    case "/login":
      return <Login />;
    case "/about/":
      return <AboutPage />;
    case "/report-abuse/":
      return <ReportAbusePage />;

    // Tools
    case "/tools/":
      return <ToolsHubPage />;
    case "/tools/utm-builder/":
      return <UtmBuilderPage />;
    case "/tools/whatsapp-link-generator/":
      return <WhatsAppLinkGeneratorPage />;
    case "/tools/qr-code-generator/":
      return <QrCodeGeneratorToolPage />;
    case "/tools/bulk-url-shortener/":
      return <BulkShortenerToolPage />;

    // Category Hubs
    case "/use-cases/":
      return <CategoryHubPage category="use-cases" />;
    case "/for/":
      return <CategoryHubPage category="for" />;
    case "/guides/":
      return <CategoryHubPage category="guides" />;
    case "/compare/":
      return <CategoryHubPage category="compare" />;
    case "/countries/":
      return <CategoryHubPage category="countries" />;
    case "/docs/":
      return <CategoryHubPage category="docs" />;
    case "/glossary/":
      return <CategoryHubPage category="glossary" />;

    // Feature deep dives
    case "/custom-aliases/":
    case "/branded-links/":
    case "/link-expiration/":
    case "/password-protected-links/":
    case "/link-analytics/":
    case "/bulk-shortener/":
    case "/qr-code-generator/":
      return <SeoDetailPage forcedSlug={normalized} />;

    default:
      if (normalized.startsWith("/docs/")) {
        return <DocsPage forcedSlug={normalized} />;
      }
      if (normalized.startsWith("/glossary/")) {
        return <GlossaryTermPage forcedSlug={normalized} />;
      }
      if (
        normalized.startsWith("/use-cases/") ||
        normalized.startsWith("/for/") ||
        normalized.startsWith("/guides/") ||
        normalized.startsWith("/compare/") ||
        normalized.startsWith("/countries/")
      ) {
        return <SeoDetailPage forcedSlug={normalized} />;
      }
      return <LandingPage />;
  }
}

export function render(url: string) {
  const helmetContext: any = {};
  const element = resolveComponent(url);

  const html = renderToString(
    <StrictMode>
      <HelmetProvider context={helmetContext}>
        <MemoryRouter initialEntries={[url]}>
          {element}
        </MemoryRouter>
      </HelmetProvider>
    </StrictMode>
  );

  return {
    html,
    head: [
      helmetContext.helmet?.title?.toString(),
      helmetContext.helmet?.meta?.toString(),
      helmetContext.helmet?.link?.toString(),
      helmetContext.helmet?.script?.toString()
    ]
      .filter(Boolean)
      .join("\n")
  };
}
