import { Link, useNavigate } from "react-router-dom";
import api from "../utils/api"
import { getUserRole } from "../utils/auth";

const Navbar = () => {
  const navigate = useNavigate();
  const role = getUserRole();
  const token = localStorage.getItem("token");

  const handleLogout = async () => {
    try {
      await api.post(
        "https://localhost:7025/api/auth/logout",
        {},
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
    } catch (error) {
      console.log("Logout error:", error);
    }

    // Remove token regardless
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav className="bg-white shadow-md px-6 py-4 flex justify-between items-center">
      {/* Logo */}
      <h1 className="text-xl font-bold text-indigo-600">
        TinyURL 🚀
      </h1>

      {/* Links */}
      <div className="flex items-center gap-6">
        <Link
          to="/dashboard"
          className="text-gray-700 hover:text-indigo-600 transition"
        >
          Dashboard
        </Link>

        {role === "Admin" && (
          <Link
            to="/admin"
            className="text-gray-700 hover:text-indigo-600 transition"
          >
            Admin Panel 🔐
          </Link>
        )}

        <Link
          to="/profile"
          className="text-gray-700 hover:text-indigo-600 transition"
        >
          Profile
        </Link>

        <button
          onClick={handleLogout}
          className="bg-indigo-600 text-white px-4 py-1.5 rounded-lg hover:bg-indigo-700 transition"
        >
          Logout
        </button>
      </div>
    </nav>
  );
};

export default Navbar;
