import { Link } from "react-router-dom";

const Footer = () => {

  return (

    <footer className="bg-[#07142B] text-gray-300 pt-10 pb-6 mt-20">

      <div className="max-w-7xl mx-auto px-6 flex flex-col md:flex-row justify-between items-center gap-8">

        {/* Brand */}
        <div>

          <h2 className="text-2xl font-bold text-white mb-3">
            LINKBT
          </h2>

          <p className="text-sm text-gray-400 max-w-md leading-6">

            Smart URL shortening platform to create,
            manage, and track your short links securely.

          </p>

        </div>

        {/* Quick Links */}
        <div className="flex flex-wrap items-center gap-6 text-sm">

          <Link
            to="/contact"
            className="hover:text-white transition"
          >
            Contact Us
          </Link>

          <Link
            to="/terms"
            className="hover:text-white transition"
          >
            Terms & Conditions
          </Link>

          <Link
            to="/privacy-policy"
            className="hover:text-white transition"
          >
            Privacy Policy
          </Link>
        </div>

      </div>

      {/* Bottom */}
      <div className="border-t border-gray-800 mt-8 pt-5 text-center text-sm text-gray-500 px-6">

        © 2026 LINKBT • Powered by Intellisoft Technology

      </div>

    </footer>
  );
};

export default Footer;