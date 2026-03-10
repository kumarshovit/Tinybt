interface Props {
  active: string;
  setActive: (section: string) => void;
}

export default function ProfileSidebar({ active, setActive }: Props) {

  const items = [
    { name: "Profile" },
    { name: "Security" },
    { name: "Danger" }
  ];

  return (
    <div className="w-60 bg-white border rounded-xl shadow-sm p-4 h-fit">

      <h2 className="text-sm font-semibold text-gray-500 mb-3">
        Settings
      </h2>

      <div className="space-y-1">

        {items.map((item) => (

          <button
            key={item.name}
            onClick={() => setActive(item.name)}
            className={`w-full text-left px-3 py-2 rounded-lg text-sm transition ${
              active === item.name
                ? "bg-indigo-50 text-indigo-600"
                : "hover:bg-gray-50"
            }`}
          >
            {item.name}
          </button>

        ))}

      </div>
    </div>
  );
}