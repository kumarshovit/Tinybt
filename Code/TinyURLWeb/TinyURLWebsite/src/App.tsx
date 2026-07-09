import { Suspense, lazy } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

// Eagerly loaded: pages visible before login + redirect handler
import LandingPage from "./pages/LandingPage";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ShortUrlRedirect from "./pages/ShortUrlRedirect";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import VerifyEmail from "./pages/Verify";
import ExpiredLinkPage from "./pages/ExpiredLinkPage";
import ContactPage from "./pages/ContactPage";
import TermsPage from "./pages/TermsPage";
import PrivacyPolicyPage from "./pages/PrivacyPolicyPage";
import AnalyticsTracker from "./components/AnalyticsTracker";
import FaqPage from "./pages/FaqPage";
import NotFoundPage from "./pages/NotFoundPage";

// Lazily loaded: authenticated / heavy pages (recharts, react-select, etc.)
const Dashboard = lazy(() => import("./pages/Dashboard"));
const AnalyticsPage = lazy(() => import("./pages/AnalyticsPage"));
const ProfilePage = lazy(() => import("./pages/ProfilePage"));
const TagManagement = lazy(() => import("./pages/TagManagement"));
const AdminPage = lazy(() => import("./pages/AdminPage"));

import ProtectedRoute from "./routes/ProtectedRoute";
import AdminRoute from "./components/auth/AdminRoute";

export default function App() {

  const token = localStorage.getItem("token");

  return (
    <BrowserRouter>

      <AnalyticsTracker />

      <Suspense fallback={<div className="min-h-screen bg-gray-50" />}>
        <Routes>

          {/* Landing */}
          <Route path="/" element={<LandingPage />} />

          {/* Auth Pages */}
          <Route
            path="/login"
            element={token ? <Navigate to="/dashboard" /> : <Login />}
          />

          <Route
            path="/register"
            element={token ? <Navigate to="/dashboard" /> : <Register />}
          />

          <Route path="/verify-email" element={<VerifyEmail />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />

          <Route
            path="/contact"
            element={<ContactPage />}
          />
          <Route
            path="/expired-link"
            element={<ExpiredLinkPage />}
          />
          <Route
            path="/terms"
            element={<TermsPage />}
          />
          <Route path="/faq" element={<FaqPage />} />
          <Route
            path="/privacy-policy"
            element={<PrivacyPolicyPage />}
          />

          {/* Protected Routes */}
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

          {/* Admin */}
          <Route
            path="/admin"
            element={
              <AdminRoute>
                <AdminPage />
              </AdminRoute>
            }
          />

          <Route path="/:shortCode" element={<ShortUrlRedirect />} />


          {/* Fallback */}
          <Route path="/not-found" element={<NotFoundPage />} />
          <Route path="*" element={<NotFoundPage />} />


        </Routes>
      </Suspense>

    </BrowserRouter>
  );
}