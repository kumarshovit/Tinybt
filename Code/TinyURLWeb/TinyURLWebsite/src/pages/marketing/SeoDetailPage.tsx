import { Link, useLocation } from "react-router-dom";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { MARKETING_PAGES } from "../../data/marketingData";
import type { SeoPageContent } from "../../data/marketingData";
import { USE_CASES_PAGES } from "../../data/useCasesData";
import { AUDIENCE_PAGES } from "../../data/audienceData";
import { GUIDES_PAGES } from "../../data/guidesData";
import { COMPARE_PAGES } from "../../data/compareData";
import { COUNTRIES_PAGES } from "../../data/countriesData";
import { ROUTE_BY_PATH } from "../../seo/routeRegistry";
import { buildBreadcrumbSchema, buildFAQSchema, buildArticleSchema } from "../../seo/schemaBuilders";
import { CheckCircle2, ArrowRight, HelpCircle } from "lucide-react";

const ALL_CONTENT_MAP: Record<string, SeoPageContent> = {
  ...MARKETING_PAGES,
  ...USE_CASES_PAGES,
  ...AUDIENCE_PAGES,
  ...GUIDES_PAGES,
  ...COMPARE_PAGES,
  ...COUNTRIES_PAGES
};

interface SeoDetailPageProps {
  forcedSlug?: string;
}

export default function SeoDetailPage({ forcedSlug }: SeoDetailPageProps) {
  const location = useLocation();
  // Normalize slug to match trailing-slash registry
  let rawPath = forcedSlug || location.pathname;
  if (!rawPath.endsWith("/")) {
    rawPath = `${rawPath}/`;
  }

  const content = ALL_CONTENT_MAP[rawPath];
  const routeMeta = ROUTE_BY_PATH[rawPath];

  if (!content) {
    return (
      <div className="min-h-[60vh] flex flex-col items-center justify-center text-center p-6 bg-slate-900 text-white">
        <h1 className="text-3xl font-bold mb-4">Content Initializing</h1>
        <p className="text-slate-400 mb-6">This section is being synchronized with our SEO content engine.</p>
        <Link to="/" className="px-6 py-2.5 rounded-xl bg-blue-600 text-white font-semibold">
          Return Home
        </Link>
      </div>
    );
  }

  const categoryLabel = content.category
    .split("-")
    .map(w => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ");

  const categoryHref = `/${content.category}/`;

  const breadcrumbs = [
    { label: categoryLabel, href: categoryHref },
    { label: content.badge || content.title, href: content.slug }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: categoryLabel, url: categoryHref },
      { name: content.title, url: content.slug }
    ]),
    buildArticleSchema({
      headline: content.title,
      description: content.subtitle,
      url: content.slug
    }),
    buildFAQSchema(content.faqs)
  ];

  return (
    <MarketingLayout
      seo={{
        title: routeMeta?.title || content.title,
        description: routeMeta?.description || content.subtitle,
        canonical: content.slug,
        keywords: routeMeta?.keywords,
        ogType: "article",
        jsonLd
      }}
      badge={content.badge}
      breadcrumbs={breadcrumbs}
      title={content.title}
      subtitle={content.subtitle}
    >
      <div className="space-y-12">
        {/* Intro Paragraphs */}
        <section className="space-y-4 max-w-3xl">
          {content.introParagraphs.map((para, idx) => (
            <p key={idx} className="text-slate-300 text-base sm:text-lg leading-relaxed">
              {para}
            </p>
          ))}
        </section>

        {/* Key Benefits Grid */}
        <section className="space-y-6">
          <h2 className="text-2xl font-bold text-white">Key Capabilities & Strategic Advantages</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {content.keyBenefits.map((benefit, idx) => (
              <div key={idx} className="p-6 rounded-2xl bg-slate-800/80 border border-slate-700/80 shadow-lg space-y-3">
                <div className="w-9 h-9 rounded-xl bg-blue-500/10 text-blue-400 flex items-center justify-center">
                  <CheckCircle2 className="w-5 h-5 text-emerald-400" />
                </div>
                <h3 className="font-bold text-white text-base">{benefit.title}</h3>
                <p className="text-xs sm:text-sm text-slate-300 leading-relaxed">{benefit.description}</p>
              </div>
            ))}
          </div>
        </section>

        {/* Deep Dive Sections */}
        {content.deepDiveSections.map((sec, idx) => (
          <section key={idx} className="p-8 rounded-2xl bg-slate-800/50 border border-slate-700/50 space-y-4 max-w-3xl">
            <h2 className="text-xl sm:text-2xl font-bold text-white">{sec.heading}</h2>
            <div className="text-slate-300 text-sm sm:text-base leading-relaxed whitespace-pre-line font-normal">
              {sec.content}
            </div>
          </section>
        ))}

        {/* FAQs Accordion */}
        {content.faqs && content.faqs.length > 0 && (
          <section className="space-y-6 max-w-3xl pt-8 border-t border-slate-800">
            <h2 className="text-2xl font-bold text-white flex items-center gap-2">
              <HelpCircle className="w-6 h-6 text-blue-400" />
              <span>Frequently Asked Questions</span>
            </h2>
            <div className="space-y-4">
              {content.faqs.map((faq, idx) => (
                <div key={idx} className="p-5 rounded-xl bg-slate-800/60 border border-slate-700/60 space-y-2">
                  <h3 className="font-semibold text-white text-base">{faq.question}</h3>
                  <p className="text-slate-300 text-sm leading-relaxed">{faq.answer}</p>
                </div>
              ))}
            </div>
          </section>
        )}

        {/* Related Category Links */}
        <section className="pt-8 border-t border-slate-800">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-bold text-white">Explore More in {categoryLabel}</h3>
            <Link to={categoryHref} className="text-xs text-blue-400 hover:text-blue-300 flex items-center gap-1">
              <span>View all</span>
              <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
            {Object.values(ALL_CONTENT_MAP)
              .filter(p => p.category === content.category && p.slug !== content.slug)
              .slice(0, 3)
              .map(related => (
                <Link
                  key={related.slug}
                  to={related.slug}
                  className="p-4 rounded-xl bg-slate-800/40 hover:bg-slate-800 border border-slate-700/40 hover:border-slate-600 transition-colors block"
                >
                  <span className="text-xs text-blue-400 font-semibold block mb-1">{related.badge}</span>
                  <span className="text-sm font-bold text-white hover:text-blue-300 transition-colors line-clamp-1 block">
                    {related.title}
                  </span>
                </Link>
              ))}
          </div>
        </section>
      </div>
    </MarketingLayout>
  );
}

