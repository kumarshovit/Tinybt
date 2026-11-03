import { jwtDecode } from "jwt-decode";

export const isSessionValid = () => {
  if (typeof window === "undefined") return false;

  const expiry = localStorage.getItem("expiry");
  if (!expiry) return false;

  return new Date().getTime() < new Date(expiry).getTime();
};

export const isTokenValid = (): boolean => {
  if (typeof window === "undefined") return false;

  const token = localStorage.getItem("token");
  if (!token) return false;

  try {
    const decoded: any = jwtDecode(token);
    if (!decoded || !decoded.exp) return true;

    const isValid = Date.now() < decoded.exp * 1000;
    if (!isValid) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      localStorage.removeItem("expiry");
    }
    return isValid;
  } catch {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    localStorage.removeItem("expiry");
    return false;
  }
};

export const getUserRole = () => {
  if (typeof window === "undefined") return null;

  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const decoded: any = jwtDecode(token);
    return decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  } catch {
    return null;
  }
};
