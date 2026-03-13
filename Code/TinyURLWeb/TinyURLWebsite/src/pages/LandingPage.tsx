import LandingNavbar from "../components/LandingNavbar";
import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";

export default function LandingPage() {

  const navigate = useNavigate();

  const [longUrl, setLongUrl] = useState("");
  const [alias, setAlias] = useState("");

  const handleGenerate = () => {

    const token = localStorage.getItem("token");

    if (!token) {
      navigate("/login");
      return;
    }

    navigate("/dashboard");
  };

  return (
    <div className="bg-gray-50 min-h-screen">

      <LandingNavbar />

      {/* HERO SECTION */}
      <section className="max-w-7xl mx-auto px-6 py-24 grid md:grid-cols-2 gap-12 items-center">

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
              className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-indigo-700 transition"
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

        <div className="bg-white shadow-xl rounded-2xl p-8 w-full max-w-md">

          <input
            placeholder="Enter long URL"
            value={longUrl}
            onChange={(e) => setLongUrl(e.target.value)}
            className="w-full border rounded-lg px-4 py-2 mb-4"
          />

          <input
            placeholder="Custom alias (optional)"
            value={alias}
            onChange={(e) => setAlias(e.target.value)}
            className="w-full border rounded-lg px-4 py-2 mb-4"
          />

          {/* Tags preview */}

          <input
            placeholder="Search or add tags..."
            disabled
            className="w-full border rounded-lg px-4 py-2 mb-4 bg-gray-100"
          />

          {/* Expiration preview */}

          <input
            type="datetime-local"
            disabled
            className="w-full border rounded-lg px-4 py-2 mb-4 bg-gray-100"
          />

          <button
            onClick={handleGenerate}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"
          >
            Generate Short Link
          </button>

          <p className="text-sm text-gray-500 text-center mt-3">
            Sign in to save and manage your links
          </p>

        </div>

      </section>


      {/* FEATURES */}
      <section id="features" className="bg-white py-24 px-6">

        <div className="max-w-7xl mx-auto text-center">

          <h2 className="text-3xl font-bold mb-12">
            Powerful Features
          </h2>

          <div className="grid md:grid-cols-4 gap-8">

            <FeatureCard
              title="Custom Aliases"
              text="Create memorable branded links for your business."
            />

            <FeatureCard
              title="Analytics"
              text="Track clicks by device, browser and location."
            />

            <FeatureCard
              title="Expiration Control"
              text="Automatically disable links after a set time."
            />

            <FeatureCard
              title="Smart Tagging"
              text="Organize and search your links easily."
            />

          </div>

        </div>

      </section>

      <section id="how" className="py-24 px-6">

        <div className="max-w-6xl mx-auto text-center">

          <h2 className="text-3xl font-bold mb-12">
            How It Works
          </h2>

          <div className="grid md:grid-cols-3 gap-10">

            <Step
              number="1"
              title="Paste your long URL"
              text="Enter your destination link into the generator."
            />

            <Step
              number="2"
              title="Customize alias"
              text="Create a branded and memorable short URL."
            />

            <Step
              number="3"
              title="Track analytics"
              text="Monitor clicks and engagement from your dashboard."
            />

          </div>

        </div>

      </section>
      {/* CTA */}
      <section className="bg-blue-600 text-white py-20 text-center">

        <h2 className="text-3xl font-bold mb-4">
          Ready to create smarter links?
        </h2>

        <p className="mb-8">
          Join thousands of users managing their links with TinyURL.
        </p>

        <Link
          to="/register"
          className="bg-white text-blue-600 px-6 py-3 rounded-lg font-semibold"
        >
          Create Free Account
        </Link>

      </section>
      <footer className="bg-gray-900 text-gray-300 py-10">

        <div className="max-w-7xl mx-auto px-6 grid md:grid-cols-4 gap-8">

          <div>
            <h3 className="text-white font-bold mb-3">
              TinyURL
            </h3>
            <p>
              Modern link management platform
              built for developers and marketers.
            </p>
          </div>

          <div>
            <h4 className="text-white mb-3">Product</h4>
            <p>Features</p>
            <p>Analytics</p>
            <p>API</p>
          </div>

          <div>
            <h4 className="text-white mb-3">Company</h4>
            <p>About</p>
            <p>Blog</p>
            <p>Careers</p>
          </div>

          <div>
            <h4 className="text-white mb-3">Legal</h4>
            <p>Privacy</p>
            <p>Terms</p>
          </div>

        </div>

      </footer>

    </div>
  );
}
function FeatureCard({ title, text }: any) {
  return (
    <div className="p-6 border rounded-xl hover:shadow-lg transition">
      <h3 className="font-semibold text-lg mb-2">{title}</h3>
      <p className="text-gray-600">{text}</p>
    </div>
  );
}
function Step({ number, title, text }: any) {
  return (
    <div>
      <div className="text-blue-600 text-4xl font-bold mb-4">
        {number}
      </div>
      <h3 className="font-semibold text-lg mb-2">{title}</h3>
      <p className="text-gray-600">{text}</p>
    </div>
  );
}

