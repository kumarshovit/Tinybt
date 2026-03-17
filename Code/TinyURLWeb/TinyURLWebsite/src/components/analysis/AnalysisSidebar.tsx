import { useState } from "react";
import {  X ,PanelLeft} from "lucide-react";

interface Props {
  active: string;
  setActive: (value: string) => void;
}

const items = [
  { id: "time", label: "Users Over Time", section: "chart-section" },
  { id: "geo", label: "Users by Geography", section: "chart-section" },
  { id: "lang", label: "Users by Language", section: "chart-section" },
  { id: "popular", label: "Popular Days & Times", section: "chart-section" },
  { id: "device", label: "Users by Device", section: "chart-section" },
  { id: "os", label: "Users by OS", section: "chart-section" },
  { id: "browser", label: "Users by Browser", section: "chart-section" },
];

export default function AnalysisSidebar({ active, setActive }: Props) {

  const [open, setOpen] = useState(false);

  const handleClick = (item: any) => {

  setActive(item.id);
  setOpen(false);

  setTimeout(() => {
    const section = document.getElementById("chart-section");

    if (section) {
      section.scrollIntoView({
        behavior: "smooth",
        block: "start",
      });
    }
  }, 100);

};

  return (
    <>

      {/* Mobile Menu Button */}

      <button
        onClick={() => setOpen(true)}
        className="lg:hidden w-8 top-20 left-4 z-50 bg-blue-600 text-white p-2 rounded-lg shadow ml-2 mt-2"
      >
        <PanelLeft size={16} />
      </button>

      {/* Overlay */}

      {open && (
        <div
          className="fixed inset-0 bg-black/40 z-40 lg:hidden"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Sidebar */}

      <div
        className={`
        fixed lg:static top-0 left-0 h-full w-64 bg-white shadow p-4 space-y-2 z-50
        transform transition-transform duration-300
        ${open ? "translate-x-0" : "-translate-x-full"}
        lg:translate-x-0
      `}
      >

        {/* Mobile Header */}

        <div className="flex justify-between items-center mb-4 lg:hidden">
          <h2 className="font-semibold text-gray-700">
            Analytics
          </h2>

          <button onClick={() => setOpen(false)}>
            <X size={20} />
          </button>
        </div>

        {/* Sidebar Items */}

        {items.map((item) => (

          <button
            key={item.id}
            onClick={() => handleClick(item)}
            className={`block w-full text-left p-2 rounded transition ${
              active === item.id
                ? "bg-blue-600 text-white"
                : "hover:bg-gray-100 text-gray-700"
            }`}
          >
            {item.label}
          </button>

        ))}

      </div>

    </>
  );
}