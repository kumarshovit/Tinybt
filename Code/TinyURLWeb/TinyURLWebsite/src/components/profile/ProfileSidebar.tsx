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
    <div className="w-full lg:w-60 bg-white border rounded-xl shadow-sm p-4 h-fit">

      {/* Title only visible on desktop */}
      <h2 className="hidden lg:block text-sm font-semibold text-gray-500 mb-3">
        Settings
      </h2>

      {/* Sidebar / Tabs */}
      <div className="flex lg:flex-col gap-2 overflow-x-auto lg:overflow-visible">

        {items.map((item) => (

          <button
            key={item.name}
            onClick={() => setActive(item.name)}
            className={`whitespace-nowrap px-4 py-2 rounded-lg text-sm transition ${
              active === item.name
                ? "bg-indigo-50 text-blue-600"
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