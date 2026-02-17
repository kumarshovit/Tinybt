import { useEffect, useState } from "react";
import api from "../utils/api";
import Navbar from "../components/Navbar";

interface Profile {
  email: string;
  fullName: string;
  createdAt: string;
}

const ProfilePage = () => {
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

  // ================= FETCH PROFILE =================
  const fetchProfile = async () => {
    try {
      const response = await api.get("/auth/profile");
      setProfile(response.data);
      setFullName(response.data.fullName || "");
    } catch {
      setError("Failed to load profile.");
    }
  };

  // ================= UPDATE NAME =================
  const handleUpdateName = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");
    setError("");
    setLoading(true);

    try {
      await api.put("/auth/update-profile", { fullName });
      setMessage("Profile updated successfully!");
      fetchProfile();
    } catch (err: any) {
      setError(
        err.response?.data?.title ||
        err.response?.data ||
        "Failed to update profile."
      );
    }

    setLoading(false);
  };

  // ================= CHANGE PASSWORD =================
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
      await api.put("/auth/change-password", {
        currentPassword,
        newPassword,
      });

      setMessage("Password changed successfully!");
      setCurrentPassword("");
      setNewPassword("");
    } catch (err: any) {
      setError(
        err.response?.data?.title ||
        err.response?.data ||
        "Failed to change password."
      );
    }

    setLoading(false);
  };

  // ================= DELETE ACCOUNT =================
  const handleDeleteAccount = async () => {
    const confirmDelete = window.confirm(
      "Are you sure? This action is permanent and cannot be undone."
    );

    if (!confirmDelete) return;

    try {
      await api.delete("/auth/delete-account");

      localStorage.removeItem("token");
      window.location.href = "/login";
    } catch {
      setError("Failed to delete account.");
    }
  };

  return (
    <>
      <Navbar />

      <div className="min-h-screen bg-gray-50 flex justify-center py-10 px-4">
        <div className="bg-white w-full max-w-md p-8 rounded-2xl shadow-xl">
          <h1 className="text-3xl font-bold mb-6 text-center">
            My Profile 👤
          </h1>

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

          {message && (
            <p className="text-green-600 text-sm mb-4 text-center">
              {message}
            </p>
          )}

          {error && (
            <p className="text-red-500 text-sm mb-4 text-center">
              {error}
            </p>
          )}

          {/* UPDATE NAME */}
          <form onSubmit={handleUpdateName} className="mb-6">
            <label className="block text-sm font-medium mb-1">
              Full Name
            </label>

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

          {/* CHANGE PASSWORD */}
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

          {/* DELETE ACCOUNT */}
          <div className="mt-8 border-t pt-6">
            <button
              type="button"
              onClick={handleDeleteAccount}
              className="w-full bg-black text-white py-2 rounded-lg hover:bg-gray-800 transition"
            >
              Delete My Account
            </button>
          </div>
        </div>
      </div>
    </>
  );
};

export default ProfilePage;
