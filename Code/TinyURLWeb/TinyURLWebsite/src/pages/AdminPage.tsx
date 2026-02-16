import { useEffect, useState } from "react";
import axios from "axios";
import Navbar from "../components/Navbar";

interface User {
  id: number;
  email: string;
  role: string;
  isEmailVerified: boolean;
}

const AdminPage = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [search, setSearch] = useState("");
  const token = localStorage.getItem("token");

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7025/api/auth/all-users",
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setUsers(response.data);
    } catch (error) {
      console.error("Error fetching users");
    }
  };

  const updateRole = async (userId: number, newRole: string) => {
    await axios.put(
      `https://localhost:7025/api/auth/update-role?userId=${userId}&newRole=${newRole}`,
      {},
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    fetchUsers();
  };

  const deleteUser = async (userId: number) => {
    if (!window.confirm("Are you sure you want to delete this user?"))
      return;

    await axios.delete(
      `https://localhost:7025/api/auth/delete-user?userId=${userId}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );

    fetchUsers();
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
                <th className="py-2 px-4">Verified</th>
                <th className="py-2 px-4">Actions</th>
              </tr>
            </thead>

            <tbody>
              {filteredUsers.map((user) => (
                <tr key={user.id} className="text-center border-b">
                  <td className="py-2 px-4">{user.email}</td>
                  <td className="py-2 px-4">{user.role}</td>
                  <td className="py-2 px-4">
                    {user.isEmailVerified ? "✅" : "❌"}
                  </td>

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
