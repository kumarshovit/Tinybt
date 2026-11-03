import { useEffect } from "react";
import { useParams } from "react-router-dom";

export default function ShortUrlRedirect() {
  const { shortCode } = useParams<{ shortCode: string }>();

  useEffect(() => {
    if (!shortCode) {
      return;
    }

    window.location.replace(`/api/r/${encodeURIComponent(shortCode)}`);
  }, [shortCode]);

  return (
    <main className="min-h-screen flex items-center justify-center bg-slate-50 text-slate-700">
      Redirecting...
    </main>
  );
}
