import { useEffect, useState } from "react";
import { googleLoginUser } from "../../services/authService";

declare global {
  interface Window {
    google: any;
  }
}

interface GoogleLoginProps {
  onError?: (msg: string) => void;
  onLoading?: (loading: boolean) => void;
  theme?: "outline" | "filled_blue" | "filled_black";
}

const GoogleLogin = ({ onError, onLoading, theme = "filled_blue" }: GoogleLoginProps) => {
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
    if (!clientId) {
      onError?.("Google Client ID is missing. Please check environment configuration.");
      return;
    }

    const initializeGoogle = () => {
      if (!window.google?.accounts?.id) return;

      window.google.accounts.id.initialize({
        client_id: clientId,
        callback: handleCredentialResponse,
      });

      const btnContainer = document.getElementById("googleBtn");
      if (btnContainer) {
        btnContainer.innerHTML = "";
        window.google.accounts.id.renderButton(btnContainer, {
          theme: theme,
          size: "large",
          shape: "rectangular",
          width: 320,
          logo_alignment: "left",
          text: "continue_with",
        });
        setIsReady(true);
      }
    };

    if (window.google?.accounts?.id) {
      initializeGoogle();
    } else {
      const interval = setInterval(() => {
        if (window.google?.accounts?.id) {
          initializeGoogle();
          clearInterval(interval);
        }
      }, 150);

      return () => clearInterval(interval);
    }
  }, []);

  const handleCredentialResponse = async (response: any) => {
    try {
      onLoading?.(true);
      onError?.("");

      await googleLoginUser(response.credential);

      // Force full reload into dashboard so auth state & token headers sync
      window.location.replace("/dashboard");
    } catch (error: any) {
      onLoading?.(false);
      console.error("Google login failed", error);
      const serverMessage =
        error.response?.data?.message ||
        (typeof error.response?.data === "string" ? error.response.data : null) ||
        "Google sign-in failed. Please try again.";
      onError?.(serverMessage);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center w-full min-h-[44px]">
      <div id="googleBtn" className="flex justify-center w-full" />
      {!isReady && !import.meta.env.VITE_GOOGLE_CLIENT_ID && (
        <p className="text-sm text-red-500 mt-2">
          Google Client ID not configured.
        </p>
      )}
    </div>
  );
};

export default GoogleLogin;
