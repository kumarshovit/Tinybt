import Img1 from "../assets/Img1.png";
import Img2 from "../assets/Img2.png";
import Img3 from "../assets/Img3.png";
import Img4 from "../assets/Img4.png";
import { ArrowRight } from "lucide-react";

interface StepProps {
  image: string;
  number: string;
  title: string;
  text: string;
  isLast?: boolean;
}

function Step({
  image,
  number,
  title,
  text,
  isLast = false,
}: StepProps) {
  return (
    <div className="relative group bg-white rounded-3xl border border-gray-200 shadow-sm hover:shadow-2xl hover:-translate-y-2 transition-all duration-300 p-8">

      {/* Image */}
      <div className="flex justify-center mb-6">
        <div className="bg-blue-50 rounded-2xl p-4 group-hover:bg-blue-100 transition duration-300">
          <img
            src={image}
            alt={title}
            className="w-24 h-24 object-contain group-hover:scale-110 transition-transform duration-300"
          />
        </div>
      </div>

      {/* Step Number */}
      <div className="w-12 h-12 rounded-full bg-blue-600 text-white flex items-center justify-center text-lg font-bold mx-auto mb-5 shadow-lg">
        {number}
      </div>

      {/* Title */}
      <h3 className="text-xl font-bold text-center mb-4">
        {title}
      </h3>

      {/* Description */}
      <p className="text-gray-600 text-center leading-7">
        {text}
      </p>

      {/* Divider */}
      <div className="w-12 h-1 bg-blue-600 rounded-full mx-auto mt-6 group-hover:w-20 transition-all duration-300"></div>

      {/* Arrow */}
      {!isLast && (
        <ArrowRight
          size={30}
          className="hidden lg:block absolute -right-7 top-1/2 -translate-y-1/2 text-blue-400"
        />
      )}
    </div>
  );
}

const HowItWorks = () => {
  return (
    <section
      id="how"
      className="bg-gradient-to-b from-blue-50 via-white to-blue-50 py-24 px-6"
    >
      <div className="max-w-7xl mx-auto">

        {/* Heading */}

        <div className="text-center mb-16">

          <span className="text-blue-600 uppercase tracking-widest font-semibold">
            Simple Process
          </span>

          <h2 className="text-4xl font-bold mt-4">
            How LinkBT Works
          </h2>

          <p className="max-w-3xl mx-auto mt-6 text-lg text-gray-600 leading-8">
            LinkBT makes URL shortening simple, fast, and secure.
            Follow four easy steps to create professional short links,
            personalize them with custom aliases, and monitor their
            performance through a powerful analytics dashboard.
          </p>

        </div>

        {/* Steps */}

        <div className="grid lg:grid-cols-4 gap-8">

          <Step
            image={Img1}
            number="01"
            title="Paste Your Long URL"
            text="Enter your long destination URL into LinkBT to begin creating a secure and shareable short link."
          />

          <Step
            image={Img2}
            number="02"
            title="Customize Alias"
            text="Create a memorable custom alias that represents your brand, campaign, or business."
          />

          <Step
            image={Img3}
            number="03"
            title="Generate Short Link"
            text="Instantly generate your shortened URL and share it across websites, emails, social media, or documents."
          />

          <Step
            image={Img4}
            number="04"
            title="Track Analytics"
            text="Monitor clicks, browsers, devices, and user engagement using LinkBT's built-in analytics dashboard."
            isLast
          />

        </div>

        {/* Bottom Section */}

        <div className="mt-24 bg-white rounded-3xl border border-gray-200 shadow-sm p-10">

          <div className="max-w-4xl mx-auto text-center">

            <h3 className="text-3xl font-bold mb-6">
              Fast, Secure & Designed for Everyone
            </h3>

            <p className="text-gray-600 leading-9 text-lg">

              Whether you're a marketer sharing campaigns, a developer
              managing application links, or a business promoting
              products and services, LinkBT simplifies the entire URL
              management process. Every feature is designed to reduce
              manual effort while giving you complete control over your
              shortened links.

              <br /><br />

              Create memorable aliases, organize links with smart
              tagging, monitor performance through detailed analytics,
              and securely manage URLs from one intuitive dashboard.
              LinkBT helps you share smarter and understand every click.

            </p>

          </div>

        </div>

      </div>
    </section>
  );
};

export default HowItWorks;