import Sidebar from "../components/Sidebar";

const Settings = () => {
  return (
    <div className="flex">
      <Sidebar />

      <div className="flex-1 p-8 bg-gray-100 min-h-screen">
        <h1 className="text-3xl font-bold mb-6">Settings</h1>

        <div className="bg-white shadow-md rounded-xl p-6 max-w-xl">
          <h2 className="text-xl font-bold mb-4">Profile Settings</h2>

          <input
            type="text"
            placeholder="Full Name"
            className="w-full p-3 border rounded-lg mb-4"
          />

          <input
            type="email"
            placeholder="Email"
            className="w-full p-3 border rounded-lg mb-4"
          />

          <button className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold">
            Update Profile
          </button>
        </div>
      </div>
    </div>
  );
};

export default Settings;
