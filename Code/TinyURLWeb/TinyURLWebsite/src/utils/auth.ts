export const isSessionValid = () => {
  const expiry = localStorage.getItem("expiry");
  if (!expiry) return false;

  return new Date().getTime() < new Date(expiry).getTime();
};
import { jwtDecode } from "jwt-decode";

export const getUserRole = () => {
  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const decoded: any = jwtDecode(token);
    return decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  } catch {
    return null;
  }
};
