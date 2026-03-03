

import { useEffect, useState } from "react";
import { getDashboardOverview } from "../api/dashboardService";
import type { DashboardOverview } from "../api/dashboardService";

import AnalysisSidebar from "../components/analysis/AnalysisSidebar";
import ClicksOverTime from "../components/analysis/ClicksOverTime";
import ClicksByGeography from "../components/analysis/ClicksByGeography";
import ClicksByLanguage from "../components/analysis/ClicksByLanguage";
import PopularDaysTimes from "../components/analysis/PopularDaysTimes";
import ClicksByDevice from "../components/analysis/ClicksByDevice";
import ClicksByOS from "../components/analysis/ClicksByOS";
import ClicksByBrowser from "../components/analysis/ClicksByBrowser";
import Navbar from "../components/Navbar";

const AnalysisPage = () => {
  const [data, setData] = useState<DashboardOverview | null>(null);
  const [loading, setLoading] = useState(false);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [active, setActive] = useState("time");

  const loadDashboard = async () => {
    setLoading(true);
    try {
      const result = await getDashboardOverview(startDate, endDate);
      setData(result);
    } catch (error) {
      console.error("Failed to load dashboard", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDashboard();
  }, []);


  return (
  <div className="min-h-screen bg-gray-50 flex flex-col">
    
    {/* Top Navbar */}
    <Navbar />

    {/* Below Navbar: Sidebar + Content */}
    <div className="flex flex-1">
      
      {/* Sidebar */}
      <AnalysisSidebar active={active} setActive={setActive} />

      {/* Main Content */}
      <div className="flex-1 p-8">
        <h1 className="text-2xl font-bold mb-6">
          Link Performance Overview
        </h1>

        {/* Date Filter */}
        <div className="flex gap-4 mb-6">
          <input
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            className="border p-2 rounded"
          />
          <input
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            className="border p-2 rounded"
          />
          <button
            onClick={loadDashboard}
            className="bg-blue-600 text-white px-4 py-2 rounded"
          >
            Apply
          </button>
        </div>

        {loading && <p>Loading...</p>}

        {/* Summary Cards */}
        {data && (
          <div className="grid grid-cols-4 gap-6 mb-10">
            <div className="bg-white shadow rounded p-6">
              <h2 className="text-gray-500">Total Clicks</h2>
              <p className="text-2xl font-bold">{data.totalClicks}</p>
            </div>

            <div className="bg-white shadow rounded p-6">
              <h2 className="text-gray-500">Total Links</h2>
              <p className="text-2xl font-bold">{data.totalUrls}</p>
            </div>

            <div className="bg-white shadow rounded p-6">
              <h2 className="text-gray-500">Active Links</h2>
              <p className="text-2xl font-bold">{data.activeLinks}</p>
            </div>

            <div className="bg-white shadow rounded p-6">
              <h2 className="text-gray-500">Expired Links</h2>
              <p className="text-2xl font-bold">{data.expiredLinks}</p>
            </div>
          </div>
        )}

        {/* Analytics Section */}
        <div className="bg-white rounded shadow p-6">
          {active === "time" && (
            <ClicksOverTime startDate={startDate} endDate={endDate} />
          )}
          {active === "geo" && <ClicksByGeography />}
          {active === "lang" && <ClicksByLanguage />}
          {active === "popular" && <PopularDaysTimes />}
          {active === "device" && <ClicksByDevice />}
          {active === "os" && <ClicksByOS />}
          {active === "browser" && <ClicksByBrowser />}
        </div>
      </div>
    </div>
  </div>
);
}
export default AnalysisPage;