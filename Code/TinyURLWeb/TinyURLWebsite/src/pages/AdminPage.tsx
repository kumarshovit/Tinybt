


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

  // ACTIVITY STATES
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
      }

      else if (err.response?.status === 401) {
        setError("Unauthorized. Please login again.");
      }

      else {
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
      }

      else {
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
      }

      else {
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

      <div className="p-6">

        <h1 className="text-3xl font-bold mb-6">
          Admin Dashboard 🔐
        </h1>

        {error && (
          <p className="text-red-500 mb-4">{error}</p>
        )}

        {/* ================= SYSTEM SETTINGS ================= */}

        <div className="bg-white shadow rounded-lg p-6 mb-8">

          <h2 className="text-xl font-semibold mb-4">
            System Settings ⚙️
          </h2>

          <div className="flex items-center gap-4">

            <p>Default expiration link</p>

            <input
              type="number"
              min={1}
              max={365}
              placeholder="Default expiration (days)"
              className="px-4 py-2 border rounded-lg w-60"
              value={defaultExpiration}
              onChange={(e) => setDefaultExpiration(Number(e.target.value))}
            />

            <button
              onClick={updateDefaultExpiration}
              disabled={settingLoading}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg"
            >
              {settingLoading ? "Saving..." : "Save"}
            </button>

          </div>

        </div>


        {/* ================= SEARCH ================= */}

        <input
          type="text"
          placeholder="Search by email..."
          className="mb-4 px-4 py-2 border rounded-lg w-full max-w-sm"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />


        {/* ================= USER TABLE ================= */}

        <div className="overflow-x-auto">

          <table className="min-w-full bg-white shadow rounded-lg">

            <thead className="bg-indigo-600 text-white">

              <tr>
                <th className="py-2 px-4">Email</th>
                <th className="py-2 px-4">Role</th>
                <th className="py-2 px-4">Actions</th>
              </tr>

            </thead>

            <tbody>

              {/* LOADING SKELETON */}

              {loading &&
                Array.from({ length: 5 }).map((_, i) => (

                  <tr key={i} className="animate-pulse border-b text-center">

                    <td className="py-3 px-4">
                      <div className="h-4 bg-gray-200 rounded w-56 mx-auto"></div>
                    </td>

                    <td className="py-3 px-4">
                      <div className="h-4 bg-gray-200 rounded w-20 mx-auto"></div>
                    </td>

                    <td className="py-3 px-4 flex justify-center gap-2">
                      <div className="h-6 bg-gray-200 rounded w-16"></div>
                      <div className="h-6 bg-gray-200 rounded w-16"></div>
                      <div className="h-6 bg-gray-200 rounded w-16"></div>
                    </td>

                  </tr>

                ))}


              {/* REAL USERS */}

              {!loading &&

                filteredUsers.map((user) => (

                  <tr
                    key={user.id}
                    className="text-center border-b hover:bg-gray-50 transition"
                  >

                    <td className="py-2 px-4">{user.email}</td>

                    <td className="py-2 px-4">{user.role}</td>

                    <td className="py-2 px-4 space-x-2">

                      {user.role === "User" ? (

                        <button
                          onClick={() => updateRole(user.id, "Admin")}
                          className="bg-green-500 text-white px-3 py-1 rounded"
                        >
                          Promote
                        </button>

                      ) : (

                        <button
                          onClick={() => updateRole(user.id, "User")}
                          className="bg-yellow-500 text-white px-3 py-1 rounded"
                        >
                          Demote
                        </button>

                      )}

                      <button
                        onClick={() => deleteUser(user.id)}
                        className="bg-red-500 text-white px-3 py-1 rounded"
                      >
                        Delete
                      </button>

                      <button
                        onClick={() => fetchUserActivity(user.id)}
                        className="bg-blue-500 text-white px-3 py-1 rounded"
                      >
                        Activity
                      </button>

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