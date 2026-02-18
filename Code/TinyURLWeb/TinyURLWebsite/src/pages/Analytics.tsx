import Sidebar from "../components/Sidebar";

const Analytics = () => {
  return (
    <div className="flex">
      <Sidebar />

      <div className="flex-1 p-8 bg-gray-100 min-h-screen">
        <h1 className="text-3xl font-bold mb-6">Analytics</h1>

        <div className="bg-white shadow-md rounded-xl p-6">
          <p className="text-gray-700 font-medium">
            Analytics UI will show charts like clicks over time, device stats,
            country wise clicks etc.
          </p>
        </div>
      </div>
    </div>
  );
};

export default Analytics;
