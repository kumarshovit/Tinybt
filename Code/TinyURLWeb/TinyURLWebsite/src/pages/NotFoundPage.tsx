import { Link } from "react-router-dom";
import SEO from "../components/SEO";

export default function NotFoundPage() {
    return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-br from-blue-50 to-white px-6">

            <SEO
                title="Page Not Found – LinkBT"
                description="The page you are looking for does not exist or has been moved."
                noindex
            />

            {/* 404 Heading */}
            <h1 className="text-9xl font-extrabold text-blue-600 tracking-tight">
                404
            </h1>

            <h2 className="text-3xl font-bold text-gray-900 mt-4">
                Page Not Found
            </h2>

            <p className="text-gray-500 text-lg mt-3 text-center max-w-md">
                Sorry, the page you're looking for doesn't exist or has been moved.
            </p>

            <div className="flex gap-4 mt-8">
                <Link
                    to="/"
                    className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold transition"
                >
                    Go Home
                </Link>

                <Link
                    to="/contact"
                    className="border border-gray-300 hover:bg-gray-100 text-gray-700 px-6 py-3 rounded-lg font-semibold transition"
                >
                    Contact Us
                </Link>
            </div>
        </div>
    );
}
