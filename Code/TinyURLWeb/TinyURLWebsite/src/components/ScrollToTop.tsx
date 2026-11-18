import { useEffect } from "react";
import { useLocation } from "react-router-dom";

/**
 * ScrollToTop component that restores window scroll to (0, 0)
 * whenever the route pathname changes, ensuring users always see
 * the top of newly navigated pages.
 */
export default function ScrollToTop() {
  const { pathname, hash } = useLocation();

  useEffect(() => {
    if (typeof window === "undefined") return;

    if (hash) {
      const element = document.querySelector(hash);
      if (element) {
        element.scrollIntoView({ behavior: "smooth" });
        return;
      }
    }

    window.scrollTo({
      top: 0,
      left: 0,
      behavior: "instant" as ScrollBehavior,
    });
  }, [pathname, hash]);

  return null;
}
