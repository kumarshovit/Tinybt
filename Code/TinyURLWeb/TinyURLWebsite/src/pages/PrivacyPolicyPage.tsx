// src/pages/PrivacyPolicyPage.tsx

import Navbar from "../components/Navbar";
import Footer from "../components/Footer";

const PrivacyPolicyPage = () => {

  return (

    <div className="min-h-screen flex flex-col bg-gray-50">

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
                Privacy Policy
              </h1>

              <p className="mt-6 text-lg text-gray-600 leading-8">

                Your privacy matters to us.

                This Privacy Policy explains how LINKBT
                collects, stores, manages, and protects
                your data across our smart URL platform.

              </p>

              <div className="flex flex-wrap gap-4 mt-8">

                <div className="bg-blue-100 text-blue-700 px-4 py-2 rounded-full text-sm font-medium">
                  Secure Platform
                </div>

                <div className="bg-green-100 text-green-700 px-4 py-2 rounded-full text-sm font-medium">
                  Analytics Enabled
                </div>

                <div className="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm font-medium">
                  Privacy Focused
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
                1. Information We Collect
              </h2>

              <p className="text-gray-600 leading-8">

                LINKBT may collect personal information
                such as email addresses, account data,
                shortened URLs, browser information,
                and analytics activity.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                2. How We Use Your Information
              </h2>

              <p className="text-gray-600 leading-8">

                Your information helps us provide secure
                authentication, URL shortening services,
                analytics insights, and overall
                platform improvements.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                3. Analytics & Tracking
              </h2>

              <p className="text-gray-600 leading-8">

                LINKBT collects analytics such as clicks,
                browser type, operating systems,
                device information, and geographic data
                to improve user experience.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                4. Security & Protection
              </h2>

              <p className="text-gray-600 leading-8">

                We apply industry-standard security measures
                to protect user information from unauthorized
                access, misuse, or disclosure.

              </p>

            </div>

            {/* Card */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                5. Policy Updates
              </h2>

              <p className="text-gray-600 leading-8">

                LINKBT reserves the right to update
                this Privacy Policy at any time.
                Updated versions will always appear here.

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

export default PrivacyPolicyPage;