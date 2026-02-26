import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { verifyEmail } from "../services/authService";

const VerifyEmail = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    const token = searchParams.get("token");

    if (!token) {
      setError("Invalid verification link.");
      return;
    }

    const verify = async () => {
      try {
        const response = await verifyEmail(token);
        setMessage(response);

        setTimeout(() => {
          navigate("/login");
        }, 3000);

      } catch (err: any) {
        setError(err.response?.data || "Verification failed.");
      }
    };

    verify();
  }, [searchParams, navigate]);

  return (
    <div style={{ textAlign: "center", marginTop: "50px" }}>
      <h2>Email Verification</h2>
      {message && <p style={{ color: "green" }}>{message}</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}
    </div>
  );
};

export default VerifyEmail;
