import { Link, useLocation } from "react-router-dom";
import { logoutUser } from "../services/authService";
import { getUserRole, isTokenValid } from "../utils/auth";
import { BarChart3, Menu, X } from "lucide-react";
import { useState } from "react";
import logo from "../assets/logo.png";

const Navbar = () => {



  const location = useLocation();

  const authenticated = typeof window !== "undefined" ? isTokenValid() : false;
  const token = authenticated ? localStorage.getItem("token") : null;
  const role = authenticated ? getUserRole() : null;

  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = async () => {
    try {
      await logoutUser();
    } catch (err) {
      console.error('Logout error:', err);
    }
    window.location.href = '/login';
  };

  // ✅ Active Link Style
  const isLanding = location.pathname === "/" || location.pathname === "/dashboard";

  const navLinkClass = (path: string) => {
    return `transition-colors duration-200 font-medium ${location.pathname === path
      ? "text-blue-600"
      : "text-gray-700 hover:text-blue-600"
      }`;
  };

  const defaultLinkClass = "text-gray-700 hover:text-blue-600 transition-colors duration-200 font-medium";

  return (

    <nav className={`px-4 sm:px-6 lg:px-10 py-3 ${isLanding ? 'bg-white/95 backdrop-blur-md shadow-sm absolute top-0 w-full z-50' : 'bg-white shadow-md'}`}>

      <div className="flex justify-between items-center">

        {/* Logo */}
        <Link
          to={token ? "/dashboard" : "/"}
          className="flex items-center transition-opacity duration-200 hover:opacity-80 block"
        >
          <img
            src={logo}
            alt="LinkBT"
            width={768}
            height={260}
            className="h-7 sm:h-9 md:h-10 w-auto object-contain"
          />
        </Link>

        {/* Mobile Button */}
        <button
          className="md:hidden"
          aria-label={menuOpen ? "Close navigation menu" : "Open navigation menu"}
          aria-expanded={menuOpen}
          onClick={() => setMenuOpen(!menuOpen)}
        >
          {menuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>

        {/* Desktop Menu */}
        <div className="hidden md:flex items-center gap-6">

          {/* ================= BEFORE LOGIN ================= */}
          {/* ================= BEFORE LOGIN ================= */}
          {!token ? (
            <>
              <Link
                to="/tools/"
                className={navLinkClass("/tools/")}
              >
                Tools
              </Link>

              <Link
                to="/docs/"
                className={navLinkClass("/docs/")}
              >
                Docs
              </Link>

              <Link
                to="/pricing"
                className={navLinkClass("/pricing")}
              >
                Pricing
              </Link>

              <a
                href="/#features"
                className={defaultLinkClass}
              >
                Features
              </a>

              <a
                href="/#how"
                className={defaultLinkClass}
              >
                How it Works
              </a>

              <Link
                to="/login"
                className={navLinkClass("/login")}
              >
                Login
              </Link>

              <Link
                to="/login"
                className="bg-blue-600 text-white px-4 py-1.5 rounded-lg hover:bg-blue-700 transition"
              >
                Sign Up
              </Link>
            </>
          ) : (

            /* ================= AFTER LOGIN ================= */
            <>
              <Link
                to="/dashboard"
                className={navLinkClass("/dashboard")}
              >
                Dashboard
              </Link>
              
              <Link
                to="/analytics"
                className={`flex items-center gap-1 ${navLinkClass("/analytics")}`}
              >
                <BarChart3 size={18} />
                Analytics
              </Link>

              {role === "Admin" && (
                <Link
                  to="/admin"
                  className={navLinkClass("/admin")}
                >
                  Admin Panel
                </Link>
              )}

              <Link
                to="/tools/"
                className={navLinkClass("/tools/")}
              >
                Tools
              </Link>

              <Link
                to="/docs/"
                className={navLinkClass("/docs/")}
              >
                Docs
              </Link>

              <Link
                to="/developer"
                className={navLinkClass("/developer")}
              >
                Developer API
              </Link>

              <Link
                to="/profile"
                className={navLinkClass("/profile")}
              >
                Profile
              </Link>

              <button
                onClick={handleLogout}
                className="bg-blue-600 text-white px-4 py-1.5 rounded-lg hover:bg-blue-700 transition"
              >
                Logout
              </button>

            </>
          )}

        </div>

      </div>

      {/* ================= MOBILE MENU ================= */}
      {menuOpen && (

        <div className="flex flex-col gap-4 mt-4 md:hidden">

          {/* ================= BEFORE LOGIN ================= */}
          {!token ? (
            <>
              <Link
                to="/tools/"
                className={navLinkClass("/tools/")}
              >
                Tools
              </Link>

              <Link
                to="/docs/"
                className={navLinkClass("/docs/")}
              >
                Docs
              </Link>

              <Link
                to="/pricing"
                className={navLinkClass("/pricing")}
              >
                Pricing
              </Link>

              <a
                href="/#features"
                className={defaultLinkClass}
              >
                Features
              </a>

              <a
                href="/#how"
                className={defaultLinkClass}
              >
                How it Works
              </a>

              <Link
                to="/login"
                className={navLinkClass("/login")}
              >
                Login
              </Link>

              <Link
                to="/login"
                className="bg-blue-600 text-white px-4 py-1.5 rounded-lg hover:bg-blue-700 transition"
              >
                Sign Up
              </Link>
            </>
          ) : (

            /* ================= AFTER LOGIN ================= */
            <>

              <Link
                to="/analytics"
                className={`flex items-center gap-2 ${navLinkClass("/analytics")}`}
              >
                <BarChart3 size={18} />
                Analytics
              </Link>

              <Link
                to="/dashboard"
                className={navLinkClass("/dashboard")}
              >
                Dashboard
              </Link>

              {role === "Admin" && (
                <Link
                  to="/admin"
                  className={navLinkClass("/admin")}
                >
                  Admin Panel
                </Link>
              )}

              <Link
                to="/tools/"
                className={navLinkClass("/tools/")}
              >
                Tools
              </Link>

              <Link
                to="/docs/"
                className={navLinkClass("/docs/")}
              >
                Docs
              </Link>

              <Link
                to="/developer"
                className={navLinkClass("/developer")}
              >
                Developer API
              </Link>

              <Link
                to="/profile"
                className={navLinkClass("/profile")}
              >
                Profile
              </Link>

              <button
                onClick={handleLogout}
                className="bg-blue-600 text-white px-4 py-2 rounded-lg"
              >
                Logout
              </button>

            </>
          )}

        </div>
      )}

    </nav>
  );
};

export default Navbar;
