import { Link } from "react-router-dom";

const NotFound = () => {
  return (
    <div className="min-h-screen flex flex-col justify-center items-center bg-gray-100">
      <h1 className="text-5xl font-bold text-gray-800">404</h1>
      <p className="text-gray-600 mt-4">Page Not Found</p>

      <Link
        to="/"
        className="mt-6 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg rounded-xl font-semibold"
      >
        Go Home
      </Link>
    </div>
  );
};

export default NotFound;
