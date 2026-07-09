// import { Link } from "react-router-dom";

// const Footer = () => {

//   return (

//     <footer className="bg-[#07142B] text-gray-300 pt-10 pb-6 mt-20">

//       <div className="max-w-7xl mx-auto px-6 flex flex-col md:flex-row justify-between items-center gap-8">

//         {/* Brand */}
//         <div>

//           <h2 className="text-2xl font-bold text-white mb-3">
//             LINKBT
//           </h2>

//           <p className="text-sm text-gray-400 max-w-md leading-6">

//             Smart URL shortening platform to create,
//             manage, and track your short links securely.

//           </p>

//         </div>

//         {/* Quick Links */}
//         <div className="flex flex-wrap items-center gap-6 text-sm">

//           <Link
//             to="/contact"
//             className="hover:text-white transition"
//           >
//             Contact Us
//           </Link>

//           <Link
//             to="/terms"
//             className="hover:text-white transition"
//           >
//             Terms & Conditions
//           </Link>

//           <Link
//             to="/privacy-policy"
//             className="hover:text-white transition"
//           >
//             Privacy Policy
//           </Link>
//         </div>

//       </div>

//       {/* Bottom */}
//       <div className="border-t border-gray-800 mt-8 pt-5 text-center text-sm text-gray-500 px-6">

//         © 2026 LINKBT • Powered by Intellisoft Technology

//       </div>

//     </footer>
//   );
// };

// export default Footer;


import { Link } from "react-router-dom";

const Footer = () => {
  return (
    <footer className="bg-[#07142B] text-gray-300 pt-10 pb-6 mt-20">
      <div className="max-w-7xl mx-auto px-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-10">

          {/* Brand */}
          <div>
            <h2 className="text-2xl font-bold text-white mb-3">
              LINKBT
            </h2>

            <p className="text-sm text-gray-400 leading-6 max-w-sm">
              Smart URL shortening platform to create,
              manage, and track your short links securely.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="text-lg font-semibold text-white mb-4">
              Quick Links
            </h3>

            <div className="flex flex-col gap-3 text-sm">
              <Link
                to="/contact"
                className="hover:text-white transition"
              >
                Contact Us
              </Link>
              <Link
                to="/faq"
                className="hover:text-white transition"
              >
                FAQ
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

          {/* Contact Information */}
          <div>
            <h3 className="text-lg font-semibold text-white mb-4">
              Contact Information
            </h3>

            <div className="space-y-4 text-sm text-gray-400 leading-6">
              <div>
                <span className="font-medium text-white">📍 Address</span>
                <br />
                Demo Office Address<br />
                Sector 62, Noida,<br />
                Uttar Pradesh 201309, India
              </div>

              <div>
                <span className="font-medium text-white">📞 Phone</span>
                <br />
                + 975 17701770
              </div>

              <div>
                <span className="font-medium text-white">✉️ Email</span>
                <br />
                norbu@nano.bt
              </div>
            </div>
          </div>

        </div>

        {/* Bottom */}
        <div className="border-t border-gray-800 mt-8 pt-5 text-center text-sm text-gray-500">
          © 2026 LINKBT • All Rights Reserved.
          <br />
          Powered by <span className="text-gray-300 font-medium">Intellisoft Technology</span>
        </div>
      </div>
    </footer>
  );
};

export default Footer;