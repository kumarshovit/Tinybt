import { useState, useMemo } from "react";
import MarketingLayout from "../../components/marketing/MarketingLayout";
import { MessageSquare, Copy, Check, ExternalLink, RefreshCw, Send, ShieldCheck } from "lucide-react";
import { buildBreadcrumbSchema, buildFAQSchema, buildSoftwareApplicationSchema } from "../../seo/schemaBuilders";

const FAQS = [
  {
    question: "What is a WhatsApp Click-to-Chat link?",
    answer: "WhatsApp's Click to Chat feature allows you to begin a direct conversation with someone without having their phone number saved in your phone's address book. Clicking the link automatically opens a WhatsApp chat window with the recipient."
  },
  {
    question: "How should the phone number be formatted?",
    answer: "Use international format without any plus signs (+), zeroes, dashes, brackets, or spaces. For example, use 15551234567 for a US number or 919876543210 for an Indian number."
  },
  {
    question: "Can I add a custom pre-filled message?",
    answer: "Yes! When visitors click your generated WhatsApp link, your pre-filled message will automatically appear in the text input field, ready for the user to send with one tap."
  },
  {
    question: "Why should I shorten my WhatsApp link with LinkBT?",
    answer: "WhatsApp wa.me links with long pre-filled text can look cluttered and suspicious. Shortening with LinkBT gives you a clean link.bt URL, tracks total click counts, and lets you update your destination if your number ever changes."
  }
];

export default function WhatsAppLinkGeneratorPage() {
  const [countryCode, setCountryCode] = useState("1");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [message, setMessage] = useState("");
  const [copied, setCopied] = useState(false);

  const cleanNumber = useMemo(() => {
    const raw = `${countryCode}${phoneNumber}`.replace(/\D/g, "");
    return raw;
  }, [countryCode, phoneNumber]);

  const generatedLink = useMemo(() => {
    if (!cleanNumber || cleanNumber.length < 7) return "";
    let link = `https://wa.me/${cleanNumber}`;
    if (message.trim()) {
      link += `?text=${encodeURIComponent(message.trim())}`;
    }
    return link;
  }, [cleanNumber, message]);

  const handleCopy = () => {
    if (!generatedLink) return;
    navigator.clipboard.writeText(generatedLink);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleReset = () => {
    setPhoneNumber("");
    setMessage("");
  };

  const breadcrumbs = [
    { label: "Tools", href: "/tools/" },
    { label: "WhatsApp Link Generator", href: "/tools/whatsapp-link-generator/" }
  ];

  const jsonLd = [
    buildBreadcrumbSchema([
      { name: "Home", url: "/" },
      { name: "Tools", url: "/tools/" },
      { name: "WhatsApp Link Generator", url: "/tools/whatsapp-link-generator/" }
    ]),
    buildSoftwareApplicationSchema("LinkBT WhatsApp Link Generator", "Free online WhatsApp Click to Chat direct link creator with pre-filled messages."),
    buildFAQSchema(FAQS)
  ];

  return (
    <MarketingLayout
      seo={{
        title: "Free WhatsApp Link Generator – Click to Chat with Pre-Filled Text | LinkBT",
        description: "Generate free direct WhatsApp Click-to-Chat links with custom pre-filled messages. Perfect for Instagram bios, websites, ads, and customer support.",
        canonical: "/tools/whatsapp-link-generator/",
        keywords: "whatsapp link generator, click to chat whatsapp, wa me link creator, whatsapp message link",
        jsonLd
      }}
      badge="Free Messaging Utility"
      breadcrumbs={breadcrumbs}
      title="Free WhatsApp Click-to-Chat Link Generator"
      subtitle="Create direct wa.me chat links with custom pre-filled messages so customers and clients can reach you instantly on WhatsApp with one click."
    >
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Interactive Configuration Card */}
        <div className="lg:col-span-7 bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 sm:p-8 backdrop-blur-sm shadow-xl">
          <div className="flex items-center justify-between mb-6 pb-4 border-b border-slate-700">
            <h2 className="text-xl font-bold text-white flex items-center gap-2">
              <MessageSquare className="w-5 h-5 text-emerald-400" />
              <span>Configure WhatsApp Chat Link</span>
            </h2>
            <button
              onClick={handleReset}
              className="text-xs text-slate-400 hover:text-slate-200 flex items-center gap-1 transition-colors"
            >
              <RefreshCw className="w-3.5 h-3.5" />
              <span>Reset</span>
            </button>
          </div>

          <div className="space-y-4">
            {/* Country Code and Phone */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Phone Number (with Country Code) <span className="text-red-400">*</span>
              </label>
              <div className="flex gap-2">
                <div className="w-28 relative">
                  <span className="absolute left-3 top-2.5 text-slate-400 text-sm font-mono">+</span>
                  <input
                    type="text"
                    placeholder="1"
                    value={countryCode}
                    onChange={(e) => setCountryCode(e.target.value.replace(/\D/g, ""))}
                    className="w-full pl-7 pr-3 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white font-mono text-sm focus:outline-none focus:border-emerald-500"
                    title="Country Calling Code"
                  />
                </div>
                <input
                  type="text"
                  placeholder="5551234567"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value.replace(/\D/g, ""))}
                  className="flex-1 px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white font-mono text-sm focus:outline-none focus:border-emerald-500 placeholder-slate-500"
                />
              </div>
              <p className="text-xs text-slate-400 mt-1">Do not include leading zeroes, plus symbols, dashes, or spaces.</p>
            </div>

            {/* Pre-filled Message */}
            <div>
              <label className="block text-sm font-semibold text-slate-200 mb-1.5">
                Pre-Filled Message (Optional)
              </label>
              <textarea
                rows={4}
                placeholder="Hi LinkBT! I'd like to ask a few questions about your enterprise plan..."
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                className="w-full px-4 py-2.5 rounded-xl bg-slate-900 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-emerald-500 text-sm leading-relaxed"
              />
              <p className="text-xs text-slate-400 mt-1">This text will be automatically typed in the user&apos;s WhatsApp input field.</p>
            </div>
          </div>
        </div>

        {/* Live Output Card */}
        <div className="lg:col-span-5 space-y-6">
          <div className="bg-slate-800/80 border border-slate-700/80 rounded-2xl p-6 backdrop-blur-sm shadow-xl">
            <h3 className="text-base font-bold text-white mb-3">Your WhatsApp Link</h3>

            {generatedLink ? (
              <div>
                <div className="p-3.5 rounded-xl bg-slate-900/90 border border-slate-700 font-mono text-xs text-emerald-300 break-all select-all max-h-36 overflow-y-auto mb-4 leading-relaxed">
                  {generatedLink}
                </div>

                <div className="flex flex-col gap-3">
                  <div className="flex gap-2">
                    <button
                      onClick={handleCopy}
                      className="flex-1 inline-flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-sm transition-all shadow-md active:scale-95"
                    >
                      {copied ? (
                        <>
                          <Check className="w-4 h-4 text-white" />
                          <span>Copied!</span>
                        </>
                      ) : (
                        <>
                          <Copy className="w-4 h-4" />
                          <span>Copy WhatsApp Link</span>
                        </>
                      )}
                    </button>

                    <a
                      href={generatedLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="inline-flex items-center justify-center px-4 py-2.5 rounded-xl bg-slate-700 hover:bg-slate-600 text-slate-200 text-sm font-medium transition-colors"
                      title="Open chat in new tab"
                    >
                      <Send className="w-4 h-4" />
                    </a>
                  </div>

                  <a
                    href={`/?url=${encodeURIComponent(generatedLink)}`}
                    className="inline-flex items-center justify-center gap-1.5 px-4 py-2.5 rounded-xl bg-blue-600/20 hover:bg-blue-600/30 text-blue-300 border border-blue-500/30 font-medium text-sm transition-colors"
                  >
                    <span>Shorten with LinkBT for Analytics</span>
                    <ExternalLink className="w-3.5 h-3.5" />
                  </a>
                </div>
              </div>
            ) : (
              <div className="p-8 text-center rounded-xl bg-slate-900/50 border border-dashed border-slate-700 text-slate-400 text-sm">
                Enter your country code and phone number above to preview your WhatsApp click-to-chat link.
              </div>
            )}
          </div>

          <div className="bg-emerald-950/20 border border-emerald-900/40 rounded-2xl p-6 text-sm text-slate-300 space-y-2">
            <h4 className="font-semibold text-emerald-300 flex items-center gap-1.5">
              <ShieldCheck className="w-4 h-4 text-emerald-400" />
              <span>Privacy Guaranteed</span>
            </h4>
            <p className="text-xs text-slate-400 leading-relaxed">
              LinkBT generates links entirely in your browser. We never store, log, or share the phone numbers or messages entered in this free tool.
            </p>
          </div>
        </div>
      </div>

      {/* Guide & FAQs */}
      <section className="mt-16 pt-12 border-t border-slate-800">
        <div className="max-w-3xl">
          <h2 className="text-2xl font-bold text-white mb-4">
            How WhatsApp Click-to-Chat Links Increase Conversions
          </h2>
          <div className="space-y-4 text-slate-300 text-base leading-relaxed">
            <p>
              Requiring customers to manually type a 10-digit phone number into their contacts app creates friction and lost leads. By placing a pre-formatted WhatsApp link in your Instagram bio, TikTok profile, or website header, potential customers can initiate an inquiry in under two seconds.
            </p>
            <p>
              Pairing your WhatsApp link with a LinkBT custom alias (like <code className="text-emerald-400 font-mono text-xs">link.bt/chat-support</code>) makes it memorable for verbal promotions, print flyers, and video descriptions while recording live click metrics.
            </p>
          </div>

          <h3 className="text-xl font-bold text-white mt-8 mb-3">Frequently Asked Questions</h3>
          <div className="space-y-4 mt-4">
            {FAQS.map((faq, idx) => (
              <div key={idx} className="p-5 rounded-xl bg-slate-800/60 border border-slate-700/60">
                <h4 className="font-semibold text-white text-base mb-2">{faq.question}</h4>
                <p className="text-slate-300 text-sm leading-relaxed">{faq.answer}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </MarketingLayout>
  );
}

