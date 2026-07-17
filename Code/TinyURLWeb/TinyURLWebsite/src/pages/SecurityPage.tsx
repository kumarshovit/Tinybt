// src/pages/SecurityPage.tsx

import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";

const SecurityPage = () => {
  return (
    <div className="min-h-screen flex flex-col bg-gray-50">

      <SEO
        title="Security – LinkBT"
        description="Learn how LinkBT protects users with URL validation, DNS verification, HTTPS preference, Google Safe Browsing, and advanced abuse prevention."
        canonical="/security"
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
                LINKBT Security
              </p>

              <h1 className="text-5xl font-bold text-gray-900 leading-tight">
                Security at LinkBT
              </h1>

              <p className="mt-6 text-lg text-gray-600 leading-8">
                At <strong>LinkBT</strong>, protecting users from malicious,
                misleading, and unsafe links is one of our highest priorities.
                Every URL submitted to our platform undergoes multiple security
                validations before a short link is created, helping provide a
                safer, more reliable, and trustworthy URL shortening experience.
              </p>
              <div className="flex flex-wrap gap-4 mt-8">

  <div className="bg-blue-100 text-blue-700 px-4 py-2 rounded-full text-sm font-medium">
    URL Validation
  </div>

  <div className="bg-green-100 text-green-700 px-4 py-2 rounded-full text-sm font-medium">
    DNS Verification
  </div>

  <div className="bg-purple-100 text-purple-700 px-4 py-2 rounded-full text-sm font-medium">
    Safe Browsing
  </div>

  <div className="bg-orange-100 text-orange-700 px-4 py-2 rounded-full text-sm font-medium">
    Abuse Protection
  </div>

</div>

            </div>

          </div>

        </section>

        {/* Content */}
        <section className="max-w-5xl mx-auto px-6 py-14">

          <div className="space-y-8">

            {/* Card 1 */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">
                <h2 className="text-2xl font-bold text-gray-900 mb-4">
  1. URL Format Validation
</h2>

<p className="text-gray-600 leading-8 mb-5">
  Every URL submitted to LinkBT is validated before it is shortened
  to ensure it complies with our security standards.
</p>

<ul className="list-disc pl-6 space-y-3 text-gray-600 leading-8">
  <li>Accepts only valid URL formats.</li>
  <li>Supports HTTPS protocols only.</li>
  <li>Rejects malformed or unsupported URL schemes.</li>
  <li>Blocks localhost and private network addresses.</li>
  <li>Rejects embedded username/password credentials.</li>
</ul>

<p className="text-gray-600 leading-8 mt-5">
  These validations help prevent malformed, deceptive, and potentially
  dangerous URLs from being shortened through LinkBT.
</p>
             

            </div>

            {/* Card 2 */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
  2. DNS Verification
</h2>

<p className="text-gray-600 leading-8 mb-5">
  Before a short link is created, LinkBT performs a DNS lookup to
  verify that the destination domain exists and can be resolved.
</p>

<p className="font-semibold text-gray-800 mb-3">
  This helps prevent:
</p>

<ul className="list-disc pl-6 space-y-3 text-gray-600 leading-8">
  <li>Broken links.</li>
  <li>Non-existent domains.</li>
  <li>Typographical errors in domain names.</li>
</ul>

<p className="text-gray-600 leading-8 mt-5">
  Only domains with valid DNS records are accepted for shortening,
  helping improve the reliability of every LinkBT URL.
</p>

            </div>

            {/* Card 3 */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

             <h2 className="text-2xl font-bold text-gray-900 mb-4">
  3. HTTPS Preference
</h2>

<p className="text-gray-600 leading-8 mb-5">
  Whenever supported by the destination website, LinkBT only prefer or support HTTPS links. This ensures that users are directed to the most secure version of a website, helping protect their data and privacy.
</p>

<p className="font-semibold text-gray-800 mb-3">
  Benefits include:
</p>

<ul className="list-disc pl-6 space-y-3 text-gray-600 leading-8">
  <li>Encrypted communication.</li>
  <li>Improved privacy.</li>
  <li>Better protection against interception.</li>
  <li>More secure browsing for visitors.</li>
</ul>

            </div>

            {/* Card 4 */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
  4. Google Safe Browsing Protection
</h2>

<p className="text-gray-600 leading-8 mb-5">
  Every destination URL is checked using Google Safe Browsing before
  a short link is created.
</p>

<p className="font-semibold text-gray-800 mb-3">
  This helps detect and block:
</p>

<ul className="list-disc pl-6 space-y-3 text-gray-600 leading-8">
  <li>Phishing websites.</li>
  <li>Malware distribution.</li>
  <li>Social engineering attacks.</li>
  <li>Unsafe or deceptive websites.</li>
</ul>

<p className="text-gray-600 leading-8 mt-5">
  URLs identified as unsafe are automatically rejected and cannot be
  shortened through LinkBT.
</p>
            </div>

            {/* Card 5 */}
            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

              <h2 className="text-2xl font-bold text-gray-900 mb-4">
  5. Abuse Prevention
</h2>

<p className="text-gray-600 leading-8 mb-5">
  LinkBT includes multiple protection mechanisms to reduce abuse and
  maintain a safe, reliable, and trustworthy platform.
</p>

<p className="font-semibold text-gray-800 mb-3">
  Our protection systems include:
</p>

<ul className="list-disc pl-6 space-y-3 text-gray-600 leading-8">
  {/* <li>Rate limiting to prevent automated abuse.</li> */}
  <li>CAPTCHA verification to block malicious bots.</li>
  <li>Abuse reporting for suspicious and harmful links.</li>
  {/* <li>Administrator moderation for reviewing and removing malicious content.</li> */}
</ul>

<p className="text-gray-600 leading-8 mt-5">
  These security measures help keep LinkBT safe for individuals,
  organizations, and businesses using our platform.
</p>

            </div>

            <div className="bg-white border border-gray-200 shadow-sm rounded-3xl p-8 hover:shadow-md transition">

  <h2 className="text-2xl font-bold text-gray-900 mb-4">
    6. Our Commitment to Security
  </h2>

  <p className="text-gray-600 leading-8">
    Security is a continuous process at LinkBT. We regularly review,
    update, and strengthen our security controls to protect users from
    emerging threats and evolving cyber risks.
  </p>

  <p className="text-gray-600 leading-8 mt-5">
    By combining proactive validation, trusted security services, and
    ongoing platform improvements, LinkBT is committed to providing a
    secure, reliable, and trustworthy URL shortening service.
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

export default SecurityPage;