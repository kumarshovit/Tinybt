import { Link } from "react-router-dom";
import { ChevronDown } from "lucide-react";

const faqs = [
  {
    question: "What is LinkBT?",
    answer:
      "LinkBT is a smart URL shortening platform that allows you to create, manage, and track shortened links from one powerful dashboard.",
  },
  {
    question: "Can I create a custom alias?",
    answer:
      "Yes. LinkBT allows you to create unique and memorable custom aliases for your shortened URLs.",
  },
  {
    question: "Does LinkBT provide analytics?",
    answer:
      "Yes. You can monitor clicks, browsers, devices, and other useful analytics for every shortened link.",
  },
  {
    question: "Can I organize my links?",
    answer:
      "Yes. Smart tagging and link management features help you organize and find your links quickly.",
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