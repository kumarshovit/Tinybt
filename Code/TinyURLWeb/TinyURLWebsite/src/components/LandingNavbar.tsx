import { Link } from "react-router-dom";
import { Menu, X } from "lucide-react";
import { useState } from "react";
import logo from '../assets/logo.png'

export default function LandingNavbar() {

  const [open, setOpen] = useState(false);


  return (
    <nav className="bg-white shadow-sm sticky top-0 z-50">

      <div className="max-w-7xl mx-auto px-6 py-4 flex justify-between items-center">

        {/* Logo */}
        <Link to="/dashboard" className="flex items-center">
          <img
            src={logo}
            alt="LinkBt Logo"
            className="h-7 sm:h-9 md:h-10 w-auto object-contain"
          />
        </Link>

        {/* Desktop Menu */}
        <div className="hidden md:flex gap-6 items-center">

          <a href="#features" className="hover:text-blue-600">
            Features
          </a>

          <a href="#how" className="hover:text-blue-600">
            How it Works
          </a>

          <Link
            to="/login"
            className="hover:text-blue-600"
          >
            Login
          </Link>

          <Link
            to="/register"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
          >
            Sign Up
          </Link>

        </div>

        {/* Mobile Button */}
        <button
          className="md:hidden"
          onClick={() => setOpen(!open)}
        >
          {open ? <X size={24} /> : <Menu size={24} />}
        </button>

      </div>

      {/* Mobile Menu */}
      {open && (
        <div className="md:hidden flex flex-col gap-4 px-6 pb-6">

          <a href="#features">Features</a>
          <a href="#how">How it Works</a>

          <Link to="/login">Login</Link>

          <Link
            to="/register"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg"
          >
            Sign Up
          </Link>

        </div>
      )}

    </nav>
  );
}