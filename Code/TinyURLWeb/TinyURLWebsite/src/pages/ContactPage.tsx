import { useState } from "react";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";


import {
    Send,
    Mail,
    User,
    MessageSquare
} from "lucide-react";

import {
    createContactMessage
} from "../services/contactService";

const ContactPage = () => {

    const [formData, setFormData] = useState({
        name: "",
        email: "",
        subject: "",
        message: ""
    });

    const [loading, setLoading] = useState(false);

    const [success, setSuccess] = useState("");

    const [error, setError] = useState("");

    // 🔹 Handle Input Change
    const handleChange = (
        e: React.ChangeEvent<
            HTMLInputElement | HTMLTextAreaElement
        >
    ) => {

        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    // 🔹 Handle Submit
    const handleSubmit = async (
        e: React.FormEvent
    ) => {

        e.preventDefault();

        setLoading(true);

        setError("");

        setSuccess("");

        try {

            const response =
                await createContactMessage(formData);

            setSuccess(
                response.message ||
                "Message sent successfully."
            );

            setFormData({
                name: "",
                email: "",
                subject: "",
                message: ""
            });

        } catch (err: any) {

            setError(
                err?.response?.data?.message ||
                "Something went wrong."
            );

        } finally {

            setLoading(false);
        }
    };

    return (

        <div className="min-h-screen flex flex-col bg-gradient-to-br from-blue-50 to-white">

            {/* Navbar */}
            <Navbar />

            {/* Main Content */}
            <main className="flex-1 py-20 px-6">

                <div className="max-w-6xl mx-auto grid lg:grid-cols-2 gap-14 items-center">

                    {/* LEFT SECTION */}
                    <div>

                        <h1 className="text-5xl font-bold text-gray-900 leading-tight">

                            Contact Our Team

                        </h1>

                        <p className="mt-6 text-lg text-gray-600 leading-8">

                            Have questions, suggestions, or facing
                            issues with your short links?

                            Our team is here to help you anytime.

                        </p>

                        <div className="mt-10 space-y-6">

                            {/* Email Support */}
                            <div className="flex items-center gap-4 bg-white shadow-sm border border-gray-200 rounded-2xl p-5">

                                <div className="bg-blue-100 p-3 rounded-xl">

                                    <Mail className="text-blue-600" />

                                </div>

                                <div>

                                    <h3 className="font-semibold text-gray-800">

                                        Email Support

                                    </h3>

                                    <p className="text-gray-500 text-sm">

                                        support@linkbt.com

                                    </p>

                                </div>

                            </div>

                            {/* Fast Response */}
                            <div className="flex items-center gap-4 bg-white shadow-sm border border-gray-200 rounded-2xl p-5">

                                <div className="bg-green-100 p-3 rounded-xl">

                                    <MessageSquare className="text-green-600" />

                                </div>

                                <div>

                                    <h3 className="font-semibold text-gray-800">

                                        Fast Response

                                    </h3>

                                    <p className="text-gray-500 text-sm">

                                        Usually replies within 24 hours

                                    </p>

                                </div>

                            </div>

                        </div>

                    </div>

                    {/* RIGHT FORM SECTION */}
                    <div className="bg-white rounded-3xl shadow-2xl border border-gray-100 p-8 lg:p-10">

                        <h2 className="text-3xl font-bold text-gray-900 mb-8">

                            Send Message

                        </h2>

                        <form
                            onSubmit={handleSubmit}
                            className="space-y-6"
                        >

                            {/* NAME */}
                            <div>

                                <label className="text-sm font-medium text-gray-700 mb-2 block">

                                    Full Name

                                </label>

                                <div className="relative">

                                    <User
                                        className="absolute left-4 top-4 text-gray-400"
                                        size={20}
                                    />

                                    <input
                                        type="text"
                                        name="name"
                                        value={formData.name}
                                        onChange={handleChange}
                                        placeholder="Enter your full name"
                                        required
                                        className="w-full h-14 pl-12 pr-4 rounded-xl border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    />

                                </div>

                            </div>

                            {/* EMAIL */}
                            <div>

                                <label className="text-sm font-medium text-gray-700 mb-2 block">

                                    Email Address

                                </label>

                                <div className="relative">

                                    <Mail
                                        className="absolute left-4 top-4 text-gray-400"
                                        size={20}
                                    />

                                    <input
                                        type="email"
                                        name="email"
                                        value={formData.email}
                                        onChange={handleChange}
                                        placeholder="Enter your email"
                                        required
                                        className="w-full h-14 pl-12 pr-4 rounded-xl border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    />

                                </div>

                            </div>

                            {/* SUBJECT */}
                            <div>

                                <label className="text-sm font-medium text-gray-700 mb-2 block">

                                    Subject

                                </label>

                                <input
                                    type="text"
                                    name="subject"
                                    value={formData.subject}
                                    onChange={handleChange}
                                    placeholder="Enter subject"
                                    required
                                    className="w-full h-14 px-4 rounded-xl border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />

                            </div>

                            {/* MESSAGE */}
                            <div>

                                <label className="text-sm font-medium text-gray-700 mb-2 block">

                                    Message

                                </label>

                                <textarea
                                    name="message"
                                    rows={6}
                                    value={formData.message}
                                    onChange={handleChange}
                                    placeholder="Write your message here..."
                                    required
                                    className="w-full p-4 rounded-xl border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                                />

                            </div>

                            {/* ERROR */}
                            {error && (
                                <div className="bg-red-100 text-red-700 text-sm px-4 py-3 rounded-lg">

                                    {error}

                                </div>
                            )}

                            {/* SUCCESS */}
                            {success && (
                                <div className="bg-green-100 text-green-700 text-sm px-4 py-3 rounded-lg">

                                    {success}

                                </div>
                            )}

                            {/* BUTTON */}
                            <button
                                type="submit"
                                disabled={loading}
                                className="w-full h-14 rounded-xl bg-blue-600 hover:bg-blue-700 transition text-white font-semibold flex items-center justify-center gap-2 disabled:opacity-60"
                            >

                                <Send size={18} />

                                {loading
                                    ? "Sending..."
                                    : "Send Message"}

                            </button>

                        </form>

                    </div>

                </div>

            </main>

            {/* Footer */}
            <Footer />

        </div>
    );
};

export default ContactPage;