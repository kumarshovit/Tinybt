import { useState } from "react";

interface Props {
  name: string;
  email: string;
}

export default function ProfileAvatar({ name,email }: Props) {

  const [preview, setPreview] = useState<string | null>(null);

  const handleFile = (e: React.ChangeEvent<HTMLInputElement>) => {

    const file = e.target.files?.[0];

    if (!file) return;

    const url = URL.createObjectURL(file);

    setPreview(url);

  };

  return (

    <div className="bg-white rounded-xl shadow p-6 flex items-center gap-4 hover:shadow-md transition">

      <div className="w-16 h-16 rounded-full bg-blue-600 text-white flex items-center justify-center text-xl font-bold overflow-hidden">

        {preview ? (

          <img
            src={preview}
            className="w-full h-full object-cover"
          />

        ) : (

          name?.charAt(0)

        )}

      </div>

        <div className="flex flex-col gap-1">
        <p className="text-sm text-gray-500">Name</p>
        <p className="font-medium text-gray-800">{name}</p>

        <p className="text-sm text-gray-500 mt-2">Email</p>
        <p className="font-medium text-gray-800">{email}</p>
      </div>

    </div>

  );
}