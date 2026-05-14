import { Link, useNavigate } from "react-router-dom";

const Footer = () => {

  const navigate = useNavigate();

  // ✅ Check login token
  const token = localStorage.getItem("token");

  // ✅ Logout handler
  const handleLogout = () => {

    localStorage.removeItem("token");

    navigate("/");
  };

  return (

    <footer className="bg-[#07142B] text-gray-300 pt-12 pb-6 mt-20">

      <div className="max-w-7xl mx-auto px-6 grid grid-cols-1 md:grid-cols-3 gap-10">

        {/* Brand */}
        <div>

          <h2 className="text-2xl font-bold text-white mb-4">
            LINKBT
          </h2>

          <p className="text-sm leading-6 text-gray-400">
            Smart URL shortening platform to create,
            manage, and track your short links securely.
          </p>

          <p className="text-sm text-gray-500 mt-4">
            Secure • Fast • Simple
          </p>

        </div>

        {/* Navigation */}
        <div>

          <h3 className="text-white font-semibold mb-4">
            Navigation
          </h3>

          <ul className="space-y-3 text-sm">

            {/* ✅ Before Login */}
            {!token && (
              <>
                <li>
                  <Link
                    to="/"
                    className="hover:text-white transition"
                  >
                    Home
                  </Link>
                </li>

                <li>
                  <Link
                    to="/login"
                    className="hover:text-white transition"
                  >
                    Login
                  </Link>
                </li>

                <li>
                  <Link
                    to="/register"
                    className="hover:text-white transition"
                  >
                    Register
                  </Link>
                </li>
              </>
            )}

            {/* ✅ After Login */}
            {token && (
              <>
                <li>
                  <Link
                    to="/dashboard"
                    className="hover:text-white transition"
                  >
                    Dashboard
                  </Link>
                </li>

                <li>
                  <Link
                    to="/profile"
                    className="hover:text-white transition"
                  >
                    Profile
                  </Link>
                </li>

                <li>

                  <button
                    onClick={handleLogout}
                    className="hover:text-white transition"
                  >
                    Logout
                  </button>

                </li>
              </>
            )}

          </ul>

        </div>

        {/* Support */}
        <div>

          <h3 className="text-white font-semibold mb-4">
            Support
          </h3>

          <ul className="space-y-3 text-sm">

            <li className="hover:text-white transition cursor-pointer">
              Contact Us
            </li>

            <li className="hover:text-white transition cursor-pointer">
              Privacy Policy
            </li>

            <li className="hover:text-white transition cursor-pointer">
              Terms & Conditions
            </li>

          </ul>

          <p className="text-sm text-gray-500 mt-4">
           
          </p>

        </div>

      </div>

      {/* Bottom */}
      <div className="border-t border-gray-800 mt-10 pt-5 text-center text-sm text-gray-500 px-6">

      

      </div>

    </footer>
  );
};

export default Footer;