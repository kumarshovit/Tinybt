import { useEffect, useState } from "react";
import api from "../utils/api";
import Navbar from "../components/Navbar";

interface User {
  id: number;
  email: string;
  role: string;
}

const AdminPage = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    fetchUsers();
  }, []);

  // ================= FETCH USERS =================
  const fetchUsers = async () => {
    try {
      const response = await api.get("/auth/all-users");
      setUsers(response.data);
    } catch {
      setError("Failed to fetch users.");
    }
  };

  // ================= UPDATE ROLE =================
  const updateRole = async (userId: number, newRole: string) => {
    try {
      await api.put(
        `/auth/update-role?userId=${userId}&newRole=${newRole}`
      );
      fetchUsers();
    } catch {
      alert("Failed to update role.");
    }
  };

  // ================= DELETE USER =================
  const deleteUser = async (userId: number) => {
    const confirmDelete = window.confirm(
      "Are you sure you want to delete this user?"
    );

    if (!confirmDelete) return;

    try {
      await api.delete(`/auth/delete-user/${userId}`);
      fetchUsers();
    } catch {
      alert("Failed to delete user.");
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

        {/* Search */}
        <input
          type="text"
          placeholder="Search by email..."
          className="mb-4 px-4 py-2 border rounded-lg w-full max-w-sm"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

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
              {filteredUsers.map((user) => (
                <tr key={user.id} className="text-center border-b">
                  <td className="py-2 px-4">{user.email}</td>
                  <td className="py-2 px-4">{user.role}</td>

                  <td className="py-2 px-4 space-x-2">
                    {user.role === "User" ? (
                      <button
                        onClick={() =>
                          updateRole(user.id, "Admin")
                        }
                        className="bg-green-500 text-white px-3 py-1 rounded"
                      >
                        Promote
                      </button>
                    ) : (
                      <button
                        onClick={() =>
                          updateRole(user.id, "User")
                        }
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
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
};

export default AdminPage;
