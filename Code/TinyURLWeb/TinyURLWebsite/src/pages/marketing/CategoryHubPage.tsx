import { useLocation, Link } from "react-router-dom";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { ROUTE_REGISTRY, type RouteCategory, type RouteMeta } from "../../seo/routeRegistry";
import { buildBreadcrumbSchema, buildFAQSchema } from "../../seo/schemaBuilders";
import { ArrowRight, BookOpen, Layers, Users, Scale, Globe, FileText, ShieldCheck } from "lucide-react";

interface HubConfig {
  category: RouteCategory;
  badge: string;
  title: string;
  subtitle: string;
  canonical: string;
  icon: any;
  overviewHeading: string;
  overviewText: string;
  faqs: { question: string; answer: string }[];
}

const HUB_CONFIGS: Record<string, HubConfig> = {
  "/use-cases/": {
    category: "use-cases",
    badge: "Campaign Solutions",
    title: "Industry Use Cases for Smart Short Links",
    subtitle: "Explore proven frameworks and workflows for deploying short links and dynamic QR codes across social media, email campaigns, SMS alerts, and print media.",
    canonical: "/use-cases/",
    icon: Layers,
    overviewHeading: "Purpose-Built Link Management for Every Channel",
    overviewText: "Modern marketing touches customers across digital feeds, inboxes, physical flyers, and text messages. LinkBT provides specialized capabilities for each touchpoint, combining compact aesthetics with real-time attribution and proactive malware scanning.",
    faqs: [
      {
        question: "Can I use LinkBT for multiple channels simultaneously?",
        answer: "Yes! You can organize links by campaign tags and track performance individually across social, email, SMS, and offline media."
      },
      {
        question: "How do dynamic short links help print campaigns?",
        answer: "By creating trackable short links and QR codes, you can measure offline conversions and update landing page targets anytime without reprinting collateral."
      }
    ]
  },

  "/for/": {
    category: "audience",
    badge: "Role-Based Solutions",
    title: "Tailored Link Solutions for Modern Teams & Creators",
    subtitle: "Discover how marketers, software engineers, small business owners, content creators, and enterprise teams leverage LinkBT's secure platform.",
    canonical: "/for/",
    icon: Users,
    overviewHeading: "Empowering Professionals Across the Digital Economy",
    overviewText: "Whether you need to protect high-volume API microservices, share classroom assignments on blackboards, or drive audience traffic from podcasts, LinkBT offers purpose-built features with zero bloat and uncompromising speed.",
    faqs: [
      {
        question: "Is LinkBT suitable for non-technical users?",
        answer: "Yes, our web dashboard is clean, intuitive, and requires zero technical knowledge to create custom aliases or download QR codes."
      },
      {
        question: "What capabilities exist for software developers?",
        answer: "Developers can access high-throughput REST APIs, API keys, webhook integrations, and comprehensive documentation."
      }
    ]
  },

  "/guides/": {
    category: "guides",
    badge: "Learning Center",
    title: "URL Shortening & Link Intelligence Learning Hub",
    subtitle: "Deep-dive tutorials, technical guides, and best-practice frameworks for mastering link management, UTM campaign tracking, QR code printing, and redirect security.",
    canonical: "/guides/",
    icon: BookOpen,
    overviewHeading: "Master Digital Link Optimization & Security",
    overviewText: "Navigating HTTP redirects, search engine link equity, and malware defense requires clear guidance. Our learning center provides actionable, engineer-authored resources to help you build resilient link architectures.",
    faqs: [
      {
        question: "Are LinkBT guides free to access?",
        answer: "Yes, all educational guides and tutorials in the LinkBT Learning Center are completely free and regularly updated."
      }
    ]
  },

  "/compare/": {
    category: "compare",
    badge: "Platform Comparisons",
    title: "Fair, Feature-by-Feature URL Shortener Comparisons",
    subtitle: "Compare LinkBT with Bitly, TinyURL, Rebrandly, and Short.io on redirect speed, threat defense, custom aliases, API reliability, and value.",
    canonical: "/compare/",
    icon: Scale,
    overviewHeading: "Honest Technical Benchmarks for Smart Decisions",
    overviewText: "Choosing the right link management platform affects your deliverability, security posture, and budget. We provide transparent, factual comparisons highlighting the core architectural strengths and limitations of each service.",
    faqs: [
      {
        question: "Why choose LinkBT over older legacy shorteners?",
        answer: "LinkBT includes modern security features like 3-engine malware quarantine, password protection, and unthrottled custom aliases without charging hundreds of dollars per month."
      }
    ]
  },

  "/countries/": {
    category: "countries",
    badge: "Global Edge Network",
    title: "Global Link Infrastructure & Regional Edge Networks",
    subtitle: "Explore LinkBT's distributed network delivering sub-50ms link redirects and regional compliance for businesses across major international markets.",
    canonical: "/countries/",
    icon: Globe,
    overviewHeading: "Ultra-Low Latency Redirects Around the World",
    overviewText: "LinkBT routes redirect traffic through geographically distributed edge nodes in North America, Europe, Asia, and Oceania, ensuring that your customers experience instant page resolution wherever they are located.",
    faqs: [
      {
        question: "How does LinkBT achieve fast international redirects?",
        answer: "By caching DNS records and routing lookups through cloud points of presence close to the end user, avoiding high-latency international fiber hops."
      }
    ]
  },

  "/glossary/": {
    category: "glossary",
    badge: "Terminology Reference",
    title: "The Essential Link Management & URL Glossary",
    subtitle: "Authoritative definitions and explanations of key concepts in link management, web redirects, QR codes, UTM parameters, and click analytics.",
    canonical: "/glossary/",
    icon: FileText,
    overviewHeading: "Demystifying Web Redirect & Analytics Terminology",
    overviewText: "From HTTP 301 status codes to Reed-Solomon error correction and Base62 encoding, our glossary provides clear, technically precise definitions of the concepts powering modern web navigation.",
    faqs: [
      {
        question: "What is the most important term for marketing tracking?",
        answer: "UTM Parameters are the standard for campaign attribution, while HTTP 301 redirects are essential for preserving SEO link equity."
      }
    ]
  }
};

interface CategoryHubPageProps {
  hubPath?: string;
  category?: string;
}

export default function CategoryHubPage({ hubPath, category }: CategoryHubPageProps) {
  const location = useLocation();
  const normalizedCategory = category
    ? category.startsWith("/")
      ? category.endsWith("/") ? category : `${category}/`
      : `/${category}/`
    : "";
  const candidate = hubPath || normalizedCategory || location.pathname;
  const cleanPath = candidate.endsWith("/") ? candidate : `${candidate}/`;
  const config = HUB_CONFIGS[cleanPath];

  if (!config) {
    return (
      <div className="min-h-[60vh] flex flex-col items-center justify-center text-center p-6 bg-slate-900 text-white">
        <h1 className="text-3xl font-bold mb-4">Hub Under Construction</h1>
        <Link to="/" className="px-6 py-2.5 rounded-xl bg-blue-600 text-white font-semibold">
          Return Home
        </Link>
      </div>
    );
  }

  // Filter routes belonging to this category
  const categoryRoutes: RouteMeta[] = ROUTE_REGISTRY.filter(
    r => r.category === config.category && r.path !== config.canonical
  );

  const breadcrumbs = [
    { label: config.badge, href: config.canonical }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: config.badge, url: config.canonical }
    ]),
    buildFAQSchema(config.faqs)
  ];

  const Icon = config.icon;

  return (
    <MarketingLayout
      seo={{
        title: config.title,
        description: config.subtitle,
        canonical: config.canonical,
        jsonLd
      }}
      badge={config.badge}
      breadcrumbs={breadcrumbs}
      title={config.title}
      subtitle={config.subtitle}
    >
      <div className="space-y-16">
        {/* Overview Box */}
        <section className="p-8 rounded-2xl bg-slate-800/60 border border-slate-700/60 space-y-4 max-w-4xl">
          <div className="flex items-center gap-3 text-blue-400">
            <Icon className="w-6 h-6" />
            <h2 className="text-xl sm:text-2xl font-bold text-white">{config.overviewHeading}</h2>
          </div>
          <p className="text-slate-300 text-base leading-relaxed">
            {config.overviewText}
          </p>
        </section>

        {/* Directory Grid */}
        <section className="space-y-6">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-bold text-white">Featured In This Category</h2>
            <span className="text-xs text-slate-400 font-semibold">{categoryRoutes.length} Published Pages</span>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {categoryRoutes.map((route) => (
              <Link
                key={route.path}
                to={route.path}
                className="group p-6 rounded-2xl bg-slate-800/80 hover:bg-slate-800 border border-slate-700/80 hover:border-blue-500/50 transition-all duration-200 shadow-lg hover:shadow-xl flex flex-col justify-between"
              >
                <div>
                  <h3 className="text-lg font-bold text-white mb-2 group-hover:text-blue-400 transition-colors line-clamp-2">
                    {route.h1 || route.title}
                  </h3>
                  <p className="text-xs sm:text-sm text-slate-300 line-clamp-3 leading-relaxed mb-4">
                    {route.description}
                  </p>
                </div>
                <div className="pt-3 border-t border-slate-700/60 flex items-center justify-between text-xs font-semibold text-blue-400 group-hover:text-blue-300">
                  <span>Explore Guide</span>
                  <ArrowRight className="w-3.5 h-3.5 transition-transform group-hover:translate-x-1" />
                </div>
              </Link>
            ))}
          </div>
        </section>

        {/* FAQs */}
        {config.faqs && config.faqs.length > 0 && (
          <section className="pt-12 border-t border-slate-800 space-y-6 max-w-3xl">
            <h2 className="text-2xl font-bold text-white flex items-center gap-2">
              <ShieldCheck className="w-6 h-6 text-blue-400" />
              <span>Category FAQs</span>
            </h2>
            <div className="space-y-4">
              {config.faqs.map((faq, idx) => (
                <div key={idx} className="p-5 rounded-xl bg-slate-800/60 border border-slate-700/60 space-y-2">
                  <h3 className="font-semibold text-white text-base">{faq.question}</h3>
                  <p className="text-slate-300 text-sm leading-relaxed">{faq.answer}</p>
                </div>
              ))}
            </div>
          </section>
        )}
      </div>
    </MarketingLayout>
  );
}

