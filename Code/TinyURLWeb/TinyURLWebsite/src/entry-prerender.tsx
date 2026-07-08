import { StrictMode } from "react";
import { renderToString } from "react-dom/server";
import { HelmetProvider } from "react-helmet-async";
import { MemoryRouter } from "react-router-dom";
import LandingPage from "./pages/LandingPage";
import ContactPage from "./pages/ContactPage";
import TermsPage from "./pages/TermsPage";
import PrivacyPolicyPage from "./pages/PrivacyPolicyPage";
import Login from "./pages/Login";
import Register from "./pages/Register";
import ForgotPassword from "./pages/ForgotPassword";

(HelmetProvider as any).canUseDOM = false;

const routes: Record<string, () => React.ReactNode> = {
  "/": () => <LandingPage />,
  "/contact": () => <ContactPage />,
  "/terms": () => <TermsPage />,
  "/privacy-policy": () => <PrivacyPolicyPage />,
  "/login": () => <Login />,
  "/register": () => <Register />,
  "/forgot-password": () => <ForgotPassword />,
};

export function render(url: string) {
  const helmetContext: any = {};

  const Page = routes[url] ?? routes["/"];

  const html = renderToString(
    <StrictMode>
      <HelmetProvider context={helmetContext}>
        <MemoryRouter initialEntries={[url]}>{Page()}</MemoryRouter>
      </HelmetProvider>
    </StrictMode>,
  );

  return {
    html,
    head: [
      helmetContext.helmet?.title.toString(),
      helmetContext.helmet?.meta.toString(),
      helmetContext.helmet?.link.toString(),
      helmetContext.helmet?.script.toString(),
    ]
      .filter(Boolean)
      .join("\n"),
  };
}
