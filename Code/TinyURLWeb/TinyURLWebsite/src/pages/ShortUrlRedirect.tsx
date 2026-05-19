import { useEffect } from "react";
import { useParams } from "react-router-dom";

const API_ORIGIN = (import.meta.env.VITE_API_URL || "").replace(/\/+$/, "");

export default function ShortUrlRedirect() {
  const { shortCode } = useParams<{ shortCode: string }>();

  useEffect(() => {
    if (!shortCode) {
      return;
    }

    const redirectOrigin = API_ORIGIN || window.location.origin;
    window.location.replace(`${redirectOrigin}/${encodeURIComponent(shortCode)}`);
  }, [shortCode]);

  return (
    <main className="min-h-screen flex items-center justify-center bg-slate-50 text-slate-700">
      Redirecting...
    </main>
  );
}
