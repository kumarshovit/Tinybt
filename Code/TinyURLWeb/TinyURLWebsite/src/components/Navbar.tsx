import { Link, useNavigate, useLocation } from "react-router-dom";
import { logoutUser } from "../services/authService";
import { getUserRole } from "../utils/auth";
import { BarChart3, Menu, X } from "lucide-react";
import { useState, useEffect } from "react";
import logo from "../assets/logo.png";

const Navbar = () => {

  const navigate = useNavigate();

  const location = useLocation();

  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  const token =
    typeof window === "undefined" || !mounted ? null : localStorage.getItem("token");

  const role = typeof window === "undefined" || !mounted ? null : getUserRole();

  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = async () => {

    await logoutUser();

    navigate("/");
  };

  // ✅ Active Link Style
  const isLanding = location.pathname === "/" || location.pathname === "/dashboard";

  const navLinkClass = (path: string) => {
    return `transition-all duration-300 hover:-translate-y-1 hover:scale-105 ${location.pathname === path
      ? "text-blue-600 font-semibold"
      : "text-gray-700 hover:text-blue-600"
      }`;
  };

  const defaultLinkClass = "text-gray-700 hover:text-blue-600 transition-all duration-300 hover:-translate-y-1 hover:scale-105";

  return (

    <nav className={`px-4 sm:px-6 lg:px-10 py-3 ${isLanding ? 'bg-white/95 backdrop-blur-md shadow-sm absolute top-0 w-full z-50' : 'bg-white shadow-md'}`}>

      <div className="flex justify-between items-center">

        {/* Logo */}
        <Link
          to={token ? "/dashboard" : "/"}
          className="flex items-center transition-transform duration-300 hover:scale-105 hover:-translate-y-1 block"
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
                to="/register"
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
                className={`flex items-center gap-1 ${navLinkClass("/analytics")}`}
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
                to="/register"
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
