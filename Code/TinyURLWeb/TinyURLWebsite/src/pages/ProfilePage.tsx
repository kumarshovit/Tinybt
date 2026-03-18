import { useEffect, useState } from "react";
import api from "../utils/api";
import Navbar from "../components/Navbar";

import ProfileSidebar from "../components/profile/ProfileSidebar";
import ProfileAvatar from "../components/profile/ProfileAvatar";
import ProfileInfoCard from "../components/profile/ProfileInfoCard";
import ProfileSecurityCard from "../components/profile/ProfileSecurityCard";
import DangerZoneCard from "../components/profile/DangerZoneCard";

interface Profile {
  email: string;
  fullName: string;
  createdAt: string;
}

const ProfilePage = () => {

  const [profile, setProfile] = useState<Profile | null>(null);

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");


  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const [loading, setLoading] = useState(true);

  const [activeSection, setActiveSection] = useState("Profile");

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {

    try {

      setLoading(true);

      const response = await api.get("/profile");

      setProfile(response.data);

      await new Promise((resolve) => setTimeout(resolve, 100));

    } catch {

      setError("Failed to load profile.");

    } finally {

      setLoading(false);

    }

  };

  const handleChangePassword = async (e: React.FormEvent) => {

    e.preventDefault();

    try {

      await api.put("/profile/password", {
        currentPassword,
        newPassword
      });

      setMessage("Password changed successfully!");

      setCurrentPassword("");
      setNewPassword("");

    } catch {

      setError("Failed to change password.");

    }

  };


  const handleUpdateName = async (newName: string) => {

    if (!newName.trim()) {
      setError("Full name is required.");
      return;
    }

    try {

      await api.put("/profile/name", { fullName: newName });

      setMessage("Profile updated successfully!");

      fetchProfile();

    } catch {

      setError("Failed to update profile.");

    }

  };
  const handleDeleteAccount = async () => {

    const confirmDelete = window.confirm(
      "Are you sure? This action cannot be undone."
    );

    if (!confirmDelete) return;

    await api.delete("/profile");

    localStorage.removeItem("token");

    window.location.href = "/login";

  };

  return (

    <>
      <Navbar />

      <div className="min-h-screen bg-gray-50 py-10 px-4 sm:px-6 overflow-x-hidden">
        <div className="max-w-6xl mx-auto flex flex-col lg:flex-row gap-8">
          <ProfileSidebar
            active={activeSection}
            setActive={setActiveSection}
          />

          <div className="flex-1 space-y-6 w-full">
            <h1 className="text-3xl font-bold">
              Account Settings
            </h1>

            {message && (
              <p className="text-green-600 text-sm">
                {message}
              </p>
            )}

            {error && (
              <p className="text-red-500 text-sm">
                {error}
              </p>
            )}

            {loading && (
              <div className="space-y-4 animate-pulse">

                <div className="bg-white h-24 rounded-xl" />

                <div className="bg-white h-40 rounded-xl" />

              </div>
            )}

            {!loading && profile && activeSection === "Profile" && (
              <>
                <ProfileAvatar name={profile.fullName} email={profile.email} />

                <ProfileInfoCard
                  name={profile.fullName}
                  email={profile.email}
                  createdAt={profile.createdAt}
                  handleUpdateName={handleUpdateName}
                />
              </>
            )}

            {activeSection === "Security" && (
              <ProfileSecurityCard
                currentPassword={currentPassword}
                newPassword={newPassword}
                setCurrentPassword={setCurrentPassword}
                setNewPassword={setNewPassword}
                handleChangePassword={handleChangePassword}
                loading={loading}
              />
            )}

            {activeSection === "Danger" && (
              <DangerZoneCard
                handleDeleteAccount={handleDeleteAccount}
              />
            )}

          </div>

        </div>

      </div>

    </>
  );
};

export default ProfilePage;



