export const isSessionValid = () => {
  const expiry = localStorage.getItem("expiry");
  if (!expiry) return false;

  return new Date().getTime() < new Date(expiry).getTime();
};
