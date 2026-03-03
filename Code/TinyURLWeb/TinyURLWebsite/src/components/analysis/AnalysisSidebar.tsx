interface Props {
  active: string;
  setActive: (value: string) => void;
}

const items = [
  { id: "time", label: "Clicks Over Time" },
  { id: "geo", label: "Clicks by Geography" },
  { id: "lang", label: "Clicks by Language" },
  { id: "popular", label: "Popular Days & Times" },
  { id: "device", label: "Clicks by Device" },
  { id: "os", label: "Clicks by OS" },
  { id: "browser", label: "Clicks by Browser" },
];

export default function AnalysisSidebar({ active, setActive }: Props) {
  return (
    <div className="w-64 bg-white shadow p-4 space-y-2">
      {items.map((item) => (
        <button
          key={item.id}
          onClick={() => setActive(item.id)}
          className={`block w-full text-left p-2 rounded ${
            active === item.id
              ? "bg-blue-600 text-white"
              : "hover:bg-gray-100"
          }`}
        >
          {item.label}
        </button>
      ))}
    </div>
  );
}