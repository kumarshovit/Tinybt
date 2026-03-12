import { useState } from "react";

interface Props {

  currentPassword: string;
  newPassword: string;

  setCurrentPassword: (v: string) => void;
  setNewPassword: (v: string) => void;

  handleChangePassword: (e: React.FormEvent) => void;

  loading: boolean;

}

export default function ProfileSecurityCard({

  currentPassword,
  newPassword,
  setCurrentPassword,
  setNewPassword,
  handleChangePassword,
  loading

}: Props) {

  const [showPassword, setShowPassword] = useState(false);

  return (

    <div className="bg-white rounded-xl shadow p-6 hover:shadow-md transition">

      <h2 className="text-lg font-semibold mb-4">
        Security
      </h2>

      <form
        onSubmit={handleChangePassword}
        className="space-y-3"
      >

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
              className="text-blue-600 text-sm"
            >
              {showPassword ? "Hide" : "Show"}
            </button>

          </div>

        </div>

        <button
          type="submit"
          disabled={loading}
          className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700"
        >
          {loading ? "Changing..." : "Change Password"}
        </button>

      </form>

    </div>

  );
}