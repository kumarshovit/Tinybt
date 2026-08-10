import ShortenCard from "./ShortenCard";

export default function HeroSection({ onUrlCreated }: any) {
  return (
    <section className="bg-gradient-to-br from-[#0f172a] via-[#0f2439] to-[#0a3854] relative pt-32 pb-56 overflow-hidden">
      {/* Wave SVG at bottom */}
      <div className="absolute bottom-0 left-0 w-full overflow-hidden leading-[0]">
        <svg data-name="Layer 1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 120" preserveAspectRatio="none" className="relative block w-full h-[60px] md:h-[100px]" style={{ transform: "rotateY(180deg)" }}>
          <path d="M321.39,56.44c58-10.79,114.16-30.13,172-41.86,82.39-16.72,168.19-17.73,250.45-.39C823.78,31,906.67,72,985.66,92.83c70.05,18.48,146.53,26.09,214.34,3V120H0V95.8C59.71,118.11,130.83,121.92,189.65,108.6Z" fill="#f9fafb"></path>
        </svg>
      </div>

      <div className="max-w-6xl mx-auto px-6 grid md:grid-cols-2 gap-10 items-center relative z-10">

        <div>
          <h2 className="text-4xl font-bold text-white mb-4">
            Shorten & Manage Your Links
          </h2>
          <p className="text-blue-100/90 text-lg">
            {/* Branded short links. Smart tagging. Real-time updates.<br></br> */}
            Welcome to LinkBT — simplifying the way you share, manage, and track links.
            Create custom short links, monitor detailed analytics, generate QR codes, and use powerful link management features to get more from every click.

          </p>
        </div>

        <div className="flex md:justify-end justify-center mt-8 md:mt-0 md:-mr-16">
          <ShortenCard onUrlCreated={onUrlCreated} />
        </div>
      </div>
    </section>
  );
}
