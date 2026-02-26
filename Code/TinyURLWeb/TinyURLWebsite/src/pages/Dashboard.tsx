import Navbar from "../components/Navbar";

const Dashboard = () => {
  return (
    <>
      <Navbar />

      <div className="min-h-screen flex items-center justify-center bg-gray-100">
        <h2 className="text-3xl font-bold">
          Welcome to Dashboard 🎉
        </h2>
      </div>
    </>
  );
};

export default Dashboard;