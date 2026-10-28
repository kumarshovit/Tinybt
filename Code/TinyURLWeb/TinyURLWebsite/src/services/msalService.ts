import { PublicClientApplication, type Configuration } from "@azure/msal-browser";

let msalInstance: PublicClientApplication | null = null;
let initPromise: Promise<void> | null = null;

const getMsalConfig = (): Configuration => {
  const clientId = import.meta.env.VITE_MICROSOFT_CLIENT_ID || "";
  const origin = typeof window !== "undefined" ? window.location.origin : "http://localhost:5173";

  return {
    auth: {
      clientId,
      authority: "https://login.microsoftonline.com/common",
      redirectUri: origin,
    },
    cache: {
      cacheLocation: "localStorage",
    },
    system: {
      // Crucial for SPA popups: enables direct location polling between parent window and popup
      enableLegacyPolling: true,
    },
  };
};

export const getMsalInstance = async (): Promise<PublicClientApplication> => {
  if (typeof window === "undefined") {
    throw new Error("MSAL is only available in browser environments.");
  }

  if (!msalInstance) {
    msalInstance = new PublicClientApplication(getMsalConfig());
  }

  if (!initPromise) {
    initPromise = (async () => {
      await msalInstance!.initialize();
      await msalInstance!.handleRedirectPromise();
    })();
  }

  await initPromise;
  return msalInstance;
};

export const loginWithMicrosoft = async (): Promise<string> => {
  const instance = await getMsalInstance();
  const origin = window.location.origin;

  const loginRequest = {
    scopes: ["openid", "profile", "email"],
    prompt: "select_account",
    redirectUri: origin,
  };

  const response = await instance.loginPopup(loginRequest);

  if (!response?.idToken) {
    throw new Error("No ID token returned from Microsoft.");
  }

  return response.idToken;
};
