import GoogleLogin from "../components/auth/GoogleLogin";

const LoginPage = () => {
  return (
    <div style={{ marginTop: "100px", textAlign: "center" }}>
      <h2>Sign in</h2>
      <GoogleLogin />
    </div>
  );
};

export default LoginPage;
