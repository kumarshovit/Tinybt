import { useState } from "react";
import { Link } from "react-router-dom";
import Navbar from "../components/Navbar";
import { CheckCircle2, ChevronRight, Sparkles } from "lucide-react";

export default function PricingPage() {
  const [isAnnual, setIsAnnual] = useState(false);
  const [proLinks, setProLinks] = useState(125);
  const [bulkLinks, setBulkLinks] = useState(50000);

  // Dynamic pricing calculation based on sliders
  const baseProPrice = isAnnual ? 9 * 12 : 9;
  const proAdditionalCost = Math.max(0, Math.floor((proLinks - 125) / 10)) * 1.5;
  const currentProPrice = (baseProPrice + proAdditionalCost).toFixed(2);

  const baseBulkPrice = isAnnual ? 69 * 12 : 69;
  const bulkAdditionalCost = Math.max(0, Math.floor((bulkLinks - 50000) / 1000)) * 0.6;
  const currentBulkPrice = (baseBulkPrice + bulkAdditionalCost).toFixed(2);

  return (
    <div className="min-h-screen bg-[#020817] text-gray-100 flex flex-col font-sans selection:bg-blue-500/30">
      <Navbar />

      <main className="flex-grow pt-32 pb-24 px-6 md:px-12 relative overflow-hidden">
        {/* Ambient Glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[800px] h-[500px] bg-blue-600/20 rounded-full blur-[120px] pointer-events-none opacity-60"></div>

        <div className="max-w-7xl mx-auto relative z-10">
          
          <div className="text-center max-w-3xl mx-auto mb-16">
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-extrabold tracking-tight mb-6 bg-clip-text text-transparent bg-gradient-to-r from-white to-gray-400">
              Find a plan that meets your needs
            </h1>
            <p className="text-xl text-gray-400 font-light">
              LinkBT's paid tiers offer powerful link branding and customization features.
              Because why settle for being noticed when you can be remembered?
            </p>
          </div>

          {/* Toggle */}
          <div className="flex justify-center mb-16">
            <div className="bg-[#0f172a]/80 p-1.5 rounded-full border border-white/10 flex items-center shadow-lg backdrop-blur-md">
              <button
                onClick={() => setIsAnnual(false)}
                className={`px-8 py-3 rounded-full text-sm font-semibold transition-all duration-300 ${
                  !isAnnual ? "bg-blue-600 text-white shadow-md shadow-blue-500/20" : "text-gray-400 hover:text-white"
                }`}
              >
                Monthly
              </button>
              <button
                onClick={() => setIsAnnual(true)}
                className={`px-8 py-3 rounded-full text-sm font-semibold transition-all duration-300 flex items-center gap-2 ${
                  isAnnual ? "bg-blue-600 text-white shadow-md shadow-blue-500/20" : "text-gray-400 hover:text-white"
                }`}
              >
                Annually
                <span className="bg-emerald-500/20 text-emerald-400 text-[10px] uppercase px-2 py-0.5 rounded-full tracking-wider">Save 20%</span>
              </button>
            </div>
          </div>

          {/* Pricing Grid */}
          <div className="grid md:grid-cols-2 xl:grid-cols-4 gap-8 lg:gap-10">
            
            {/* FREE PLAN */}
            <div className="bg-[#1e293b]/60 backdrop-blur-xl border border-white/10 rounded-3xl p-8 flex flex-col relative group hover:border-gray-500/50 transition-all duration-300 hover:shadow-2xl hover:shadow-gray-500/10">
              <div className="mb-6">
                <h3 className="text-2xl font-bold text-white mb-2">Free</h3>
                <div className="flex items-baseline gap-2">
                  <span className="text-4xl font-extrabold text-white">$0</span>
                  <span className="text-gray-400 font-medium">/ mo</span>
                </div>
                <p className="text-sm text-gray-400 mt-3 h-10">
                  Essential link shortening and basic tracking for individuals.
                </p>
              </div>

              <div className="flex-grow mt-[72px]">
                <ul className="space-y-4 text-gray-300 text-sm font-medium">
                  <li className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span><strong className="text-white">50 Links / mo</strong></span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>Basic Analytics</span>
                  </li>
                  <li className="flex items-center gap-3 opacity-50">
                    <CheckCircle2 className="w-5 h-5 text-gray-600 shrink-0" />
                    <span className="line-through">Branded Domains</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>No Credit required</span>
                  </li>
                </ul>
              </div>

              <div>
                <Link onClick={() => window.scrollTo(0, 0)} to="/" className="mt-8 w-full block text-center bg-white/5 hover:bg-gray-700 border border-white/10 hover:border-transparent text-white font-semibold py-3.5 rounded-xl transition-all duration-300">
                  Get Started Free
                </Link>
              </div>
            </div>

            {/* PRO PLAN */}
            <div className="bg-[#1e293b]/60 backdrop-blur-xl border border-white/10 rounded-3xl p-8 flex flex-col relative group hover:border-blue-500/50 transition-all duration-300 hover:shadow-2xl hover:shadow-blue-500/10">
              <div className="mb-6">
                <h3 className="text-2xl font-bold text-white mb-2">Pro</h3>
                <div className="flex items-baseline gap-2">
                  <span className="text-4xl font-extrabold text-white">${currentProPrice}</span>
                  <span className="text-gray-400 font-medium">/ {isAnnual ? "yr" : "mo"}</span>
                </div>
                <p className="text-sm text-gray-400 mt-3 h-10">
                  Get full access to advanced link analytics, editing, and management.
                </p>
              </div>

              <div className="mb-8">
                <div className="flex justify-between text-sm text-gray-300 mb-2 font-medium">
                  <span>125</span>
                  <span>4K Links</span>
                </div>
                <input 
                  type="range" 
                  min="125" 
                  max="4000" 
                  step="10"
                  value={proLinks}
                  onChange={(e) => setProLinks(Number(e.target.value))}
                  className="w-full h-1.5 bg-gray-700 rounded-lg appearance-none cursor-pointer accent-blue-500"
                />
              </div>

              <div className="flex-grow">
                <ul className="space-y-4 text-gray-300 text-sm font-medium">
                  <li className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span><strong className="text-white">{proLinks} Links / mo</strong><br/><span className="text-gray-500 text-xs">+$1.50 per 10 additional links</span></span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span>Unlimited Tracked Clicks</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span>3 Branded Domains</span>
                  </li>
                </ul>
              </div>

              <Link to="/register" className="mt-8 w-full block text-center bg-white/5 hover:bg-blue-600 border border-white/10 hover:border-transparent text-white font-semibold py-3.5 rounded-xl transition-all duration-300">
                Subscribe Now
              </Link>
            </div>

            {/* BULK 50K PLAN */}
            <div className="bg-gradient-to-b from-blue-900/40 to-[#1e293b]/60 backdrop-blur-xl border border-blue-500/30 rounded-3xl p-8 flex flex-col relative group hover:border-blue-400 transition-all duration-300 shadow-2xl shadow-blue-900/20 transform md:-translate-y-4">
              <div className="absolute -top-4 left-1/2 -translate-x-1/2 bg-blue-600 text-white text-xs font-bold uppercase tracking-widest py-1.5 px-4 rounded-full flex items-center gap-1.5 shadow-lg shadow-blue-600/30">
                <Sparkles className="w-3.5 h-3.5" /> Most Popular
              </div>

              <div className="mb-6">
                <h3 className="text-2xl font-bold text-white mb-2">Bulk 50K</h3>
                <div className="flex items-baseline gap-2">
                  <span className="text-4xl font-extrabold text-white">${currentBulkPrice}</span>
                  <span className="text-gray-400 font-medium">/ {isAnnual ? "yr" : "mo"}</span>
                </div>
                <p className="text-sm text-gray-400 mt-3 h-10">
                  Generate, edit and manage your links in bulk with high volume capacity.
                </p>
              </div>

              <div className="mb-8">
                <div className="flex justify-between text-sm text-gray-300 mb-2 font-medium">
                  <span>50K</span>
                  <span>5M Links</span>
                </div>
                <input 
                  type="range" 
                  min="50000" 
                  max="5000000" 
                  step="50000"
                  value={bulkLinks}
                  onChange={(e) => setBulkLinks(Number(e.target.value))}
                  className="w-full h-1.5 bg-gray-700 rounded-lg appearance-none cursor-pointer accent-blue-500"
                />
              </div>

              <div className="flex-grow">
                <ul className="space-y-4 text-gray-300 text-sm font-medium">
                  <li className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span><strong className="text-white">{bulkLinks.toLocaleString()} Links / mo</strong><br/><span className="text-gray-500 text-xs">+$2.00 per 1000 additional links</span></span>
                  </li>
                  <li className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span><strong className="text-white">{bulkLinks.toLocaleString()} Tracked Clicks / mo</strong><br/><span className="text-gray-500 text-xs">+$0.60 per 1000 additional clicks</span></span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-blue-400 shrink-0" />
                    <span>3 Branded Domains</span>
                  </li>
                </ul>
              </div>

              <Link to="/register" className="mt-8 w-full block text-center bg-blue-600 hover:bg-blue-500 text-white font-semibold py-3.5 rounded-xl transition-all duration-300 shadow-lg shadow-blue-500/25">
                Subscribe Now
              </Link>
            </div>

            {/* ENTERPRISE PLAN */}
            <div className="bg-[#1e293b]/60 backdrop-blur-xl border border-white/10 rounded-3xl p-8 flex flex-col relative group hover:border-white/20 transition-all duration-300 hover:shadow-2xl hover:shadow-white/5">
              <div className="mb-6">
                <h3 className="text-2xl font-bold text-white mb-2">Enterprise</h3>
                <div className="flex items-baseline gap-2">
                  <span className="text-4xl font-extrabold text-white">Custom</span>
                </div>
                <p className="text-sm text-gray-400 mt-3 h-10">
                  A tailor-made plan for enterprises that need more than our regular plans offer.
                </p>
                <div className="text-xs text-gray-500 mt-1">
                  (Starts at $3,999 / yr)
                </div>
              </div>

              <div className="flex-grow mt-10">
                <ul className="space-y-4 text-gray-300 text-sm font-medium">
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>Custom Number of Links</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>Custom Number of Tracked Clicks</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>Custom Number of Branded Domains</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>99.9% SLA-backed uptime</span>
                  </li>
                  <li className="flex items-center gap-3">
                    <CheckCircle2 className="w-5 h-5 text-gray-400 shrink-0" />
                    <span>Custom Solutions for Compliance</span>
                  </li>
                </ul>
              </div>

              <Link to="/contact" className="mt-8 w-full flex items-center justify-center gap-2 bg-white text-gray-900 hover:bg-gray-100 font-semibold py-3.5 rounded-xl transition-all duration-300">
                Contact Sales <ChevronRight className="w-4 h-4" />
              </Link>
            </div>

          </div>
          
          <div className="mt-12 text-center text-xs text-gray-500 font-medium">
            * Listed prices exclude any applicable taxes.
          </div>
        </div>
      </main>
    </div>
  );
}
