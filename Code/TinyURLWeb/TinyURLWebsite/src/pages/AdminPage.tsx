import { useEffect, useState } from "react";
import api from "../utils/api";
import Navbar from "../components/Navbar";

import ActivityModal from "../components/ActivityModal";
import { getUserActivity } from "../api/activityService";
import type { Activity } from "../types/activity";

interface User {
  id: number;
  email: string;
  role: string;
}

const AdminPage = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  const [defaultExpiration, setDefaultExpiration] = useState<number | "">("");
  const [settingLoading, setSettingLoading] = useState(false);

  const [loading, setLoading] = useState(true);

  const [activities, setActivities] = useState<Activity[]>([]);
  const [showActivity, setShowActivity] = useState(false);

  useEffect(() => {
    fetchUsers();
    fetchDefaultExpiration();
  }, []);

  // ================= FETCH USERS =================

  const fetchUsers = async () => {
    try {
      setLoading(true);

      const response = await api.get("/admin/all-users");

      setUsers(response.data);
      setError("");
    } catch (err: any) {
      if (err.response?.status === 403) {
        setError("Access denied. Admin only.");
      } else if (err.response?.status === 401) {
        setError("Unauthorized. Please login again.");
      } else {
        setError("Failed to fetch users.");
      }
    } finally {
      setLoading(false);
    }
  };

  // ================= FETCH DEFAULT EXPIRATION =================

  const fetchDefaultExpiration = async () => {
    try {
      const res = await api.get("/admin/settings/expiration");
      setDefaultExpiration(res.data.defaultExpirationDays ?? "");
    } catch {
      console.log("Failed to fetch expiration setting");
    }
  };

  // ================= UPDATE EXPIRATION =================

  const updateDefaultExpiration = async () => {
    if (!defaultExpiration) return alert("Enter valid days");

    try {
      setSettingLoading(true);

      await api.put("/admin/settings/expiration", {
        defaultExpirationDays: Number(defaultExpiration),
      });

      alert("Default expiration updated successfully!");
    } catch {
      alert("Failed to update expiration.");
    } finally {
      setSettingLoading(false);
    }
  };

  // ================= UPDATE ROLE =================

  const updateRole = async (userId: number, newRole: string) => {
    try {
      await api.put("/admin/update-role", {
        userId,
        newRole,
      });

      fetchUsers();
    } catch (err: any) {
      if (err.response?.status === 403) {
        alert("Access denied.");
      } else {
        alert("Failed to update role.");
      }
    }
  };

  // ================= DELETE USER =================

  const deleteUser = async (userId: number) => {
    const confirmDelete = window.confirm(
      "Are you sure you want to delete this user?"
    );

    if (!confirmDelete) return;

    try {
      await api.delete(`/admin/delete-user/${userId}`);
      fetchUsers();
    } catch (err: any) {
      if (err.response?.status === 403) {
        alert("Access denied.");
      } else {
        alert("Failed to delete user.");
      }
    }
  };

  // ================= FETCH USER ACTIVITY =================

  const fetchUserActivity = async (userId: number) => {
    try {
      const data = await getUserActivity(userId);
      setActivities(data);
      setShowActivity(true);
    } catch {
      alert("Failed to fetch activity.");
    }
  };

  const filteredUsers = users.filter((user) =>
    user.email.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <Navbar />

      <div className="p-4 sm:p-6 lg:p-8 max-w-7xl mx-auto space-y-6">

        {/* PAGE TITLE */}

        <h1 className="text-2xl sm:text-3xl font-bold">
          Admin Dashboard 🔐
        </h1>

        {error && <p className="text-red-500">{error}</p>}

        {/* ================= ANALYTICS CARDS ================= */}

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">

          <div className="bg-white shadow rounded-lg p-4">
            <p className="text-gray-500 text-sm">Total Users</p>
            <p className="text-2xl font-bold">{users.length}</p>
          </div>

          <div className="bg-white shadow rounded-lg p-4">
            <p className="text-gray-500 text-sm">Admins</p>
            <p className="text-2xl font-bold">
              {users.filter((u) => u.role === "Admin").length}
            </p>
          </div>

          <div className="bg-white shadow rounded-lg p-4">
            <p className="text-gray-500 text-sm">Normal Users</p>
            <p className="text-2xl font-bold">
              {users.filter((u) => u.role === "User").length}
            </p>
          </div>

          <div className="bg-white shadow rounded-lg p-4">
            <p className="text-gray-500 text-sm">Search Results</p>
            <p className="text-2xl font-bold">{filteredUsers.length}</p>
          </div>

        </div>

        {/* ================= SYSTEM SETTINGS ================= */}

        <div className="bg-white shadow rounded-lg p-4 sm:p-6">

          <h2 className="text-lg sm:text-xl font-semibold mb-4">
            System Settings ⚙️
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 items-center">

            <p className="font-medium">
              Default expiration link
            </p>

            <input
              type="number"
              min={1}
              max={365}
              placeholder="Default expiration (days)"
              className="px-4 py-2 border rounded-lg w-full"
              value={defaultExpiration}
              onChange={(e) =>
                setDefaultExpiration(Number(e.target.value))
              }
            />

            <button
              onClick={updateDefaultExpiration}
              disabled={settingLoading}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition"
            >
              {settingLoading ? "Saving..." : "Save"}
            </button>

          </div>

        </div>

        {/* ================= SEARCH ================= */}

        <input
          type="text"
          placeholder="Search by email..."
          className="px-4 py-2 border rounded-lg w-full md:w-80"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        {/* ================= USER TABLE ================= */}

        <div className="overflow-x-auto">

          <table className="min-w-[600px] w-full bg-white shadow rounded-lg text-sm sm:text-base">

            <thead className="bg-indigo-600 text-white">

              <tr>
                <th className="py-3 px-4 text-left">Email</th>
                <th className="py-3 px-4 text-center">Role</th>
                <th className="py-3 px-4 text-center">Actions</th>
              </tr>

            </thead>

            <tbody>

              {/* LOADING */}

              {loading &&
                Array.from({ length: 5 }).map((_, i) => (

                  <tr key={i} className="animate-pulse border-b">

                    <td className="py-3 px-4">
                      <div className="h-4 bg-gray-200 rounded w-56"></div>
                    </td>

                    <td className="py-3 px-4 text-center">
                      <div className="h-4 bg-gray-200 rounded w-20 mx-auto"></div>
                    </td>

                    <td className="py-3 px-4">
                      <div className="flex justify-center gap-2">
                        <div className="h-6 bg-gray-200 rounded w-16"></div>
                        <div className="h-6 bg-gray-200 rounded w-16"></div>
                        <div className="h-6 bg-gray-200 rounded w-16"></div>
                      </div>
                    </td>

                  </tr>

                ))}

              {/* USERS */}

              {!loading &&
                filteredUsers.map((user) => (

                  <tr
                    key={user.id}
                    className="border-b hover:bg-gray-50 transition"
                  >

                    <td className="py-3 px-4 break-words max-w-[250px]">
                      {user.email}
                    </td>

                    <td className="py-3 px-4 text-center">
                      {user.role}
                    </td>

                    <td className="py-3 px-4">

                      <div className="flex flex-wrap md:flex-nowrap justify-center gap-2">

                        {user.role === "User" ? (

                          <button
                            onClick={() => updateRole(user.id, "Admin")}
                            className="bg-green-500 text-white px-3 py-1 rounded text-sm hover:bg-green-600"
                          >
                            Promote
                          </button>

                        ) : (

                          <button
                            onClick={() => updateRole(user.id, "User")}
                            className="bg-yellow-500 text-white px-3 py-1 rounded text-sm hover:bg-yellow-600"
                          >
                            Demote
                          </button>

                        )}

                        <button
                          onClick={() => deleteUser(user.id)}
                          className="bg-red-500 text-white px-3 py-1 rounded text-sm hover:bg-red-600"
                        >
                          Delete
                        </button>

                        <button
                          onClick={() => fetchUserActivity(user.id)}
                          className="bg-blue-500 text-white px-3 py-1 rounded text-sm hover:bg-blue-600"
                        >
                          Activity
                        </button>

                      </div>

                    </td>

                  </tr>

                ))}

              {!loading && filteredUsers.length === 0 && (

                <tr>
                  <td colSpan={3} className="py-4 text-gray-500 text-center">
                    No users found.
                  </td>
                </tr>

              )}

            </tbody>

          </table>

        </div>

      </div>

      {/* ACTIVITY MODAL */}

      {showActivity && (
        <ActivityModal
          activities={activities}
          onClose={() => setShowActivity(false)}
        />
      )}

    </>
  );
};

export default AdminPage;