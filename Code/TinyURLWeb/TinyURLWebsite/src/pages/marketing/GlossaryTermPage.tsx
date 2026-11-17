import { Link, useLocation } from "react-router-dom";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { GLOSSARY_PAGES } from "../../data/glossaryData";
import { ROUTE_BY_PATH } from "../../seo/routeRegistry";
import { buildBreadcrumbSchema, buildFAQSchema, buildDefinedTermSchema } from "../../seo/schemaBuilders";
import { BookOpen, CheckCircle2, ChevronRight, HelpCircle } from "lucide-react";

interface GlossaryTermPageProps {
  forcedSlug?: string;
}

export default function GlossaryTermPage({ forcedSlug }: GlossaryTermPageProps) {
  const location = useLocation();
  let rawPath = forcedSlug || location.pathname;
  if (!rawPath.endsWith("/")) {
    rawPath = `${rawPath}/`;
  }

  const term = GLOSSARY_PAGES[rawPath] || GLOSSARY_PAGES["/glossary/url-shortener/"];
  const routeMeta = ROUTE_BY_PATH[rawPath];

  const canonicalUrl = `https://link.bt${term.slug}`;

  const breadcrumbs = [
    { label: "Glossary", href: "/glossary/" },
    { label: term.title.split(":")[0], href: term.slug }
  ];

  const otherTerms = Object.values(GLOSSARY_PAGES)
    .filter(t => t.slug !== term.slug)
    .slice(0, 5);

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Link Glossary", url: "/glossary/" },
      { name: term.title, url: term.slug }
    ]),
    buildDefinedTermSchema({
      name: term.title.split(":")[0].trim(),
      description: term.definition,
      termCode: term.termCode,
      url: canonicalUrl
    }),
    buildFAQSchema(term.faqs)
  ];

  return (
    <MarketingLayout
      seo={{
        title: routeMeta?.title || term.title,
        description: routeMeta?.description || term.subtitle,
        canonical: term.slug,
        keywords: routeMeta?.keywords,
        ogType: "article",
        jsonLd
      }}
      badge={term.badge}
      breadcrumbs={breadcrumbs}
      title={term.title}
      subtitle={term.subtitle}
    >
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
        {/* Main Content */}
        <div className="lg:col-span-3 space-y-10">
          {/* Definition Callout Box */}
          <section className="p-6 rounded-2xl bg-blue-950/40 border border-blue-500/30 shadow-lg">
            <div className="flex items-center gap-2 text-xs font-semibold uppercase tracking-wider text-blue-400 mb-2">
              <BookOpen className="w-4 h-4" />
              <span>Official Definition</span>
              {term.pronunciation && (
                <span className="text-slate-400 normal-case font-mono ml-2">[{term.pronunciation}]</span>
              )}
            </div>
            <p className="text-white text-lg sm:text-xl font-medium leading-relaxed">
              {term.definition}
            </p>
          </section>

          {/* Intro Paragraphs */}
          <section className="space-y-4">
            {term.introParagraphs.map((para, idx) => (
              <p key={idx} className="text-slate-300 text-base sm:text-lg leading-relaxed">
                {para}
              </p>
            ))}
          </section>

          {/* Key Characteristics */}
          <section className="space-y-4">
            <h2 className="text-xl sm:text-2xl font-bold text-white">
              Key Properties & Significance
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {term.keyBenefits.map((item, idx) => (
                <div
                  key={idx}
                  className="p-5 rounded-2xl bg-slate-800/40 border border-slate-700/50 hover:border-blue-500/30 transition-colors"
                >
                  <div className="flex items-center gap-2 text-emerald-400 mb-2">
                    <CheckCircle2 className="w-5 h-5 shrink-0" />
                    <h3 className="font-semibold text-white text-base">{item.title}</h3>
                  </div>
                  <p className="text-sm text-slate-300 leading-relaxed">{item.description}</p>
                </div>
              ))}
            </div>
          </section>

          {/* Deep-Dive Sections */}
          {term.deepDiveSections.map((sec, idx) => (
            <section key={idx} className="space-y-3">
              <h2 className="text-xl sm:text-2xl font-bold text-white">{sec.heading}</h2>
              <p className="text-slate-300 text-base leading-relaxed">{sec.content}</p>
            </section>
          ))}

          {/* FAQ */}
          {term.faqs.length > 0 && (
            <section className="space-y-4 pt-6 border-t border-slate-800">
              <h2 className="text-xl sm:text-2xl font-bold text-white flex items-center gap-2">
                <HelpCircle className="w-6 h-6 text-blue-400" />
                Frequently Asked Questions
              </h2>
              <div className="space-y-3">
                {term.faqs.map((faq, idx) => (
                  <div key={idx} className="p-5 rounded-xl bg-slate-800/40 border border-slate-700/50">
                    <h3 className="text-base font-semibold text-white mb-2">{faq.question}</h3>
                    <p className="text-sm text-slate-300 leading-relaxed">{faq.answer}</p>
                  </div>
                ))}
              </div>
            </section>
          )}
        </div>

        {/* Sidebar: Related Terms */}
        <aside className="lg:col-span-1 space-y-4">
          <div className="p-5 rounded-2xl bg-slate-800/80 border border-slate-700/60 sticky top-24">
            <h3 className="text-sm font-semibold uppercase tracking-wider text-slate-400 mb-3">
              Related Terminology
            </h3>
            <div className="space-y-2">
              {otherTerms.map((other) => (
                <Link
                  key={other.slug}
                  to={other.slug}
                  className="flex items-center justify-between p-2.5 rounded-xl text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-700/50 transition-colors"
                >
                  <span className="truncate">{other.title.split(":")[0]}</span>
                  <ChevronRight className="w-4 h-4 text-slate-500 shrink-0" />
                </Link>
              ))}
            </div>

            <div className="mt-6 pt-4 border-t border-slate-700/60">
              <Link
                to="/glossary/"
                className="text-xs font-semibold text-blue-400 hover:text-blue-300 flex items-center gap-1"
              >
                <span>View Full Glossary (8+ Terms) &rarr;</span>
              </Link>
            </div>
          </div>
        </aside>
      </div>
    </MarketingLayout>
  );
}

