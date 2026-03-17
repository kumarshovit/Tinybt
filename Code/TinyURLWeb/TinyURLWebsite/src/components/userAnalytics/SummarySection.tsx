import SummaryCard from "./SummaryCard";

/* ---------- REUSABLE CARD WRAPPER ---------- */

const CardWrapper = ({ children, onClick, hoverText }: any) => (
  <div
    onClick={onClick}
    className="relative group cursor-pointer rounded-2xl overflow-hidden transition transform hover:scale-[1.03] hover:shadow-xl"
  >
    {children}

    {/* Hover Overlay */}
    <div className="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition duration-300">
      <div className="text-center">
        <p className="text-blue-300 text-sm font-semibold tracking-wide mt-20 ml-15">
          {hoverText}
        </p>
      </div>
    </div>
  </div>
);

/* ---------- MAIN COMPONENT ---------- */

export default function SummarySection({
  totalClicks,
  totalLinks,
  country,
  device,
  browser,
  openDataPopup,
  openAllLinksPopup   // ⭐ ADD THIS
}: any){

  const totalClicksValue = totalClicks?.reduce(
    (sum:any,x:any)=>sum+x.count,0
  ) || 0;

  return(

    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 sm:gap-6 mb-10">

      {/* ---------- Total Clicks ---------- */}
      <CardWrapper
        onClick={() => openDataPopup("Daily Click Breakdown", totalClicks)}
        hoverText="View click trends"
      >
        <SummaryCard
          title="Total Clicks 📈"
          value={totalClicksValue}
        />
      </CardWrapper>

      {/* ---------- Total Links (FIXED) ---------- */}
      <CardWrapper
        onClick={async () => {

          const data = await openAllLinksPopup();   // ⭐ FIX

          openDataPopup("All Links", data);         // ⭐ FIX

        }}
        hoverText="View all links"
      >
        <SummaryCard
          title="Total Links 🔗"
          value={totalLinks?.length || 0}
        />
      </CardWrapper>

      {/* ---------- Countries ---------- */}
      <CardWrapper
        onClick={() => openDataPopup("Country Analytics", country)}
        hoverText="View country stats"
      >
        <SummaryCard
          title="Countries 🌍"
          value={country?.length || 0}
        />
      </CardWrapper>

      {/* ---------- Devices ---------- */}
      <CardWrapper
        onClick={() => openDataPopup("Device Analytics", device)}
        hoverText="View device stats"
      >
        <SummaryCard
          title="Devices 💻"
          value={device?.length || 0}
        />
      </CardWrapper>

      {/* ---------- Browsers ---------- */}
      <CardWrapper
        onClick={() => openDataPopup("Browser Analytics", browser)}
        hoverText="View browser stats"
      >
        <SummaryCard
          title="Browsers 🌐"
          value={browser?.length || 0}
        />
      </CardWrapper>

    </div>

  )
}