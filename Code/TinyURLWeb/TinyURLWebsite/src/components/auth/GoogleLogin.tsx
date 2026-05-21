import { useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

declare global {
  interface Window {
    google: any;
  }
}

const GoogleLogin = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const initializeGoogle = () => {
      if (!window.google) return;

      window.google.accounts.id.initialize({
        client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
        callback: handleCredentialResponse,
      });

      window.google.accounts.id.renderButton(
        document.getElementById("googleBtn"),
        {
          theme: "outline",
          size: "large",
        },
      );
    };

    // 🔥 Wait until script loads
    const interval = setInterval(() => {
      if (window.google) {
        initializeGoogle();
        clearInterval(interval);
      }
    }, 100);

    return () => clearInterval(interval);
  }, []);

  const handleCredentialResponse = async (response: any) => {
    try {
      const res = await  axios.post(
        `${import.meta.env.VITE_API_URL}/api/auth/google-login`,
        { token: response.credential },
      );

      localStorage.setItem("token", res.data.accessToken);
      localStorage.setItem("refreshToken", res.data.refreshToken);

      navigate("/dashboard");
    } catch (error) {
      console.error("Google login failed", error);
    }
  };

  return <div id="googleBtn"></div>;
};

export default GoogleLogin;
