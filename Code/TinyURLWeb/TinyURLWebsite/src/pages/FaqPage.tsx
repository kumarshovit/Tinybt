import { useState } from "react";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
import { ChevronDown, ChevronUp } from "lucide-react";

const faqData = [
  {
    question: "What is LinkBT?",
    answer:
      "LinkBT is a modern URL shortening and link management platform that helps individuals, businesses, marketers, developers, and content creators transform long URLs into clean, professional short links. In addition to generating shortened URLs, LinkBT provides powerful features such as custom aliases, detailed click analytics, smart tagging, expiration controls, and centralized link management. Whether you're sharing links on social media, email campaigns, websites, advertisements, or internal business systems, LinkBT makes every URL easier to remember, easier to share, and easier to monitor. Our goal is to simplify URL management while providing users with valuable insights that help improve engagement, branding, and overall online performance.",
  },
  {
    question: "How do I shorten a URL?",
    answer:
      "Paste your long URL into the input field, optionally choose a custom alias, and click 'Shorten URL'. Your shortened link will be generated instantly.",
  },
  {
    question: "Can I create a custom alias?",
    answer:
      "Yes. LinkBT allows you to create custom aliases so your shortened URLs become more meaningful, memorable, and professional. Instead of random characters, you can personalize your short link using words related to your business, campaign, event, or product. A branded alias improves trust, increases click-through rates, and makes links easier for users to recognize. Before creating the alias, LinkBT automatically checks its availability to ensure uniqueness. This feature is especially useful for businesses, digital marketers, educational institutions, and organizations that want consistent branding across all shared links.",
  },
  {
  question: "Does LinkBT scan links for malware?",
  answer:
    "Yes. Google Safe Browsing integration is being added to automatically detect phishing and malware before links are shortened.",
},
 {
  question: "Can I update my shortened link later?",
  answer:
    "Yes. LinkBT allows you to update the destination URL of your shortened link whenever needed. If you are using a custom alias, you can also modify it based on availability. This makes it easy to keep your links up to date without creating a new short URL, helping you maintain consistency across your marketing campaigns and shared content.",
},
 {
  question: "Can I delete a shortened URL?",
  answer:
    "Yes. You can delete shortened URLs that are no longer needed directly from your dashboard. Removing unused or outdated links helps keep your account organized and ensures you only manage active links. Depending on your account permissions, deleted links may no longer redirect visitors to the original destination.",
},
{
  question: "Does LinkBT provide analytics?",
  answer:
    "Yes. LinkBT provides detailed link analytics to help you understand how your shortened URLs are performing. You can monitor metrics such as total clicks, unique visitors, devices, browsers, operating systems, locations, referrers, and other engagement data. These insights help businesses, marketers, and individuals measure campaign performance and optimize their marketing strategies.",
},
{
  question: "Are my shortened links secure?",
  answer:
    "Yes. Security is a priority at LinkBT. We follow industry best practices to protect your shortened URLs and user information. Our platform uses secure HTTPS connections and continuously works to safeguard your links from unauthorized access, helping ensure a reliable and secure experience for both link creators and visitors.",
},
{
  question: "Is there any limit to the number of URLs I can shorten?",
  answer:
    "LinkBT allows you to create and manage multiple shortened URLs based on your account type and permissions. Whether you're shortening a few personal links or managing hundreds of business links, our platform is designed to help you organize, track, and manage your URLs efficiently from a single dashboard.",
},
{
  question: "How can I contact LinkBT support?",
  answer:
    "If you need assistance, you can contact the LinkBT support team through the Contact Us page on our website. You can also use the contact information provided in the website footer to reach us. Our team is available to help with account issues, technical questions, feature requests, and general inquiries.",
},
];

const FaqPage = () => {
  const [openIndex, setOpenIndex] = useState<number | null>(0);

  const toggle = (index: number) => {
    setOpenIndex(openIndex === index ? null : index);
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <SEO
        title="FAQ – LinkBT"
        description="Frequently Asked Questions about LinkBT. Find answers related to URL shortening, custom aliases, analytics, and link management."
        canonical="/faq"
      />

      <Navbar />

      <main className="flex-1 bg-gradient-to-b from-blue-50 via-white to-gray-50">

        {/* Hero */}
        <section className="border-b border-gray-200 bg-white">
          <div className="max-w-6xl mx-auto px-6 py-16">
            <div className="max-w-3xl">

              <p className="text-blue-600 font-semibold mb-3 tracking-wide uppercase">
                LINKBT Support
              </p>

              <h1 className="text-5xl font-bold text-gray-900">
                Frequently Asked Questions
              </h1>

              <p className="mt-6 text-lg text-gray-600 leading-8">
                Find answers to the most common questions about LinkBT,
                including URL shortening, custom aliases, analytics,
                and link management.
              </p>

              <div className="flex flex-wrap gap-4 mt-8">
                <div className="bg-blue-100 text-blue-700 px-4 py-2 rounded-full text-sm font-medium">
                  URL Shortening
                </div>

                <div className="bg-green-100 text-green-700 px-4 py-2 rounded-full text-sm font-medium">
                  Analytics
                </div>

                <div className="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm font-medium">
                  Support
                </div>
              </div>

            </div>
          </div>
        </section>

        {/* FAQ */}
        <section className="max-w-5xl mx-auto px-6 py-14">

          <div className="space-y-5">

            {faqData.map((faq, index) => (
              <div
                key={index}
                className="bg-white border border-gray-200 rounded-2xl shadow-sm overflow-hidden transition hover:shadow-md"
              >
                <button
                  onClick={() => toggle(index)}
                  className="w-full flex justify-between items-center px-8 py-6 text-left"
                >
                  <h2 className="text-lg md:text-xl font-semibold text-gray-900">
                    {faq.question}
                  </h2>

                  {openIndex === index ? (
                    <ChevronUp className="text-blue-600" size={22} />
                  ) : (
                    <ChevronDown className="text-blue-600" size={22} />
                  )}
                </button>

                {openIndex === index && (
                  <div className="px-8 pb-6">
                    <p className="text-gray-600 leading-8">
                      {faq.answer}
                    </p>
                  </div>
                )}
              </div>
            ))}

          </div>

          {/* Contact Card */}

          <div className="mt-16 bg-blue-600 rounded-3xl text-white text-center p-10 shadow-lg">

            <h2 className="text-3xl font-bold">
              Still have questions?
            </h2>

            <p className="mt-4 text-blue-100 max-w-2xl mx-auto leading-7">
              Can't find the answer you're looking for?
              Our team is always happy to help you.
            </p>

            <a
              href="/contact"
              className="inline-block mt-8 bg-white text-blue-600 font-semibold px-8 py-3 rounded-xl hover:bg-gray-100 transition"
            >
              Contact Us
            </a>

          </div>

        </section>
      </main>

      <Footer />
    </div>
  );
};

export default FaqPage;