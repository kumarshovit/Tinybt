import React from "react";
import { Link } from "react-router-dom";
import Navbar from "../Navbar";
import Footer from "../Footer";
import SEO from "../SEO";
import type { SEOProps } from "../SEO";
import { ArrowRight, CheckCircle2, ChevronRight, ShieldCheck, Zap } from "lucide-react";

export interface BreadcrumbItem {
  label: string;
  href: string;
}

interface MarketingLayoutProps {
  seo: SEOProps;
  badge?: string;
  breadcrumbs?: BreadcrumbItem[];
  title: string;
  subtitle: string;
  children: React.ReactNode;
  showCta?: boolean;
}

export default function MarketingLayout({
  seo,
  badge = "Enterprise Link Management",
  breadcrumbs,
  title,
  subtitle,
  children,
  showCta = true,
}: MarketingLayoutProps) {
  return (
    <div className="min-h-screen bg-slate-900 text-slate-100 flex flex-col font-sans selection:bg-blue-600 selection:text-white">
      <SEO {...seo} />
      <Navbar />

      {/* Hero Header */}
      <header className="relative pt-24 pb-16 md:pt-32 md:pb-20 bg-gradient-to-b from-slate-950 via-slate-900 to-slate-900 border-b border-slate-800">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,_var(--tw-gradient-stops))] from-blue-900/20 via-transparent to-transparent pointer-events-none" />

        <div className="max-w-6xl mx-auto px-4 sm:px-6 relative z-10">
          {/* Breadcrumbs */}
          {breadcrumbs && breadcrumbs.length > 0 && (
            <nav aria-label="Breadcrumb" className="mb-6 flex items-center gap-2 text-xs sm:text-sm text-slate-400">
              <Link to="/" className="hover:text-blue-400 transition-colors">Home</Link>
              {breadcrumbs.map((crumb, idx) => (
                <React.Fragment key={crumb.href || idx}>
                  <ChevronRight className="w-3.5 h-3.5 text-slate-600" />
                  {idx === breadcrumbs.length - 1 ? (
                    <span className="text-slate-200 font-medium">{crumb.label}</span>
                  ) : (
                    <Link to={crumb.href} className="hover:text-blue-400 transition-colors">
                      {crumb.label}
                    </Link>
                  )}
                </React.Fragment>
              ))}
            </nav>
          )}

          {/* Badge */}
          {badge && (
            <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full text-xs font-semibold bg-blue-500/10 text-blue-400 border border-blue-500/20 mb-4">
              <ShieldCheck className="w-3.5 h-3.5" />
              <span>{badge}</span>
            </div>
          )}

          <h1 className="text-3xl sm:text-4xl md:text-5xl font-extrabold tracking-tight text-white mb-5 leading-tight">
            {title}
          </h1>

          <p className="text-lg sm:text-xl text-slate-300 max-w-3xl leading-relaxed">
            {subtitle}
          </p>
        </div>
      </header>

      {/* Main Content Body */}
      <main className="flex-1 max-w-6xl w-full mx-auto px-4 sm:px-6 py-12">
        {children}

        {/* Global Conversion CTA */}
        {showCta && (
          <section className="mt-20 p-8 sm:p-12 rounded-2xl bg-gradient-to-r from-blue-600 to-indigo-700 text-white shadow-2xl relative overflow-hidden">
            <div className="absolute -right-10 -bottom-10 w-64 h-64 bg-white/10 rounded-full blur-2xl pointer-events-none" />
            <div className="relative z-10 max-w-2xl">
              <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-white/20 text-white text-xs font-semibold mb-4">
                <Zap className="w-3.5 h-3.5" />
                <span>Instant Setup • No Credit Card Required</span>
              </div>
              <h2 className="text-2xl sm:text-3xl font-bold mb-4">
                Ready to Shorten, Secure & Track Your Links?
              </h2>
              <p className="text-blue-100 text-base sm:text-lg mb-8 leading-relaxed">
                Join thousands of creators, developers, and businesses using LinkBT for fast, encrypted, and trackable links with 3-engine malware protection.
              </p>
              <div className="flex flex-wrap items-center gap-4">
                <Link
                  to="/login"
                  className="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-white text-blue-600 font-bold hover:bg-blue-50 transition-all shadow-lg hover:shadow-xl active:scale-[0.98]"
                >
                  <span>Get Started for Free</span>
                  <ArrowRight className="w-4 h-4" />
                </Link>
                <Link
                  to="/features/"
                  className="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-white/10 hover:bg-white/20 text-white font-medium transition-colors border border-white/20"
                >
                  <span>Explore All Features</span>
                </Link>
              </div>
              <div className="mt-6 flex flex-wrap items-center gap-6 text-xs text-blue-100">
                <span className="flex items-center gap-1.5"><CheckCircle2 className="w-4 h-4 text-emerald-300" /> Free Custom Aliases</span>
                <span className="flex items-center gap-1.5"><CheckCircle2 className="w-4 h-4 text-emerald-300" /> Scannable QR Codes</span>
                <span className="flex items-center gap-1.5"><CheckCircle2 className="w-4 h-4 text-emerald-300" /> Multi-Layer Threat Scanning</span>
              </div>
            </div>
          </section>
        )}
      </main>

      <Footer />
    </div>
  );
}

