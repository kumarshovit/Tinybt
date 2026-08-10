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
      <section className="bg-gradient-to-br from-[#0f172a] via-[#0f2439] to-[#0a3854] relative pt-32 pb-56 overflow-hidden">
        {/* Wave SVG at bottom */}
        <div className="absolute bottom-0 left-0 w-full overflow-hidden leading-[0]">
          <svg data-name="Layer 1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 120" preserveAspectRatio="none" className="relative block w-full h-[60px] md:h-[100px]" style={{ transform: "rotateY(180deg)" }}>
            <path d="M321.39,56.44c58-10.79,114.16-30.13,172-41.86,82.39-16.72,168.19-17.73,250.45-.39C823.78,31,906.67,72,985.66,92.83c70.05,18.48,146.53,26.09,214.34,3V120H0V95.8C59.71,118.11,130.83,121.92,189.65,108.6Z" fill="#f9fafb"></path>
          </svg>
        </div>

        <div className="max-w-7xl mx-auto px-6 grid md:grid-cols-2 gap-10 items-center relative z-10">

          <div>

            <h1 className="text-5xl font-bold mb-6 leading-tight text-white">
              Create Smart Short Links,
              <br />
              Track Detailed Analytics &
              <br />
              Manage Your Brand
            </h1>

            <p className="text-blue-100/90 text-lg mb-8 leading-8 max-w-xl">
              LinkBT is a complete platform to amplify your reach. Create branded, memorable aliases, monitor real-time traffic, manage link lifecycles, and use powerful tags—all from an intuitive, unified dashboard. Empower your marketing, secure your links, and grow with insights.
            </p>

            <div className="flex gap-4">

              <Link
                to="/register"
                className="bg-blue-600 font-semibold text-white px-8 py-3.5 rounded-lg hover:bg-blue-700 transition shadow-lg shadow-blue-600/30"
              >
                Get Started Free
              </Link>

              <Link
                to="/login"
                className="bg-white font-semibold flex items-center justify-center text-gray-900 border border-gray-200 px-8 py-3.5 rounded-lg hover:bg-gray-50 transition shadow-lg shadow-black/5"
              >
                Login
              </Link>

            </div>

          </div>

          {/* SHORTEN CARD PREVIEW */}
          <div className="flex justify-end hidden md:flex md:-mr-16">
            <ShortenCard onUrlCreated={() => { }} />
          </div>
          <div className="flex justify-center md:hidden mt-8">
            <ShortenCard onUrlCreated={() => { }} />
          </div>

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