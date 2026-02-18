import { Link, useLocation } from "react-router-dom";

const Sidebar = () => {
  const location = useLocation();

  const menu = [
    { name: "Dashboard", path: "/dashboard" },
    { name: "Shorten Link", path: "/shorten" },
    { name: "My Links", path: "/mylinks" },
    { name: "Analytics", path: "/analytics" },
    { name: "Settings", path: "/settings" },
  ];

  return (
    <div className="w-64 min-h-screen bg-gray-900 text-white flex flex-col p-6">
      <h2 className="text-2xl font-bold text-blue-400 mb-8">TinyBT</h2>

      <div className="flex flex-col gap-4">
        {menu.map((item) => (
          <Link
            key={item.path}
            to={item.path}
            className={`p-3 rounded-lg ${
              location.pathname === item.path
                ? "bg-blue-600"
                : "hover:bg-gray-700"
            }`}
          >
            {item.name}
          </Link>
        ))}
      </div>

      <button
        onClick={() => {
          localStorage.removeItem("token");
          window.location.href = "/login";
        }}
        className="mt-auto bg-red-600 hover:bg-red-700 py-2 rounded-lg"
      >
        Logout
      </button>
    </div>
  );
};

export default Sidebar;
