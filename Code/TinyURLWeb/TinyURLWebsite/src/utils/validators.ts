export const validateEmail = (email: string) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
};

export const validatePassword = (password: string) => {
  return /^(?=.*[A-Z])(?=.*\d).{8,}$/.test(password);
};
