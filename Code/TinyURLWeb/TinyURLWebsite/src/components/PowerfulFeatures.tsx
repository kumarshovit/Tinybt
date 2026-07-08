import FeatureCard from "./FeatureCard";

import Img1 from "../assets/Img1.png";
import Img2 from "../assets/Img2.png";
import Img3 from "../assets/Img3.png";
import Img4 from "../assets/Img4.png";

const PowerfulFeatures = () => {
  return (
    <section
      id="features"
      className="bg-gradient-to-b from-white via-blue-50 to-white py-24 px-6"
    >
      <div className="max-w-7xl mx-auto">

        {/* Heading */}

        <div className="text-center mb-16">

          <h2 className="text-4xl font-bold text-gray-900">
            Powerful Features
          </h2>

          <p className="mt-5 text-lg text-gray-600 max-w-3xl mx-auto leading-8">
            LinkBT offers a modern URL shortening platform designed for
            individuals, developers, marketers, and businesses. Create
            branded short URLs, monitor link performance, organize links,
            and improve your digital marketing strategy—all from one
            powerful dashboard.
          </p>

        </div>

        {/* Feature Cards */}

        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">

          <FeatureCard
            image={Img1}
            title="Custom Aliases"
            text="Create memorable branded links for your business."
          />

          <FeatureCard
            image={Img2}
            title="Analytics"
            text="Track clicks by device, browser and location."
          />

          <FeatureCard
            image={Img3}
            title="Expiration Control"
            text="Automatically disable links after a set time."
          />

          <FeatureCard
            image={Img4}
            title="Smart Tagging"
            text="Organize and search your links easily."
          />

        </div>

        {/* ============================= */}

        {/* Feature Showcase */}

        {/* ============================= */}

        <div className="mt-28 space-y-32">

          {/* Feature 1 */}

          <div className="grid lg:grid-cols-2 gap-16 items-center">

            <div>

              <img
                src={Img1}
                alt="Custom Alias"
                className="w-full max-w-md mx-auto"
              />

            </div>

            <div>

              <span className="text-blue-600 font-semibold uppercase tracking-wider">
                Branding
              </span>

              <h3 className="text-4xl font-bold mt-3 mb-6">
                Create Memorable Custom Aliases
              </h3>

              <p className="text-gray-600 leading-8 mb-6">

                Long URLs are difficult to remember, less attractive to
                users, and often reduce trust when shared across digital
                platforms. LinkBT allows you to replace lengthy web
                addresses with clean, professional, and memorable custom
                aliases that reflect your brand or campaign.

              </p>

              <p className="text-gray-600 leading-8 mb-8">

                Whether you're sharing marketing campaigns, social media
                posts, business presentations, email newsletters, or
                internal documents, custom aliases make every shortened
                link easier to recognize and significantly improve the
                overall user experience.

              </p>

              <div className="grid grid-cols-2 gap-4">

                <div className="bg-blue-50 rounded-xl p-4">
                  ✔ Easy to Remember
                </div>

                <div className="bg-blue-50 rounded-xl p-4">
                  ✔ Better Branding
                </div>

                <div className="bg-blue-50 rounded-xl p-4">
                  ✔ Professional URLs
                </div>

                <div className="bg-blue-50 rounded-xl p-4">
                  ✔ Higher Trust
                </div>

              </div>

            </div>

          </div>

          {/* Feature 2 */}

          <div className="grid lg:grid-cols-2 gap-16 items-center">

            <div>

              <span className="text-blue-600 font-semibold uppercase tracking-wider">
                Analytics
              </span>

              <h3 className="text-4xl font-bold mt-3 mb-6">
                Understand Every Click
              </h3>

              <p className="text-gray-600 leading-8 mb-6">

                LinkBT doesn't simply shorten URLs—it helps you understand
                how they perform. Every click provides valuable insights
                into audience engagement, allowing you to measure campaign
                effectiveness with confidence.

              </p>

              <p className="text-gray-600 leading-8 mb-8">

                Monitor clicks by browser, operating system, location,
                and device to understand where your audience comes from.
                These insights help businesses make smarter marketing
                decisions while improving customer engagement over time.

              </p>

              <div className="grid grid-cols-2 gap-4">

                <div className="bg-green-50 rounded-xl p-4">
                  📈 Click Tracking
                </div>

                <div className="bg-green-50 rounded-xl p-4">
                  🌍 Location Insights
                </div>

                <div className="bg-green-50 rounded-xl p-4">
                  💻 Browser Analytics
                </div>

                <div className="bg-green-50 rounded-xl p-4">
                  📱 Device Statistics
                </div>

              </div>

            </div>

            <div>

              <img
                src={Img2}
                alt="Analytics"
                className="w-full max-w-md mx-auto"
              />

            </div>

          </div>

                    {/* Feature 3 */}

          <div className="grid lg:grid-cols-2 gap-16 items-center">

            <div>

              <img
                src={Img3}
                alt="Expiration Control"
                className="w-full max-w-md mx-auto"
              />

            </div>

            <div>

              <span className="text-blue-600 font-semibold uppercase tracking-wider">
                Security
              </span>

              <h3 className="text-4xl font-bold mt-3 mb-6">
                Smart Expiration Control
              </h3>

              <p className="text-gray-600 leading-8 mb-6">

                Every shortened link doesn't need to stay active forever.
                LinkBT provides flexible expiration controls that allow
                you to automatically deactivate links after a specific
                date and time. This is especially useful for marketing
                campaigns, event registrations, confidential documents,
                temporary promotions, and time-sensitive resources.

              </p>

              <p className="text-gray-600 leading-8 mb-8">

                Instead of manually removing outdated links, LinkBT
                automatically handles expiration based on your settings.
                This helps improve security, prevents users from accessing
                outdated information, and keeps your shared content
                relevant and up to date.

              </p>

              <div className="grid grid-cols-2 gap-4">

                <div className="bg-purple-50 rounded-xl p-4">
                  ⏰ Automatic Expiration
                </div>

                <div className="bg-purple-50 rounded-xl p-4">
                  🔒 Better Security
                </div>

                <div className="bg-purple-50 rounded-xl p-4">
                  📅 Campaign Management
                </div>

                <div className="bg-purple-50 rounded-xl p-4">
                  ⚡ Less Manual Work
                </div>

              </div>

            </div>

          </div>

          {/* Feature 4 */}

          <div className="grid lg:grid-cols-2 gap-16 items-center">

            <div>

              <span className="text-blue-600 font-semibold uppercase tracking-wider">
                Organization
              </span>

              <h3 className="text-4xl font-bold mt-3 mb-6">
                Organize Everything with Smart Tagging
              </h3>

              <p className="text-gray-600 leading-8 mb-6">

                Managing a growing collection of shortened URLs can quickly
                become difficult without proper organization. LinkBT's
                Smart Tagging feature allows users to categorize links
                using meaningful labels, making it easier to locate,
                manage, and maintain URLs for different projects,
                campaigns, or departments.

              </p>

              <p className="text-gray-600 leading-8 mb-8">

                Whether you manage marketing campaigns, internal
                documentation, client resources, or promotional content,
                Smart Tagging keeps everything organized in one place.
                Instead of endlessly scrolling through long lists, simply
                search by tag and find exactly what you need within
                seconds.

              </p>

              <div className="grid grid-cols-2 gap-4">

                <div className="bg-orange-50 rounded-xl p-4">
                  🏷 Easy Categorization
                </div>

                <div className="bg-orange-50 rounded-xl p-4">
                  🔍 Quick Search
                </div>

                <div className="bg-orange-50 rounded-xl p-4">
                  📂 Better Organization
                </div>

                <div className="bg-orange-50 rounded-xl p-4">
                  🚀 Higher Productivity
                </div>

              </div>

            </div>

            <div>

              <img
                src={Img4}
                alt="Smart Tagging"
                className="w-full max-w-md mx-auto"
              />

            </div>

          </div>

          {/* SEO Section */}

          <div className="mt-32 bg-gradient-to-r from-blue-600 to-indigo-700 rounded-3xl p-12 text-white">

            <div className="max-w-5xl mx-auto text-center">

              <h2 className="text-4xl font-bold mb-8">
                Everything You Need to Manage Short Links Efficiently
              </h2>

              <p className="leading-9 text-lg text-blue-100">

                LinkBT is more than a traditional URL shortener. It is a
                complete link management platform built for businesses,
                developers, marketers, content creators, and individuals
                who need a secure and organized way to create, manage,
                and monitor shortened URLs. From creating memorable
                custom aliases to understanding detailed analytics,
                LinkBT simplifies every stage of link management.

                <br /><br />

                Our platform focuses on usability, performance,
                organization, and security while providing valuable
                insights into user engagement. Whether you're sharing
                marketing campaigns, event registrations, business
                resources, educational content, or internal documents,
                LinkBT helps you distribute professional-looking links
                with confidence.

                <br /><br />

                With features such as analytics, expiration control,
                custom aliases, and smart tagging, LinkBT ensures that
                every shared URL remains easy to manage throughout its
                lifecycle. Our goal is to provide an intuitive platform
                that saves time, improves productivity, and delivers
                meaningful insights without unnecessary complexity.

              </p>

            </div>

          </div>

        </div>

      </div>

    </section>
  );
};

export default PowerfulFeatures;