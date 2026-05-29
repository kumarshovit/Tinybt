import { useState } from "react";

interface Props {
  name: string;
  email: string;
  createdAt: string;
  handleUpdateName: (newName: string) => Promise<void>;
}

export default function ProfileInfoCard({
  name,
  email,
  createdAt,
  handleUpdateName
}: Props) {

  const [editing, setEditing] = useState(false);
  const [value, setValue] = useState(name);

  const save = async () => {
    await handleUpdateName(value);
    setEditing(false);
  };

  return (
    <div className="bg-white rounded-xl shadow p-6">

      <h2 className="text-lg font-semibold mb-4">
        Personal Information
      </h2>

      {!editing ? (

        <div className="flex justify-between items-center mb-4">

          <div>
            <p className="text-sm text-gray-500">Full Name</p>
            <p className="font-medium text-gray-800">{name}</p>
          </div>

          <button
            onClick={() => setEditing(true)}
            className="text-blue-600 text-sm hover:underline"
          >
            Edit
          </button>

        </div>

      ) : (

        <div className="space-y-3 mb-4">

          <input
            aria-label="Full name"
            value={value}
            onChange={(e) => setValue(e.target.value)}
            className="w-full border px-4 py-2 rounded-lg"
          />

          <div className="flex gap-2">

            <button
              onClick={save}
              className="bg-blue-600 text-white px-4 py-2 rounded-lg"
            >
              Save
            </button>

            <button
              onClick={() => setEditing(false)}
              className="px-4 py-2 border rounded-lg"
            >
              Cancel
            </button>

          </div>

        </div>

      )}

      <p className="text-sm text-gray-600">
        <strong>Email:</strong> {email}
      </p>

      <p className="text-sm text-gray-600 mt-1">
        <strong>Account Created:</strong>{" "}
        {new Date(createdAt).toLocaleDateString()}
      </p>

    </div>
  );
}