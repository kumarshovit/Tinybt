import { useEffect } from "react";

export default function useClickOutside(
  ref: React.RefObject<HTMLElement | null>,
  onClose: () => void,
  enabled: boolean
) {
  useEffect(() => {
    if (!enabled) return;

    function handleClick(event: MouseEvent) {
      if (!ref.current) return;

      if (!ref.current.contains(event.target as Node)) {
        onClose();
      }
    }

    function handleEscape(event: KeyboardEvent) {
      if (event.key === "Escape") {
        onClose();
      }
    }

    document.addEventListener("mousedown", handleClick);
    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("mousedown", handleClick);
      document.removeEventListener("keydown", handleEscape);
    };
  }, [ref, onClose, enabled]);
}