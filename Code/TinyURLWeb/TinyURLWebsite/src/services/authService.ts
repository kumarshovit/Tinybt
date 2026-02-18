import axios from "axios";

// 🔹 Base API URL
const API = "https://localhost:7025/api/auth";

// 🔹 Create axios instance
const apiClient = axios.create({
  baseURL: API,
});

// 🔹 Attach JWT automatically
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// 🔹 Auto logout ONLY for protected endpoints
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const originalRequest = error.config;

    if (
      error.response?.status === 401 &&
      !originalRequest.url?.includes("/login") &&
      !originalRequest.url?.includes("/register") &&
      !originalRequest.url?.includes("/verify")
    ) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

// 🔹 Register
export const registerUser = async (email: string, password: string) => {
  const response = await apiClient.post("/register", { email, password });
  return response.data;
};

// 🔹 Verify Email
export const verifyEmail = async (token: string) => {
  const response = await apiClient.get(`/verify?token=${token}`);
  return response.data;
};

// 🔹 Login
export const loginUser = async (email: string, password: string) => {
  const response = await apiClient.post("/login", { email, password });

  const token =
    response.data.token ||
    response.data.accessToken;

  if (!token) {
    throw new Error("Token not received from backend");
  }

  localStorage.setItem("token", token);
  localStorage.setItem("refreshToken", response.data.refreshToken);

  return response.data;
};


// 🔹 Logout
export const logoutUser = async () => {
  const token = localStorage.getItem("token");
  const refreshToken = localStorage.getItem("refreshToken");

  try {
    await apiClient.post(
      "/logout",
      { refreshToken },
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (error) {
    console.error("Logout API error:", error);
  }

  localStorage.removeItem("token");
  localStorage.removeItem("refreshToken");
};

export default apiClient;
