import { useLoading } from "../../context/LoadingContext";
import { Loader2 } from "lucide-react";

export default function GlobalLoadingPanel() {
  const { isLoading, message } = useLoading();

  if (!isLoading) return null;

  return (
    <>
      {/* 1. Ultra-sleek Top Progress Bar */}
      <div className="fixed top-0 left-0 right-0 z-[9999] h-[3px] bg-transparent overflow-hidden pointer-events-none">
        <div className="h-full w-full bg-gradient-to-r from-blue-600 via-indigo-400 to-cyan-300 shadow-[0_0_10px_rgba(56,189,248,0.8)] animate-top-loading-bar" />
      </div>

      {/* 2. Floating Glassmorphism Status Badge */}
      <div className="fixed bottom-6 right-6 z-[9998] flex items-center gap-3 px-4 py-2.5 rounded-full bg-slate-900/85 backdrop-blur-md border border-white/15 text-white shadow-2xl shadow-blue-500/20 animate-fade-in pointer-events-none transition-all duration-300">
        <Loader2 className="w-4 h-4 text-cyan-400 animate-spin" />
        <span className="text-xs font-medium text-gray-200 tracking-wide">
          {message || "Loading data..."}
        </span>
        <span className="flex h-2 w-2 relative">
          <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
          <span className="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
        </span>
      </div>
    </>
  );
}
