import { Link } from "react-router-dom";
import { ChevronDown } from "lucide-react";

const faqs = [
  {
    question: "What is LinkBT?",
    answer:
      "LinkBT is a modern URL shortening and link management platform that helps individuals, businesses, marketers, developers, and content creators transform long URLs into clean, professional short links. In addition to generating shortened URLs, LinkBT provides powerful features such as custom aliases, detailed click analytics, smart tagging, expiration controls, and centralized link management. Whether you're sharing links on social media, email campaigns, websites, advertisements, or internal business systems, LinkBT makes every URL easier to remember, easier to share, and easier to monitor. Our goal is to simplify URL management while providing users with valuable insights that help improve engagement, branding, and overall online performance.",
  },
  {
    question: "Can I create a custom alias?",
    answer:
      "Yes. LinkBT allows you to create custom aliases so your shortened URLs become more meaningful, memorable, and professional. Instead of random characters, you can personalize your short link using words related to your business, campaign, event, or product. A branded alias improves trust, increases click-through rates, and makes links easier for users to recognize. Before creating the alias, LinkBT automatically checks its availability to ensure uniqueness. This feature is especially useful for businesses, digital marketers, educational institutions, and organizations that want consistent branding across all shared links.",
  },
  {
    question: "Does LinkBT provide analytics?",
    answer:
      "Absolutely. LinkBT includes built-in analytics that help you understand how your shortened links are performing. You can monitor total clicks, user engagement, browsers, devices, operating systems, and other valuable statistics directly from your dashboard. These insights help businesses measure marketing campaigns, evaluate audience behavior, improve content performance, and make informed decisions based on real user interactions. Instead of simply shortening links, LinkBT gives you meaningful data that helps optimize your digital marketing strategy and improve the effectiveness of every shared URL.",
  },
  {
    question: "Can I organize my links?",
    answer:
      "Yes. LinkBT provides smart link management features that make organizing large numbers of URLs simple and efficient. Users can assign tags, update destination URLs, edit custom aliases, manage expiration dates, and quickly search through their saved links whenever needed. Instead of manually maintaining spreadsheets or searching through hundreds of URLs, everything is managed from one centralized dashboard. This organized approach saves time, improves productivity, and allows individuals and teams to efficiently manage personal projects, business campaigns, educational resources, and marketing assets.",
  },
  {
    question: "Are my shortened links secure?",
    answer:
      "Security is one of LinkBT's primary priorities. Every shortened URL is managed through a secure platform designed to protect your data and maintain reliable redirects. Users can update destination URLs, set expiration dates for temporary links, and manage all their links from a secure dashboard. By combining secure infrastructure with modern link management features, LinkBT helps users confidently share links across websites, emails, social media platforms, and business communications.",
  },
];

const FAQPreview = () => {
  return (
    <section className="bg-gradient-to-b from-white via-blue-50 to-white py-24 px-6">
      <div className="max-w-5xl mx-auto">

        {/* Heading */}
        <div className="text-center mb-14">
          <h2 className="text-4xl font-bold text-gray-900">
            Frequently Asked Questions
          </h2>

          <p className="mt-4 text-lg text-gray-600 max-w-2xl mx-auto">
            Find answers to the most common questions about LinkBT and
            discover how our platform helps you create, manage, and track
            your shortened links with ease.
          </p>
        </div>

        {/* FAQ Cards */}
        <div className="space-y-5">

          {faqs.map((faq, index) => (
            <details
              key={index}
              className="group bg-white border border-gray-200 rounded-2xl shadow-sm p-6 hover:shadow-lg transition-all duration-300"
            >
              <summary className="flex items-center justify-between cursor-pointer list-none">
                <h3 className="text-lg font-semibold text-gray-900">
                  {faq.question}
                </h3>

                <ChevronDown className="w-5 h-5 text-blue-600 transition-transform duration-300 group-open:rotate-180" />
              </summary>

              <p className="mt-4 text-gray-600 leading-7">
                {faq.answer}
              </p>
            </details>
          ))}

        </div>

        {/* Bottom CTA */}
        <div className="text-center mt-12">

          <p className="text-gray-600 mb-5">
            Still have questions? Visit our complete FAQ page.
          </p>

          <Link
            to="/faq"
            className="inline-flex items-center bg-blue-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition"
          >
            View All FAQs
          </Link>

        </div>

      </div>
    </section>
  );
};

export default FAQPreview;