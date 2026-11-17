import { Suspense, lazy } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

// Eagerly loaded: core pages visible before login + redirect handler
import LandingPage from "./pages/LandingPage";
import Login from "./pages/Login";
import ShortUrlRedirect from "./pages/ShortUrlRedirect";
import ExpiredLinkPage from "./pages/ExpiredLinkPage";
import ContactPage from "./pages/ContactPage";
import TermsPage from "./pages/TermsPage";
import PrivacyPolicyPage from "./pages/PrivacyPolicyPage";
import AnalyticsTracker from "./components/AnalyticsTracker";
import FaqPage from "./pages/FaqPage";
import NotFoundPage from "./pages/NotFoundPage";
import FeaturesPage from "./pages/FeaturesPage";
import SecurityPage from "./pages/SecurityPage";
import PricingPage from "./pages/PricingPage";
import PasswordProtectedPage from "./pages/PasswordProtectedPage";
import BlockedLinkPage from "./pages/BlockedLinkPage";
import InterstitialPage from "./pages/InterstitialPage";

// Marketing, SEO Hubs & Detail Pages
import AboutPage from "./pages/marketing/AboutPage";
import ReportAbusePage from "./pages/marketing/ReportAbusePage";
import CategoryHubPage from "./pages/marketing/CategoryHubPage";
import SeoDetailPage from "./pages/marketing/SeoDetailPage";
import DocsPage from "./pages/marketing/DocsPage";
import GlossaryTermPage from "./pages/marketing/GlossaryTermPage";

// SEO Interactive Tools
import ToolsHubPage from "./pages/tools/ToolsHubPage";
import UtmBuilderPage from "./pages/tools/UtmBuilderPage";
import WhatsAppLinkGeneratorPage from "./pages/tools/WhatsAppLinkGeneratorPage";
import QrCodeGeneratorToolPage from "./pages/tools/QrCodeGeneratorToolPage";
import BulkShortenerToolPage from "./pages/tools/BulkShortenerToolPage";

// Lazily loaded: authenticated / heavy pages (recharts, react-select, etc.)
const Dashboard = lazy(() => import("./pages/Dashboard"));
const AnalyticsPage = lazy(() => import("./pages/AnalyticsPage"));
const ProfilePage = lazy(() => import("./pages/ProfilePage"));
const TagManagement = lazy(() => import("./pages/TagManagement"));
const AdminPage = lazy(() => import("./pages/AdminPage"));
const DeveloperDashboard = lazy(() => import("./pages/DeveloperDashboard"));

import ProtectedRoute from "./routes/ProtectedRoute";
import PublicOnlyRoute from "./routes/PublicOnlyRoute";
import AdminRoute from "./components/auth/AdminRoute";
import { LoadingProvider } from "./context/LoadingContext";
import GlobalLoadingPanel from "./components/common/GlobalLoadingPanel";
import PageLoadingFallback from "./components/common/PageLoadingFallback";
import ErrorBoundary from "./components/common/ErrorBoundary";

export default function App() {
  return (
    <ErrorBoundary>
      <LoadingProvider>
        <BrowserRouter>
          <AnalyticsTracker />
          <GlobalLoadingPanel />

          <Suspense fallback={<PageLoadingFallback />}>
            <Routes>
              {/* 1. Landing */}
              <Route path="/" element={<LandingPage />} />

              {/* 2. Authentication & Account Access */}
              <Route
                path="/login"
                element={
                  <PublicOnlyRoute>
                    <Login />
                  </PublicOnlyRoute>
                }
              />
              <Route path="/register" element={<Navigate to="/login" replace />} />
              <Route path="/verify-email" element={<Navigate to="/login" replace />} />
              <Route path="/forgot-password" element={<Navigate to="/login" replace />} />
              <Route path="/reset-password" element={<Navigate to="/login" replace />} />

              {/* 3. Core Retained Marketing & Legal Pages */}
              <Route path="/features" element={<FeaturesPage />} />
              <Route path="/features/" element={<FeaturesPage />} />
              <Route path="/pricing" element={<PricingPage />} />
              <Route path="/pricing/" element={<PricingPage />} />
              <Route path="/security" element={<SecurityPage />} />
              <Route path="/security/" element={<SecurityPage />} />
              <Route path="/contact" element={<ContactPage />} />
              <Route path="/contact/" element={<ContactPage />} />
              <Route path="/terms" element={<TermsPage />} />
              <Route path="/terms/" element={<TermsPage />} />
              <Route path="/faq" element={<FaqPage />} />
              <Route path="/faq/" element={<FaqPage />} />
              <Route path="/privacy-policy" element={<PrivacyPolicyPage />} />
              <Route path="/privacy-policy/" element={<PrivacyPolicyPage />} />

              {/* 4. Company & Trust Pages */}
              <Route path="/about" element={<AboutPage />} />
              <Route path="/about/" element={<AboutPage />} />
              <Route path="/report-abuse" element={<ReportAbusePage />} />
              <Route path="/report-abuse/" element={<ReportAbusePage />} />

              {/* 5. Feature Deep-Dives */}
              <Route path="/custom-aliases" element={<SeoDetailPage forcedSlug="/custom-aliases/" />} />
              <Route path="/custom-aliases/" element={<SeoDetailPage forcedSlug="/custom-aliases/" />} />
              <Route path="/branded-links" element={<SeoDetailPage forcedSlug="/branded-links/" />} />
              <Route path="/branded-links/" element={<SeoDetailPage forcedSlug="/branded-links/" />} />
              <Route path="/link-expiration" element={<SeoDetailPage forcedSlug="/link-expiration/" />} />
              <Route path="/link-expiration/" element={<SeoDetailPage forcedSlug="/link-expiration/" />} />
              <Route path="/password-protected-links" element={<SeoDetailPage forcedSlug="/password-protected-links/" />} />
              <Route path="/password-protected-links/" element={<SeoDetailPage forcedSlug="/password-protected-links/" />} />
              <Route path="/link-analytics" element={<SeoDetailPage forcedSlug="/link-analytics/" />} />
              <Route path="/link-analytics/" element={<SeoDetailPage forcedSlug="/link-analytics/" />} />
              <Route path="/bulk-shortener" element={<SeoDetailPage forcedSlug="/bulk-shortener/" />} />
              <Route path="/bulk-shortener/" element={<SeoDetailPage forcedSlug="/bulk-shortener/" />} />
              <Route path="/qr-code-generator" element={<SeoDetailPage forcedSlug="/qr-code-generator/" />} />
              <Route path="/qr-code-generator/" element={<SeoDetailPage forcedSlug="/qr-code-generator/" />} />

              {/* 6. Free Tools Hub & Tool Pages */}
              <Route path="/tools" element={<ToolsHubPage />} />
              <Route path="/tools/" element={<ToolsHubPage />} />
              <Route path="/tools/utm-builder" element={<UtmBuilderPage />} />
              <Route path="/tools/utm-builder/" element={<UtmBuilderPage />} />
              <Route path="/tools/whatsapp-link-generator" element={<WhatsAppLinkGeneratorPage />} />
              <Route path="/tools/whatsapp-link-generator/" element={<WhatsAppLinkGeneratorPage />} />
              <Route path="/tools/qr-code-generator" element={<QrCodeGeneratorToolPage />} />
              <Route path="/tools/qr-code-generator/" element={<QrCodeGeneratorToolPage />} />
              <Route path="/tools/bulk-url-shortener" element={<BulkShortenerToolPage />} />
              <Route path="/tools/bulk-url-shortener/" element={<BulkShortenerToolPage />} />

              {/* 7. Category Hubs & Detail Pages */}
              <Route path="/use-cases" element={<CategoryHubPage category="use-cases" />} />
              <Route path="/use-cases/" element={<CategoryHubPage category="use-cases" />} />
              <Route path="/use-cases/:slug" element={<SeoDetailPage />} />
              <Route path="/use-cases/:slug/" element={<SeoDetailPage />} />

              <Route path="/for" element={<CategoryHubPage category="for" />} />
              <Route path="/for/" element={<CategoryHubPage category="for" />} />
              <Route path="/for/:slug" element={<SeoDetailPage />} />
              <Route path="/for/:slug/" element={<SeoDetailPage />} />

              <Route path="/guides" element={<CategoryHubPage category="guides" />} />
              <Route path="/guides/" element={<CategoryHubPage category="guides" />} />
              <Route path="/guides/:slug" element={<SeoDetailPage />} />
              <Route path="/guides/:slug/" element={<SeoDetailPage />} />

              <Route path="/compare" element={<CategoryHubPage category="compare" />} />
              <Route path="/compare/" element={<CategoryHubPage category="compare" />} />
              <Route path="/compare/:slug" element={<SeoDetailPage />} />
              <Route path="/compare/:slug/" element={<SeoDetailPage />} />

              <Route path="/countries" element={<CategoryHubPage category="countries" />} />
              <Route path="/countries/" element={<CategoryHubPage category="countries" />} />
              <Route path="/countries/:slug" element={<SeoDetailPage />} />
              <Route path="/countries/:slug/" element={<SeoDetailPage />} />

              <Route path="/docs" element={<CategoryHubPage category="docs" />} />
              <Route path="/docs/" element={<CategoryHubPage category="docs" />} />
              <Route path="/docs/:slug" element={<DocsPage />} />
              <Route path="/docs/:slug/" element={<DocsPage />} />

              <Route path="/glossary" element={<CategoryHubPage category="glossary" />} />
              <Route path="/glossary/" element={<CategoryHubPage category="glossary" />} />
              <Route path="/glossary/:slug" element={<GlossaryTermPage />} />
              <Route path="/glossary/:slug/" element={<GlossaryTermPage />} />

              {/* 8. Protected Authenticated App Routes */}
              <Route
                path="/dashboard"
                element={
                  <ProtectedRoute>
                    <Dashboard />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/analytics"
                element={
                  <ProtectedRoute>
                    <AnalyticsPage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/developer"
                element={
                  <ProtectedRoute>
                    <DeveloperDashboard />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/profile"
                element={
                  <ProtectedRoute>
                    <ProfilePage />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/tags/:id"
                element={
                  <ProtectedRoute>
                    <TagManagement />
                  </ProtectedRoute>
                }
              />

              {/* 9. Admin */}
              <Route
                path="/admin"
                element={
                  <AdminRoute>
                    <AdminPage />
                  </AdminRoute>
                }
              />

              {/* 10. Platform Utility Routes */}
              <Route path="/expired-link" element={<ExpiredLinkPage />} />
              <Route path="/blocked-link" element={<BlockedLinkPage />} />
              <Route path="/preview/:shortCode" element={<InterstitialPage />} />
              <Route path="/protected/:shortCode" element={<PasswordProtectedPage />} />

              {/* 11. Short URL Redirect (Must be placed after all defined routes) */}
              <Route path="/:shortCode" element={<ShortUrlRedirect />} />

              {/* 12. Fallback */}
              <Route path="/not-found" element={<NotFoundPage />} />
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
          </Suspense>
        </BrowserRouter>
      </LoadingProvider>
    </ErrorBoundary>
  );
}
