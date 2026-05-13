import { useEffect, useRef, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { verifyEmail } from "../services/authService";

const VerifyEmail = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  // ✅ Prevent double API call in React StrictMode
  const hasVerified = useRef(false);

  useEffect(() => {
    // ✅ Stop second execution
    if (hasVerified.current) return;

    hasVerified.current = true;

    const token = searchParams.get("token");

    if (!token) {
      setError("Invalid verification link.");
      return;
    }

    const verify = async () => {
      // ✅ Clear old states
      setMessage("");
      setError("");

      try {
        const response = await verifyEmail(token);

        // ✅ Handle both string & object response
        setMessage(response.message || response);

        // ✅ Redirect to login after success
        setTimeout(() => {
          navigate("/login");
        }, 3000);
      } catch (err: any) {
        setError(
          err.response?.data?.message ||
            err.response?.data ||
            "Verification failed.",
        );
      }
    };

    verify();
  }, [searchParams, navigate]);

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        background: "#f5f5f5",
        padding: "20px",
      }}
    >
      <div
        style={{
          background: "white",
          padding: "40px",
          borderRadius: "12px",
          boxShadow: "0 4px 20px rgba(0,0,0,0.1)",
          textAlign: "center",
          width: "100%",
          maxWidth: "450px",
        }}
      >
        <h2
          style={{
            marginBottom: "20px",
            color: "#222",
          }}
        >
          Email Verification
        </h2>

        {message && (
          <p
            style={{
              color: "green",
              fontSize: "16px",
              fontWeight: 500,
            }}
          >
            {message}
          </p>
        )}

        {error && (
          <p
            style={{
              color: "red",
              fontSize: "16px",
              fontWeight: 500,
            }}
          >
            {error}
          </p>
        )}

        {(message || error) && (
          <button
            onClick={() => navigate("/login")}
            style={{
              marginTop: "20px",
              padding: "10px 20px",
              background: "#2563eb",
              color: "white",
              border: "none",
              borderRadius: "8px",
              cursor: "pointer",
              fontSize: "14px",
              fontWeight: 600,
            }}
          >
            Go to Login
          </button>
        )}
      </div>
    </div>
  );
};

export default VerifyEmail;
