import { useEffect } from "react";
import { googleLogin } from "../../services/authService";

const GoogleLogin = () => {
  useEffect(() => {
    if (window.google) {
      window.google.accounts.id.initialize({
        client_id: "725373686810-7l1vpjc41duo2sjjbs8djp05vmqeaoam.apps.googleusercontent.com",
        callback: handleCredentialResponse,
      });

      window.google.accounts.id.renderButton(
        document.getElementById("googleBtn"),
        { theme: "outline", size: "large" }
      );
    }
  }, []);

  const handleCredentialResponse = async (response: any) => {
    try {
      const data = await googleLogin(response.credential);
      console.log("JWT:", data);

      localStorage.setItem("token", data.token);
      alert("Login Successful 🚀");
    } catch (error) {
      console.error(error);
      alert("Google login failed");
    }
  };

  return <div id="googleBtn"></div>;
};

export default GoogleLogin;
