import ShortenCard from "./ShortenCard";

export default function HeroSection({ setLinks }: any) {
  return (
    <div className="bg-gradient-to-r from-blue-50 to-indigo-50 py-20">
      <div className="max-w-6xl mx-auto px-6 grid md:grid-cols-2 gap-10 items-center">

        <div>
          <h2 className="text-4xl font-bold text-gray-800 mb-4">
            Shorten & Manage Your Links
          </h2>
          <p className="text-gray-500 text-lg">
            Branded short links. Smart tagging. Real-time updates.
          </p>
        </div>

        <ShortenCard setLinks={setLinks} />
      </div>
    </div>
  );
}
