import { Link, useNavigate, useLocation } from "react-router-dom";
import { logoutUser } from "../services/authService";
import { getUserRole } from "../utils/auth";
import { BarChart3, Menu, X } from "lucide-react";
import { useState } from "react";
import logo from "../assets/logo.png";

const Navbar = () => {

  const navigate = useNavigate();

  const location = useLocation();

  const token = localStorage.getItem("token");

  const role = getUserRole();

  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = async () => {

    await logoutUser();

    navigate("/");
  };

  // ✅ Active Link Style
  const navLinkClass = (path: string) =>
    `transition-colors ${
      location.pathname === path
        ? "text-blue-600 font-semibold"
        : "text-gray-700 hover:text-blue-600"
    }`;

  return (

    <nav className="bg-white shadow-md px-4 sm:px-6 lg:px-10 py-3">

      <div className="flex justify-between items-center">

        {/* Logo */}
        <Link
          to={token ? "/dashboard" : "/"}
          className="flex items-center"
        >
          <img
            src={logo}
            alt="LinkBt Logo"
            className="h-7 sm:h-9 md:h-10 w-auto object-contain"
          />
        </Link>

        {/* Mobile Button */}
        <button
          className="md:hidden"
          onClick={() => setMenuOpen(!menuOpen)}
        >
          {menuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>

        {/* Desktop Menu */}
        <div className="hidden md:flex items-center gap-6">

          {/* ================= BEFORE LOGIN ================= */}
          {!token ? (
            <>

              <Link
                to="/"
                className={navLinkClass("/")}
              >
                Home
              </Link>

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
                Register
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
                to="/"
                className={navLinkClass("/")}
              >
                Home
              </Link>

              <Link
                to="/login"
                className={navLinkClass("/login")}
              >
                Login
              </Link>

              <Link
                to="/register"
                className="bg-blue-600 text-white px-4 py-2 rounded-lg text-center"
              >
                Register
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