import { Link } from "react-router-dom";

const Footer = () => {
  const handleFooterClick = (e: React.MouseEvent<HTMLElement>) => {
    const target = (e.target as HTMLElement).closest("a");
    if (target && !target.getAttribute("href")?.startsWith("#")) {
      window.scrollTo({ top: 0, left: 0, behavior: "instant" });
    }
  };

  return (
    <footer onClick={handleFooterClick} className="bg-[#07142B] text-gray-300 pt-16 pb-8 mt-20 border-t border-gray-800">
      <div className="max-w-7xl mx-auto px-6">
        <div className="grid grid-cols-2 md:grid-cols-5 gap-10">

          {/* Col 1: Brand & Contact Info */}
          <div className="col-span-2 md:col-span-1">
            <h2 className="text-2xl font-bold text-white mb-3 tracking-tight">
              LINKBT
            </h2>
            <p className="text-sm text-gray-400 leading-6 mb-6">
              Smart, enterprise-grade URL shortening platform to create, manage, and track short links with multi-engine threat defense.
            </p>
            <div className="space-y-2 text-xs text-gray-400 leading-relaxed">
              <div>
                <span className="font-semibold text-gray-300">📍 Address:</span> NANO, Thimphu TechPark, Wangchuktaba, Babesa, Thimphu 11001, Bhutan
              </div>
              <div>
                <span className="font-semibold text-gray-300">📞 Phone:</span> +975 17701770
              </div>
              <div>
                <span className="font-semibold text-gray-300">✉️ Email:</span> info@link.bt
              </div>
            </div>
          </div>

          {/* Col 2: Free Tools */}
          <div>
            <h3 className="text-base font-semibold text-white mb-4 tracking-wide uppercase text-xs text-blue-400">
              Free Tools
            </h3>
            <div className="flex flex-col gap-2.5 text-sm">
              <Link to="/tools/" className="hover:text-white transition">All Free Tools</Link>
              <Link to="/tools/bulk-url-shortener/" className="hover:text-white transition">Bulk URL Shortener</Link>
              <Link to="/tools/qr-code-generator/" className="hover:text-white transition">QR Code Generator</Link>
              <Link to="/tools/utm-builder/" className="hover:text-white transition">UTM Campaign Builder</Link>
              <Link to="/tools/whatsapp-link-generator/" className="hover:text-white transition">WhatsApp Link Creator</Link>
              <Link to="/custom-aliases/" className="hover:text-white transition">Custom Aliases</Link>
              <Link to="/branded-links/" className="hover:text-white transition">Branded Short Links</Link>
            </div>
          </div>

          {/* Col 3: Solutions */}
          <div>
            <h3 className="text-base font-semibold text-white mb-4 tracking-wide uppercase text-xs text-blue-400">
              Solutions
            </h3>
            <div className="flex flex-col gap-2.5 text-sm">
              <Link to="/use-cases/" className="hover:text-white transition">Industry Use Cases</Link>
              <Link to="/for/marketers/" className="hover:text-white transition">For Marketers</Link>
              <Link to="/for/developers/" className="hover:text-white transition">For Developers</Link>
              <Link to="/for/small-business/" className="hover:text-white transition">For Small Business</Link>
              <Link to="/for/content-creators/" className="hover:text-white transition">For Content Creators</Link>
              <Link to="/compare/" className="hover:text-white transition">Compare Shorteners</Link>
              <Link to="/countries/" className="hover:text-white transition">Global Edge Network</Link>
            </div>
          </div>

          {/* Col 4: Resources & Learning */}
          <div>
            <h3 className="text-base font-semibold text-white mb-4 tracking-wide uppercase text-xs text-blue-400">
              Resources
            </h3>
            <div className="flex flex-col gap-2.5 text-sm">
              <Link to="/guides/" className="hover:text-white transition">Learning Guides</Link>
              <Link to="/docs/" className="hover:text-white transition">Developer API Docs</Link>
              <Link to="/docs/api-overview/" className="hover:text-white transition">REST API Reference</Link>
              <Link to="/glossary/" className="hover:text-white transition">Link Terminology Glossary</Link>
              <Link to="/security" className="hover:text-white transition">Security Architecture</Link>
              <Link to="/faq" className="hover:text-white transition">Frequently Asked Questions</Link>
            </div>
          </div>

          {/* Col 5: Company & Legal */}
          <div>
            <h3 className="text-base font-semibold text-white mb-4 tracking-wide uppercase text-xs text-blue-400">
              Company
            </h3>
            <div className="flex flex-col gap-2.5 text-sm">
              <Link to="/about/" className="hover:text-white transition">About LinkBT</Link>
              <Link to="/pricing" className="hover:text-white transition">Pricing Plans</Link>
              <Link to="/features" className="hover:text-white transition">Platform Features</Link>
              <Link to="/contact" className="hover:text-white transition">Contact Support</Link>
              <Link to="/report-abuse/" className="hover:text-white transition">Report Malicious Link</Link>
              <Link to="/terms" className="hover:text-white transition">Terms & Conditions</Link>
              <Link to="/privacy-policy" className="hover:text-white transition">Privacy Policy</Link>
            </div>
          </div>

        </div>

        {/* Bottom */}
        <div className="border-t border-gray-800 mt-12 pt-6 flex flex-col sm:flex-row items-center justify-between text-xs text-gray-500 gap-4">
          <div>
            © 2026 LINKBT • All Rights Reserved. Powered by <span className="text-gray-300 font-medium">Intellisoft Technology</span>
          </div>
          <div className="flex items-center gap-6">
            <Link to="/terms" className="hover:text-gray-400 transition">Terms</Link>
            <Link to="/privacy-policy" className="hover:text-gray-400 transition">Privacy</Link>
            <Link to="/security" className="hover:text-gray-400 transition">Security</Link>
            <Link to="/report-abuse/" className="hover:text-gray-400 transition">Report Abuse</Link>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;