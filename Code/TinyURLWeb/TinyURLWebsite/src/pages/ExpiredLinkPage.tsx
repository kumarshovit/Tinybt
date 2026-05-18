import { Link } from "react-router-dom";
import { AlertTriangle } from "lucide-react";

const ExpiredLinkPage = () => {

    return (

        <div className="min-h-screen bg-gradient-to-br from-gray-100 to-blue-50 flex items-center justify-center px-6">

            <div className="max-w-lg w-full bg-white rounded-3xl shadow-2xl border border-gray-200 p-10 text-center">

                <div className="flex justify-center mb-6">

                    <div className="bg-red-100 p-5 rounded-full">
                        <AlertTriangle
                            className="text-red-600"
                            size={50}
                        />
                    </div>

                </div>

                <h1 className="text-4xl font-bold text-gray-900 mb-4">

                    Link Expired

                </h1>

                <p className="text-gray-600 leading-7 text-lg">

                    This short URL has expired or is no longer available.

                    Please contact the owner or create a new short link.

                </p>

                <div className="flex flex-col sm:flex-row gap-4 mt-10">

                    <Link
                        to={localStorage.getItem("token")
                            ? "/dashboard"
                            : "/"}
                        className="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-xl font-semibold transition"
                    >
                        Go Home
                    </Link>

                    <Link
                        to="/contact"
                        className="flex-1 border border-gray-300 hover:bg-gray-100 py-3 rounded-xl font-semibold transition"
                    >
                        Contact Support
                    </Link>

                </div>

            </div>

        </div>
    );
};

export default ExpiredLinkPage;