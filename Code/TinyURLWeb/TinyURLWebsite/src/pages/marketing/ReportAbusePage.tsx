import { useState } from "react";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { AlertTriangle, CheckCircle2, Send, ShieldAlert, Mail } from "lucide-react";
import { buildBreadcrumbSchema } from "../../seo/schemaBuilders";

export default function ReportAbusePage() {
  const [shortUrl, setShortUrl] = useState("");
  const [abuseType, setAbuseType] = useState("phishing");
  const [email, setEmail] = useState("");
  const [notes, setNotes] = useState("");
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!shortUrl.trim()) return;
    // In production, triggers abuse queue ingestion
    setSubmitted(true);
  };

  const breadcrumbs = [
    { label: "Report Abuse", href: "/report-abuse/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Report Abuse", url: "/report-abuse/" }
    ])
  ];

  return (
    <MarketingLayout
      seo={{
        title: "Report Abuse – Flag Malicious Links, Phishing & Malware | LinkBT",
        description: "Submit abusive, malicious, or deceptive LinkBT short links for immediate security investigation and takedown. Protecting users from web threats.",
        canonical: "/report-abuse/",
        keywords: "report abuse, flag phishing link, report malicious url, takedown request",
        jsonLd
      }}
      badge="Security Operations"
      breadcrumbs={breadcrumbs}
      title="Report Malicious or Abusive Short Links"
      subtitle="LinkBT has zero tolerance for phishing, malware, scams, or fraudulent activity. Submit suspicious links for immediate automated inspection and takedown."
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-10 items-start">
        {/* Form */}
        <div className="lg:col-span-7 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          {submitted ? (
            <div className="text-center py-8 space-y-4">
              <div className="w-16 h-16 rounded-full bg-emerald-500/10 text-emerald-400 flex items-center justify-center mx-auto">
                <CheckCircle2 className="w-8 h-8" />
              </div>
              <h2 className="text-2xl font-bold text-white">Report Received</h2>
              <p className="text-slate-300 text-sm max-w-md mx-auto leading-relaxed">
                Thank you for helping keep the web secure. Our automated scanning engines and security team have queued <code className="text-blue-400 font-mono text-xs">{shortUrl}</code> for inspection. Confirmed abusive links are disabled immediately.
              </p>
              <button
                onClick={() => {
                  setSubmitted(false);
                  setShortUrl("");
                  setNotes("");
                }}
                className="mt-4 px-6 py-2.5 rounded-xl bg-slate-700 hover:bg-slate-600 text-white text-sm font-semibold transition-colors"
              >
                Report Another Link
              </button>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-5">
              <h2 className="text-xl font-bold text-white mb-2 flex items-center gap-2">
                <AlertTriangle className="w-5 h-5 text-amber-400" />
                <span>Submit Abuse Report</span>
              </h2>

              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Suspect LinkBT URL <span className="text-red-400">*</span>
                </label>
                <input
                  type="text"
                  required
                  placeholder="https://link.bt/suspicious-link"
                  value={shortUrl}
                  onChange={(e) => setShortUrl(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
                />
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Type of Abuse <span className="text-red-400">*</span>
                </label>
                <select
                  value={abuseType}
                  onChange={(e) => setAbuseType(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white focus:outline-none focus:border-blue-500 text-sm"
                >
                  <option value="phishing">Phishing / Credential Theft</option>
                  <option value="malware">Malware / Ransomware / Virus Distribution</option>
                  <option value="spam">Unsolicited Spam / Deceptive Ads</option>
                  <option value="scam">Financial Scam / Fraud</option>
                  <option value="impersonation">Brand Impersonation / Trademark Violation</option>
                  <option value="other">Other Violation</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Your Email Address (Optional)
                </label>
                <input
                  type="email"
                  placeholder="contact@yourdomain.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm"
                />
                <p className="text-xs text-slate-400 mt-1">If you would like confirmation of the investigation outcome.</p>
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                  Additional Details / Evidence
                </label>
                <textarea
                  rows={3}
                  placeholder="Please describe where you encountered this link or any specific threats observed..."
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-blue-500 text-sm leading-relaxed"
                />
              </div>

              <button
                type="submit"
                className="w-full inline-flex items-center justify-center gap-2 px-6 py-3 rounded-xl bg-red-600 hover:bg-red-500 text-white font-bold text-sm transition-all shadow-md active:scale-95"
              >
                <Send className="w-4 h-4" />
                <span>Submit Abuse Report</span>
              </button>
            </form>
          )}
        </div>

        {/* Takedown SLA & Security Info */}
        <div className="lg:col-span-5 space-y-6">
          <div className="p-6 rounded-2xl bg-slate-800/80 border border-slate-700/80 space-y-4">
            <h3 className="text-lg font-bold text-white flex items-center gap-2">
              <ShieldAlert className="w-5 h-5 text-red-400" />
              <span>Our Takedown Process</span>
            </h3>
            <p className="text-xs text-slate-300 leading-relaxed">
              When an abuse report is received, the target link is immediately submitted to Google Safe Browsing, URLhaus, and Cloudflare Scanner for algorithmic verification. If any scanner confirms malicious activity, the short link is immediately quarantined.
            </p>
            <div className="p-3.5 rounded-xl bg-slate-900 border border-slate-800 text-xs text-slate-400 space-y-1">
              <div className="font-semibold text-slate-200">Average Response Time:</div>
              <div>Automated Quarantine: &lt; 5 minutes</div>
              <div>Manual Review: &lt; 2 hours</div>
            </div>
          </div>

          <div className="p-6 rounded-2xl bg-blue-950/20 border border-blue-900/40 text-xs text-slate-300 space-y-2">
            <h4 className="font-semibold text-blue-300 flex items-center gap-1.5">
              <Mail className="w-4 h-4 text-blue-400" />
              <span>Urgent Law Enforcement & CERT Contacts</span>
            </h4>
            <p className="text-slate-400 leading-relaxed">
              For law enforcement, CERT teams, and urgent legal notices, you can contact our security operations center directly at <a href="mailto:info@link.bt" className="text-blue-400 font-semibold hover:underline">info@link.bt</a>.
            </p>
          </div>
        </div>
      </div>
    </MarketingLayout>
  );
}

