import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { ShieldCheck, Loader2 } from "lucide-react";

export default function ShortUrlRedirect() {
  const { shortCode } = useParams<{ shortCode: string }>();

  useEffect(() => {
    if (!shortCode) {
      return;
    }

    window.location.replace(`/api/r/${encodeURIComponent(shortCode)}`);
  }, [shortCode]);

  return (
    <main className="min-h-screen flex flex-col items-center justify-center bg-[#0B1120] text-slate-200 selection:bg-blue-500/30">
      <div className="flex flex-col items-center gap-4 p-8 rounded-2xl bg-[#0f172a]/80 border border-slate-800 shadow-2xl backdrop-blur-md">
        <div className="relative">
          <div className="w-12 h-12 rounded-xl bg-blue-500/15 border border-blue-500/30 flex items-center justify-center text-blue-400">
            <ShieldCheck className="w-6 h-6 text-blue-400 animate-pulse" />
          </div>
          <Loader2 className="w-4 h-4 text-blue-400 animate-spin absolute -bottom-1 -right-1" />
        </div>
        <div className="text-center">
          <p className="text-sm font-semibold text-slate-100">Checking link safety...</p>
          <p className="text-xs text-slate-400 mt-1">Redirecting you securely</p>
        </div>
      </div>
    </main>
  );
}
