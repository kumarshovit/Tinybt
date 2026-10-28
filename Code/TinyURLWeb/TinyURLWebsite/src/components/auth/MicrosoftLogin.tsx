import { useState } from "react";
import { loginWithMicrosoft } from "../../services/msalService";
import { microsoftLoginUser } from "../../services/authService";
import { Loader2 } from "lucide-react";

interface MicrosoftLoginProps {
  onError?: (msg: string) => void;
  onLoading?: (loading: boolean) => void;
}

const MicrosoftLogin = ({ onError, onLoading }: MicrosoftLoginProps) => {
  const [loading, setLoading] = useState(false);

  const handleMicrosoftLogin = async () => {
    const clientId = import.meta.env.VITE_MICROSOFT_CLIENT_ID;
    if (!clientId) {
      onError?.("Microsoft Client ID is not configured. Please check environment settings.");
      return;
    }

    try {
      setLoading(true);
      onLoading?.(true);
      onError?.("");

      // 1. Popup login via Microsoft Identity
      const idToken = await loginWithMicrosoft();

      // 2. Validate token on backend and set cookies/localStorage
      await microsoftLoginUser(idToken);

      // 3. Navigate into dashboard
      window.location.replace("/dashboard");
    } catch (error: any) {
      setLoading(false);
      onLoading?.(false);

      // Ignore normal user cancellations
      if (
        error?.name === "BrowserAuthError" &&
        (error?.errorCode === "user_cancelled" || error?.message?.includes("user_cancelled"))
      ) {
        return;
      }

      console.error("Microsoft login failed", error);
      const serverMessage =
        error.response?.data?.message ||
        (typeof error.response?.data === "string" ? error.response.data : null) ||
        error.message ||
        "Microsoft sign-in failed. Please try again.";

      onError?.(serverMessage);
    }
  };

  return (
    <button
      type="button"
      onClick={handleMicrosoftLogin}
      disabled={loading}
      aria-label="Sign in with Microsoft"
      className="flex items-center justify-center gap-3 w-full max-w-[320px] h-[40px] px-4 rounded-md bg-white hover:bg-gray-50 active:bg-gray-100 text-gray-800 text-[14px] font-medium tracking-tight shadow-sm border border-gray-200 transition-all duration-150 disabled:opacity-60 disabled:cursor-not-allowed cursor-pointer"
    >
      {loading ? (
        <Loader2 className="w-4 h-4 animate-spin text-gray-700" />
      ) : (
        /* Official Microsoft 4-Color Squares Logo */
        <svg className="w-4 h-4 flex-shrink-0" viewBox="0 0 21 21">
          <rect x="1" y="1" width="9" height="9" fill="#f25022" />
          <rect x="11" y="1" width="9" height="9" fill="#7fba00" />
          <rect x="1" y="11" width="9" height="9" fill="#00a4ef" />
          <rect x="11" y="11" width="9" height="9" fill="#ffb900" />
        </svg>
      )}
      <span className="font-semibold text-gray-800">
        {loading ? "Signing in..." : "Sign in with Microsoft"}
      </span>
    </button>
  );
};

export default MicrosoftLogin;

