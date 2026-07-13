import Navbar from "../components/Navbar";
import { Link } from "react-router-dom";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
import ShortenCard from "../components/ShortenCard";
import PowerfulFeatures from "../components/PowerfulFeatures";
import HowItWorks from "../components/HowItWorks";
import FAQPreview from "../components/FAQPreview";

const jsonLd = {
  "@context": "https://schema.org",
  "@graph": [
    {
      "@type": "WebSite",
      "@id": "https://link.bt/#website",
      "url": "https://link.bt/",
      "name": "LinkBT",
      "description": "Free URL shortener with analytics, custom aliases, and link management.",
    },
    {
      "@type": "Organization",
      "@id": "https://link.bt/#organization",
      "name": "LinkBT",
      "url": "https://link.bt/",
      "logo": "https://link.bt/favicon.svg",
      "contactPoint": {
        "@type": "ContactPoint",
        "email": "info@link.bt",
        "contactType": "customer support"
      }
    },
    {
      "@type": "SoftwareApplication",
      "name": "LinkBT – URL Shortener",
      "applicationCategory": "UtilitiesApplication",
      "operatingSystem": "Web",
      "offers": { "@type": "Offer", "price": "0", "priceCurrency": "USD" },
      "description": "Create branded short URLs, track click analytics by device, browser and location, and manage all your links in one dashboard."
    }
  ]
};

export default function LandingPage() {

  return (
    <div className="bg-gray-50 min-h-screen">

      <SEO
        title="Free URL Shortener – Shorten Links & Track Clicks | LinkBT"
        description="Shorten URLs and track clicks instantly with LinkBT, the best free URL shortener. Create custom aliases, monitor real-time analytics, and boost your engagement today!"
        canonical="/"
        jsonLd={jsonLd}
      />

      <Navbar />

      {/* HERO SECTION */}
      <section className="max-w-7xl mx-auto px-6 py-24 grid md:grid-cols-2 gap-30 items-center">

        <div>

          <h1 className="text-5xl font-bold mb-6 leading-tight">
            Create
            <span className="text-blue-600"> Short URLs</span>,
            <br />
            Track Analytics &
            <br />
            Manage Links
          </h1>

          <p className="text-gray-600 text-lg mb-8 leading-8">
            Create branded short URLs, customize memorable aliases, monitor
            real-time click analytics, manage link expiration, organize links
            with smart tags, and securely share links from one powerful
            dashboard. LinkBT helps businesses, developers, marketers, and
            content creators simplify link management while gaining valuable
            insights into link performance.
          </p>

          <div className="flex gap-4">

            <Link
              to="/register"
              className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition"
            >
              Get Started Free
            </Link>

            <Link
              to="/login"
              className="border px-6 py-3 rounded-lg hover:bg-gray-100"
            >
              Login
            </Link>

          </div>

        </div>

        {/* SHORTEN CARD PREVIEW */}
        <div>
          <ShortenCard onUrlCreated={() => { }} />

        </div>

      </section>


      {/* FEATURES */}
      <PowerfulFeatures />

      {/* HOW IT WORKS */}
      <HowItWorks />


      {/* FAQ */}
      <FAQPreview />


      {/* CTA */}
      {/* CTA */}
      <section className="bg-blue-600 text-white py-24">

        <div className="max-w-5xl mx-auto text-center px-6">

          <h2 className="text-4xl font-bold leading-tight mb-6">
            Ready to Create Smarter, Faster & Branded Short Links?
          </h2>

          <p className="text-xl text-blue-100 leading-9 max-w-4xl mx-auto">
            Join thousands of professionals, businesses, developers, marketers,
            educators, and content creators who trust LinkBT to simplify URL
            management. Create branded short links, organize URLs with smart
            tagging, monitor click analytics, customize memorable aliases,
            control link expiration, and manage everything from one intuitive
            dashboard. Whether you're running digital marketing campaigns,
            sharing educational resources, promoting products, or managing
            business communications.
          </p>

          <div className="flex flex-wrap justify-center gap-4 mt-10">

            <Link
              to="/register"
              className="bg-white text-blue-600 px-8 py-4 rounded-xl font-semibold hover:bg-gray-100 transition"
            >
              Create Free Account
            </Link>

            <Link
              to="/login"
              className="border border-white px-8 py-4 rounded-xl font-semibold hover:bg-white hover:text-blue-600 transition"
            >
              Sign In
            </Link>

          </div>

          <div className="grid md:grid-cols-4 gap-6 mt-14 text-center">

            <div>
              <h3 className="text-3xl font-bold">✓</h3>
              <p className="text-blue-100 mt-2">Custom Aliases</p>
            </div>

            <div>
              <h3 className="text-3xl font-bold">✓</h3>
              <p className="text-blue-100 mt-2">Analytics Dashboard</p>
            </div>

            <div>
              <h3 className="text-3xl font-bold">✓</h3>
              <p className="text-blue-100 mt-2">Smart Tagging</p>
            </div>

            <div>
              <h3 className="text-3xl font-bold">✓</h3>
              <p className="text-blue-100 mt-2">Expiration Control</p>
            </div>

          </div>

        </div>

      </section>
      <Footer />

    </div>
  );
}