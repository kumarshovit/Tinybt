import { Loader2 } from "lucide-react";

export default function PageLoadingFallback() {
  return (
    <div className="min-h-screen w-full flex flex-col items-center justify-center bg-[#0b1120] text-white relative overflow-hidden">
      {/* Subtle Background Glows */}
      <div className="absolute top-1/4 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-1/4 left-1/2 -translate-x-1/2 translate-y-1/2 w-80 h-80 bg-indigo-500/10 rounded-full blur-3xl pointer-events-none" />

      {/* Center Glass Card */}
      <div className="relative z-10 flex flex-col items-center p-8 rounded-2xl bg-white/[0.03] border border-white/10 backdrop-blur-xl shadow-2xl">
        {/* Animated Brand Emblem */}
        <div className="relative mb-5 flex items-center justify-center">
          <div className="w-16 h-16 rounded-2xl bg-gradient-to-tr from-blue-600 to-cyan-400 p-[2px] shadow-lg shadow-blue-500/30 animate-pulse">
            <div className="w-full h-full bg-[#0f172a] rounded-[14px] flex items-center justify-center">
              <span className="text-xl font-black bg-gradient-to-r from-blue-400 to-cyan-300 bg-clip-text text-transparent">
                BT
              </span>
            </div>
          </div>
          <Loader2 className="absolute -inset-2 w-20 h-20 text-blue-400/40 animate-spin" />
        </div>

        {/* Text Details */}
        <h3 className="text-lg font-semibold text-white tracking-wide mb-1">
          LinkBT
        </h3>
        <p className="text-xs text-gray-400 tracking-wider uppercase font-medium animate-pulse">
          Loading page resources...
        </p>

        {/* Shimmer skeleton bar */}
        <div className="w-48 h-1.5 bg-white/10 rounded-full mt-6 overflow-hidden">
          <div className="h-full w-full bg-gradient-to-r from-blue-500 to-cyan-400 rounded-full animate-top-loading-bar" />
        </div>
      </div>
    </div>
  );
}
