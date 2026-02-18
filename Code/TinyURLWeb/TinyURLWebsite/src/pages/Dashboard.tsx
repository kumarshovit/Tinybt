import Sidebar from "../components/Sidebar";
import StatCard from "../components/StatCard";
import LinkTable from "../components/LinkTable";

const Dashboard = () => {
  const links = [
    {
      shortUrl: "tinybt.io/abc123",
      longUrl: "https://example.com/very-long-link",
      clicks: 23,
    },
    {
      shortUrl: "tinybt.io/xyz789",
      longUrl: "https://google.com/search?q=tinybt",
      clicks: 55,
    },
  ];

  return (
    <div className="flex">
      <Sidebar />

      <div className="flex-1 p-8 bg-gray-100 min-h-screen">
        <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <StatCard title="Total Links" value="12" />
          <StatCard title="Total Clicks" value="250" />
          <StatCard title="Active Links" value="9" />
        </div>

        <LinkTable links={links} />
      </div>
    </div>
  );
};

export default Dashboard;
