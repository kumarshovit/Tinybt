import { useEffect, useState } from "react";
import axios from "axios";
import Navbar from "../components/Navbar";

interface Profile {
  email: string;
  fullName: string;
  createdAt: string;
}

const ProfilePage = () => {
  const token = localStorage.getItem("token");

  const [profile, setProfile] = useState<Profile | null>(null);
  const [fullName, setFullName] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7025/api/auth/profile",
        {
          headers: { Authorization: `Bearer ${token}` },
        },
      );

      setProfile(response.data);
      setFullName(response.data.fullName || "");
    } catch (err: any) {
      setError("Failed to load profile.");
    }
  };

  const handleUpdateName = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");
    setError("");
    setLoading(true);

    try {
      await axios.put(
        "https://localhost:7025/api/auth/update-profile",
        {
          fullName: fullName,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        },
      );

      setMessage("Profile updated successfully!");
    } catch (err: any) {
      if (typeof err.response?.data === "string") {
        setError(err.response.data);
      } else if (err.response?.data?.title) {
        setError(err.response.data.title);
      } else {
        setError("Failed to update profile.");
      }
    }

    setLoading(false);
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");
    setError("");

    if (!currentPassword || !newPassword) {
      setError("Please fill all password fields.");
      return;
    }

    setLoading(true);

    try {
      await axios.put(
        "https://localhost:7025/api/auth/change-password",
        {
          currentPassword,
          newPassword,
        },
        {
          headers: { Authorization: `Bearer ${token}` },
        },
      );

      setMessage("Password changed successfully!");

      // Clear fields after success
      setCurrentPassword("");
      setNewPassword("");
    } catch (err: any) {
      if (typeof err.response?.data === "string") {
        setError(err.response.data);
      } else if (err.response?.data?.title) {
        setError(err.response.data.title);
      } else {
        setError("Failed to change password.");
      }
    }

    setLoading(false);
  };

  return (
    <>
      <Navbar />

      <div className="min-h-screen bg-gray-50 flex justify-center py-10 px-4">
        <div className="bg-white w-full max-w-md p-8 rounded-2xl shadow-xl">
          <h1 className="text-3xl font-bold mb-6 text-center">My Profile 👤</h1>

          {profile && (
            <div className="mb-6 text-sm text-gray-600">
              <p>
                <strong>Email:</strong> {profile.email}
              </p>
              <p>
                <strong>Account Created:</strong>{" "}
                {new Date(profile.createdAt).toLocaleDateString()}
              </p>
            </div>
          )}

          {/* Success Message */}
          {message && (
            <p className="text-green-600 text-sm mb-4 text-center">{message}</p>
          )}

          {/* Error Message */}
          {error && (
            <p className="text-red-500 text-sm mb-4 text-center">{error}</p>
          )}

          {/* Update Name Form */}
          <form onSubmit={handleUpdateName} className="mb-6">
            <label className="block text-sm font-medium mb-1">Full Name</label>

            <input
              type="text"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              className="w-full border px-4 py-2 rounded-lg focus:ring-2 focus:ring-indigo-500"
            />

            <button
              type="submit"
              disabled={loading}
              className="w-full mt-3 bg-indigo-600 text-white py-2 rounded-lg hover:bg-indigo-700 transition disabled:opacity-60"
            >
              {loading ? "Updating..." : "Update Name"}
            </button>
          </form>

          {/* Change Password Form */}
          <form onSubmit={handleChangePassword}>
            <label className="block text-sm font-medium mb-1">
              Current Password
            </label>

            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="w-full border px-4 py-2 rounded-lg mb-3 focus:ring-2 focus:ring-indigo-500"
            />

            <label className="block text-sm font-medium mb-1">
              New Password
            </label>

            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="w-full border px-4 py-2 rounded-lg"
            />

            <button
              type="submit"
              disabled={loading}
              className="w-full mt-3 bg-red-600 text-white py-2 rounded-lg hover:bg-red-700 transition disabled:opacity-60"
            >
              {loading ? "Changing..." : "Change Password"}
            </button>
          </form>
        </div>
      </div>
    </>
  );
};

export default ProfilePage;
