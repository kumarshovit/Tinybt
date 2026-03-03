import { useState } from "react";
import Navbar from "../components/Navbar";

interface AnalyticsItem {
  period: string;
  clicks: number;
}

const BASE_URL = "https://localhost:57679/api/analytics";

export default function MyAnalytics() {
  const [shortCode, setShortCode] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [viewType, setViewType] = useState("Daily");
  const [data, setData] = useState<AnalyticsItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const fetchAnalytics = async () => {
    if (!shortCode || !startDate || !endDate) {
      setError("All fields are required");
      return;
    }

    const token = localStorage.getItem("token");

    try {
      setLoading(true);
      setError("");
      setData([]);

      const response = await fetch(
        `${BASE_URL}/${shortCode}/clicks-over-time?startDate=${startDate}&endDate=${endDate}&viewType=${viewType}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Unable to fetch analytics");
      }

      const result = await response.json();
      setData(result);
    } catch (err: any) {
      setError(err.message || "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Navbar />

      <div className="min-h-screen bg-gray-100 py-10 px-4">
        <div className="max-w-6xl mx-auto">

          {/* Page Title */}
          <h1 className="text-4xl font-bold mb-8 text-gray-800">
            📊 My Link Analytics
          </h1>

          {/* Filter Card */}
          <div className="bg-white rounded-2xl shadow-md p-8 mb-8">
            <div className="grid md:grid-cols-4 gap-6">

              <input
                type="text"
                placeholder="Enter Short Code"
                value={shortCode}
                onChange={(e) => setShortCode(e.target.value)}
                className="border border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200 p-3 rounded-lg outline-none transition"
              />

              <input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="border border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200 p-3 rounded-lg outline-none transition"
              />

              <input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                className="border border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200 p-3 rounded-lg outline-none transition"
              />

              <select
                value={viewType}
                onChange={(e) => setViewType(e.target.value)}
                className="border border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200 p-3 rounded-lg outline-none transition"
              >
                <option value="Daily">Daily</option>
                <option value="Weekly">Weekly</option>
                <option value="Monthly">Monthly</option>
              </select>
            </div>

            <button
              onClick={fetchAnalytics}
              disabled={loading}
              className="mt-6 bg-gradient-to-r from-purple-600 to-indigo-600 hover:opacity-90 text-white px-8 py-3 rounded-lg font-semibold transition disabled:opacity-50"
            >
              {loading ? "Loading..." : "Get Analytics"}
            </button>

            {error && (
              <p className="text-red-500 mt-4 font-medium">{error}</p>
            )}
          </div>

          {/* Empty State */}
          {!loading && data.length === 0 && !error && (
            <div className="bg-white rounded-xl shadow-sm p-8 text-center text-gray-500">
              No analytics data available.
            </div>
          )}

          {/* Results Table */}
          {data.length > 0 && (
            <div className="bg-white rounded-2xl shadow-md overflow-hidden">
              <div className="p-6 border-b">
                <h2 className="text-2xl font-semibold text-gray-700">
                  Analytics Results
                </h2>
              </div>

              <table className="w-full text-left">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="p-4 font-semibold text-gray-600">
                      Period
                    </th>
                    <th className="p-4 font-semibold text-gray-600">
                      Clicks
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {data.map((item, index) => (
                    <tr
                      key={index}
                      className="border-t hover:bg-gray-50 transition"
                    >
                      <td className="p-4">{item.period}</td>
                      <td className="p-4 font-bold text-purple-600">
                        {item.clicks}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

        </div>
      </div>
    </>
  );
}