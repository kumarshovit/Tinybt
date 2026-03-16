import { Link, useNavigate } from "react-router-dom";
import { logoutUser } from "../services/authService";
import { getUserRole } from "../utils/auth";
import { BarChart3, Menu, X } from "lucide-react";
import { useState } from "react";
import logo from "../assets/logo.png"

const Navbar = () => {
  const navigate = useNavigate();
  const role = getUserRole();
  const [menuOpen, setMenuOpen] = useState(false);


  const handleLogout = async () => {
    await logoutUser(); // ✅ Call service
    navigate("/"); // ✅ Redirect
  };

  return (
    <nav className="bg-white shadow-md px-4 sm:px-6 lg:px-10 py-3">
      <div className="flex justify-between items-center">

        {/* Logo */}
        <Link to="/dashboard" className="flex items-center">
          <img
            src={logo}
            alt="LinkBt Logo"
            className="h-7 sm:h-9 md:h-10 w-auto object-contain"
          />
        </Link>
        {/* Hamburger Button (Mobile Only) */}
        <button
          className="md:hidden"
          onClick={() => setMenuOpen(!menuOpen)}
        >
          {menuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>

        {/* Desktop Menu */}
        <div className="hidden md:flex items-center gap-6">

          {role === "Admin" && (
            <Link
              to="/analysis"
              className="flex items-center gap-1 hover:text-blue-600"
            >
              <BarChart3 size={18} />
              Analysis
            </Link>
          )}

          <Link
            to="/dashboard"
            className="text-gray-700 hover:text-blue-600 transition"
          >
            Dashboard
          </Link>

          {role === "Admin" && (
            <Link
              to="/admin"
              className="text-gray-700 hover:text-blue-600 transition"
            >
              Admin Panel
            </Link>
          )}

          <Link
            to="/profile"
            className="text-gray-700 hover:text-blue-600 transition"
          >
            Profile
          </Link>

          {role === "User" && (
            <Link
              to="/my-analytics"
              className="text-gray-700 hover:text-purple-600"
            >
              My Analytics
            </Link>
          )}

          <button
            onClick={handleLogout}
            className="bg-blue-600 text-white px-4 py-1.5 rounded-lg hover:bg-blue-700 transition"
          >
            Logout
          </button>

        </div>
      </div>

      {/* Mobile Menu */}
      {menuOpen && (
        <div className="flex flex-col gap-4 mt-4 md:hidden">

          {role === "Admin" && (
            <Link
              to="/analysis"
              className="flex items-center gap-2"
            >
              <BarChart3 size={18} />
              Analysis
            </Link>
          )}

          <Link to="/dashboard">Dashboard</Link>

          {role === "Admin" && (
            <Link to="/admin">Admin Panel 🔐</Link>
          )}

          <Link to="/profile">Profile</Link>

          {role === "User" && (
            <Link to="/my-analytics">My Analytics 📊</Link>
          )}

          <button
            onClick={handleLogout}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg"
          >
            Logout
          </button>

        </div>
      )}
    </nav>
  );
};

export default Navbar;
