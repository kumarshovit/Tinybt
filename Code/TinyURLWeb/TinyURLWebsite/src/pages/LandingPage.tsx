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
      "logo": "https://link.bt/fevicon.svg",
      "contactPoint": {
        "@type": "ContactPoint",
        "email": "support@linkbt.com",
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
        title="LinkBT – Free URL Shortener & Link Analytics"
        description="Create branded short links, track clicks by device, browser & location, and manage all your links from one powerful dashboard. Free forever."
        canonical="/"
        jsonLd={jsonLd}
      />

      <Navbar />

      {/* HERO SECTION */}
      <section className="max-w-7xl mx-auto px-6 py-24 grid md:grid-cols-2 gap-30 items-center">

        <div>

          <h1 className="text-5xl font-bold mb-6 leading-tight">
            Shorten & Manage
            <span className="text-blue-600"> Your Links</span>
          </h1>

          <p className="text-gray-600 text-lg mb-8">
            Create branded short URLs, track analytics, and manage
            your links from one powerful dashboard.
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
      <section className="bg-blue-600 text-white py-20 text-center">

        <h2 className="text-3xl font-bold mb-4">
          Ready to create smarter links?
        </h2>

        <p className="mb-8">
          Join thousands of users managing their links with LinkBt.
        </p>

        <Link
          to="/register"
          className="bg-white text-blue-600 px-6 py-3 rounded-lg font-semibold"
        >
          Create Free Account
        </Link>

      </section>
      <Footer />

    </div>
  );
}