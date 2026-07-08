import { useState } from "react";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
import { ChevronDown, ChevronUp } from "lucide-react";

const faqData = [
  {
    question: "What is LinkBT?",
    answer:
      "LinkBT is a smart URL shortening platform that transforms long URLs into short, shareable links while providing analytics and easy link management.",
  },
  {
    question: "How do I shorten a URL?",
    answer:
      "Paste your long URL into the input field, optionally choose a custom alias, and click 'Shorten URL'. Your shortened link will be generated instantly.",
  },
  {
    question: "Can I create a custom alias?",
    answer:
      "Yes. LinkBT allows you to create a custom alias, provided it is unique and available.",
  },
  {
    question: "Can I update my shortened link later?",
    answer:
      "Yes. You can edit the destination URL or update the custom alias whenever required.",
  },
  {
    question: "Can I delete a shortened URL?",
    answer:
      "Yes. You can remove links that are no longer required from your dashboard.",
  },
  {
    question: "Does LinkBT provide analytics?",
    answer:
      "Yes. LinkBT provides analytics such as click tracking and link performance to help you monitor your shortened URLs.",
  },
  {
    question: "Are my shortened links secure?",
    answer:
      "Yes. LinkBT follows secure practices to protect your links and user information.",
  },
  {
    question: "Is there any limit to the number of URLs I can shorten?",
    answer:
      "You can create and manage multiple shortened URLs based on your account permissions.",
  },
  {
    question: "How can I contact LinkBT support?",
    answer:
      "You can reach us through the Contact Us page or by using the contact information available in the website footer.",
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