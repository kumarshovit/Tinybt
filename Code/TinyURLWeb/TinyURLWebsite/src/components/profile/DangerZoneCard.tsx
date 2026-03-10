interface Props {
  handleDeleteAccount: () => void;
}

export default function DangerZoneCard({ handleDeleteAccount }: Props) {

  return (

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
        className="bg-black text-white px-4 py-2 rounded-lg hover:bg-gray-800"
      >
        Delete My Account
      </button>

    </div>

  );
}