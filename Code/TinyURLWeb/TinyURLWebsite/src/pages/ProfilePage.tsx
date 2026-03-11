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

  const [editingName, setEditingName] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      setLoading(true);

      const response = await api.get("/profile");

      setProfile(response.data);
      setFullName(response.data.fullName || "");
      await new Promise((resolve) => setTimeout(resolve, 100));


    } catch {
      setError("Failed to load profile.");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateName = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");
    setError("");

    if (!fullName.trim()) {
      setError("Full name is required.");
      return;
    }

    setLoading(true);

    try {
      await api.put("/profile/name", { fullName });
      setMessage("Profile updated successfully!");
      setEditingName(false);
      fetchProfile();
    } catch (err: any) {
      setError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        "Failed to update profile."
      );
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

    if (newPassword.length < 6) {
      setError("Password must be at least 6 characters.");
      return;
    }

    setLoading(true);

    try {
      await api.put("/profile/password", {
        currentPassword,
        newPassword,
      });

      setMessage("Password changed successfully!");
      setCurrentPassword("");
      setNewPassword("");
    } catch (err: any) {
      setError(
        err.response?.data?.message ||
        err.response?.data?.title ||
        "Failed to change password."
      );
    }

    setLoading(false);
  };

  const handleDeleteAccount = async () => {
    const confirmDelete = window.confirm(
      "Are you sure? This action is permanent and cannot be undone."
    );

    if (!confirmDelete) return;

    try {
      await api.delete("/profile");

      localStorage.removeItem("token");
      window.location.href = "/login";
    } catch {
      setError("Failed to delete account.");
    }
  };

  return (
    <>
      <Navbar />

      <div className="min-h-screen bg-gray-50 py-10 px-4">
        <div className="max-w-3xl mx-auto space-y-6">

          {/* Page Title */}
          <h1 className="text-3xl font-bold text-center">
            Account Settings
          </h1>
          {loading && (
            <div className="space-y-6">

              <div className="bg-white rounded-xl shadow p-6 flex items-center gap-4 animate-pulse">
                <div className="w-14 h-14 rounded-full bg-gray-200"></div>

                <div className="space-y-2">
                  <div className="h-4 bg-gray-200 rounded w-32"></div>
                  <div className="h-3 bg-gray-200 rounded w-48"></div>
                </div>
              </div>

              <div className="bg-white rounded-xl shadow p-6 space-y-3 animate-pulse">
                <div className="h-4 bg-gray-200 rounded w-40"></div>
                <div className="h-4 bg-gray-200 rounded w-64"></div>
                <div className="h-4 bg-gray-200 rounded w-56"></div>
              </div>

              <div className="bg-white rounded-xl shadow p-6 space-y-3 animate-pulse">
                <div className="h-4 bg-gray-200 rounded w-40"></div>
                <div className="h-10 bg-gray-200 rounded"></div>
                <div className="h-10 bg-gray-200 rounded"></div>
              </div>

            </div>
          )}

          {/* Messages */}
          {message && (
            <p className="text-green-600 text-center text-sm">
              {message}
            </p>
          )}

          {error && (
            <p className="text-red-500 text-center text-sm">
              {error}
            </p>
          )}

          {/* PROFILE HEADER */}
          {!loading && profile && (
            <div className="bg-white rounded-xl shadow p-6 flex items-center gap-4 hover:shadow-lg transition">
              <div className="w-14 h-14 rounded-full bg-indigo-600 text-white flex items-center justify-center text-xl font-bold">
                {profile.fullName?.charAt(0)}
              </div>

              <div>
                <p className="font-semibold text-lg">{profile.fullName}</p>
                <p className="text-sm text-gray-500">{profile.email}</p>
              </div>
            </div>
          )}

          {/* ACCOUNT INFO */}
          {!loading && profile && (
            <div className="bg-white rounded-xl shadow p-6 hover:shadow-lg transition">
              <h2 className="text-lg font-semibold mb-4">
                Account Information
              </h2>

              <p className="text-sm text-gray-600">
                <strong>Email:</strong> {profile.email}
              </p>

              <p className="text-sm text-gray-600 mt-1">
                <strong>Account Created:</strong>{" "}
                {new Date(profile.createdAt).toLocaleDateString()}
              </p>
            </div>
          )}

          {/* PERSONAL INFO */}
          <div className="bg-white rounded-xl shadow p-6 hover:shadow-lg transition">
            <h2 className="text-lg font-semibold mb-4">
              Personal Information
            </h2>

            {!editingName ? (
              <div className="flex justify-between items-center">
                <p className="text-gray-700">{fullName}</p>

                <button
                  onClick={() => setEditingName(true)}
                  className="text-indigo-600 text-sm hover:underline"
                >
                  Edit
                </button>
              </div>
            ) : (
              <form onSubmit={handleUpdateName} className="space-y-3">
                <input
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  className="w-full border px-4 py-2 rounded-lg"
                />

                <div className="flex gap-2">
                  <button
                    type="submit"
                    className="bg-indigo-600 text-white px-4 py-2 rounded-lg flex items-center gap-2"
                  >
                    {loading && (
                      <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
                    )}
                    Save
                  </button>

                  <button
                    type="button"
                    onClick={() => setEditingName(false)}
                    className="px-4 py-2 border rounded-lg"
                  >
                    Cancel
                  </button>
                </div>
              </form>
            )}
          </div>

          {/* SECURITY */}
          <div className="bg-white rounded-xl shadow p-6 hover:shadow-lg transition">
            <h2 className="text-lg font-semibold mb-4">
              Security
            </h2>

            <form onSubmit={handleChangePassword} className="space-y-3">

              <div>
                <label className="block text-sm font-medium mb-1">
                  Current Password
                </label>

                <input
                  type="password"
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                  className="w-full border px-4 py-2 rounded-lg"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">
                  New Password
                </label>

                <div className="flex gap-2">
                  <input
                    type={showPassword ? "text" : "password"}
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    className="w-full border px-4 py-2 rounded-lg"
                  />

                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="text-indigo-600 text-sm"
                  >
                    {showPassword ? "Hide" : "Show"}
                  </button>
                </div>
              </div>

              <button
                type="submit"
                disabled={loading}
                className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition"
              >
                {loading ? "Changing..." : "Change Password"}
              </button>

            </form>
          </div>

          {/* DANGER ZONE */}
          <div className="bg-red-50 border border-red-200 rounded-xl p-6">
            <h2 className="text-red-600 font-semibold mb-3">
              Danger Zone
            </h2>

            <p className="text-sm text-gray-600 mb-4">
              Permanently delete your account and all associated data.
              This action cannot be undone.
            </p>

            <button
              onClick={handleDeleteAccount}
              className="bg-black text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition"
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

