import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
import PowerfulFeatures from "../components/PowerfulFeatures";

export default function FeaturesPage() {
  return (
    <div className="min-h-screen flex flex-col bg-gray-50">

      <SEO
        title="Powerful Features - LinkBT"
        description="Discover LinkBT's powerful features including custom aliases, analytics, smart tagging, expiration control, and complete link management."
        canonical="/features"
      />

      <Navbar />

      <main className="flex-1">


        {/* SAME COMPONENT */}
        <PowerfulFeatures />

      </main>

      <Footer />

    </div>
  );
}