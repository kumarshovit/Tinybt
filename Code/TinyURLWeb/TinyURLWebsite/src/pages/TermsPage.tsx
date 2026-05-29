// src/pages/TermsPage.tsx

import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";

const TermsPage = () => {

  return (

    <div className="min-h-screen flex flex-col bg-gray-50">

      <SEO
        title="Terms & Conditions – LinkBT"
        description="Read the Terms & Conditions governing your use of the LinkBT URL shortening platform. Understand your rights and responsibilities as a user."
        canonical="/terms"
      />

      {/* Navbar */}
      <Navbar />

      {/* Main */}
      <main className="flex-1 bg-gradient-to-b from-blue-50 via-white to-gray-50">

        {/* Hero */}
        <section className="border-b border-gray-200 bg-white">

          <div className="max-w-6xl mx-auto px-6 py-16">

            <div className="max-w-3xl">

              <p className="text-blue-600 font-semibold mb-3 tracking-wide uppercase">
                LINKBT Legal
              </p>

              <h1 className="text-5xl font-bold text-gray-900 leading-tight">
                Terms & Conditions
              </h1>

              <p className="mt-6 text-lg text-gray-600 leading-8">

                These Terms & Conditions govern your use of LINKBT
                and its smart URL shortening services.

                By using our platform, you agree to comply with
                these terms and policies.

              </p>

              <div className="flex flex-wrap gap-4 mt-8">

                <div className="bg-blue-100 text-blue-700 px-4 py-2 rounded-full text-sm font-medium">
                  Secure Platform
                </div>

                <div className="bg-green-100 text-green-700 px-4 py-2 rounded-full text-sm font-medium">
                  Trusted Services
                </div>

                <div className="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm font-medium">
                  Smart URL Management
                </div>

              </div>

            </div>

          </div>

        </section>

        {/* Content */}
        <section className="max-w-5xl mx-auto px-6 py-14">

          <div className="space-y-8">

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                1. Use of Services
              </h2>

              <p className="text-gray-600 leading-8">

                LINKBT provides URL shortening,
                analytics, and link management services.

                Users must use the platform responsibly
                and only for lawful purposes.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                2. User Responsibilities
              </h2>

              <p className="text-gray-600 leading-8">

                Users are responsible for maintaining
                account security, protecting credentials,
                and ensuring their activities comply
                with platform policies.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                3. Prohibited Activities
              </h2>

              <p className="text-gray-600 leading-8">

                Users must not use LINKBT for phishing,
                malicious links, spam, illegal content,
                or attempts to compromise platform security.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                4. Service Availability
              </h2>

              <p className="text-gray-600 leading-8">

                While LINKBT aims to provide uninterrupted
                services, temporary downtime may occur
                due to maintenance or technical issues.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                5. Changes to Terms
              </h2>

              <p className="text-gray-600 leading-8">

                LINKBT reserves the right to modify these
                Terms & Conditions at any time.
                Updated versions will appear on this page.

              </p>

            </div>

          </div>

        </section>

      </main>

      {/* Footer */}
      <Footer />

    </div>
  );
};

export default TermsPage;