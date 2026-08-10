import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import CreatableSelect from "react-select/creatable";
import { Turnstile } from "@marsidev/react-turnstile";
import { QrCode, AlertCircle, CheckCircle } from "lucide-react";
import QrModal from "./QrModal";

import {
  createUrl,
  addTags,
  getAllTags
} from "../api/urlService";

let memoryGuestUrlResult: any = null;

export default function ShortenCard({ onUrlCreated }: any) {
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);


  const isAuthenticated =
    typeof window !== "undefined" && mounted
      ? !!localStorage.getItem("token")
      : false;


  const [longUrl, setLongUrl] = useState("");
  const [alias, setAlias] = useState("");
  const [expirationDate, setExpirationDate] = useState("");

  const [tags, setTags] = useState<string[]>([]);
  const [tagOptions, setTagOptions] = useState<any[]>([]);

  const [result, setResult] = useState<any>(memoryGuestUrlResult);
  const [error, setError] = useState("");

  const [captchaToken, setCaptchaToken] = useState("");
  const [showCaptcha, setShowCaptcha] = useState(false);

  const [activeTab, setActiveTab] = useState<"shorten" | "qr">("shorten");
  const [showQrModal, setShowQrModal] = useState(false);

  const turnstileRef = useRef<any>(null);

  const siteKey = import.meta.env.VITE_TURNSTILE_SITEKEY || "";



  /* ============================= */
  /* LOAD TAGS                      */
  /* ============================= */

  useEffect(() => {

    const loadTags = async () => {

      const response = await getAllTags();

      if (response.success) {

        setTagOptions(
          (response.data as string[]).map((t) => ({
            value: t,
            label: t
          }))
        );

      }

    };


    if (isAuthenticated) {
      loadTags();
    }


  }, [isAuthenticated]);





  /* ============================= */
  /* RESET CAPTCHA                  */
  /* ============================= */

  const resetCaptcha = () => {

    if (!isAuthenticated) {

      setCaptchaToken("");

      turnstileRef.current?.reset();

    }

  };





  /* ============================= */
  /* CREATE URL                     */
  /* ============================= */

  const performCreate = async (token?: string) => {

    try {

      const response = await createUrl(
        longUrl,
        alias || undefined,
        expirationDate
          ? new Date(expirationDate).toISOString()
          : undefined,
        token
      );



      if (!response.success) {

        setError(
          response.message || "Something went wrong"
        );


        resetCaptcha();

        return;

      }



      const res = response.data;



      if (tags.length > 0) {

        await addTags(res.id, tags);

      }




      const newLink = {

        id: res.id,

        shortUrl: res.shortUrl,

        clickCount: 0,

        tags

      };



      if (!isAuthenticated) {

        memoryGuestUrlResult = newLink;

      }




      if (onUrlCreated) {

        await onUrlCreated();

      }




      setResult(newLink);


      setLongUrl("");

      setAlias("");

      setExpirationDate("");

      setTags([]);


      setCaptchaToken("");

      setShowCaptcha(false);


      resetCaptcha();



    }
    catch {

      setError(
        "Something went wrong"
      );

      resetCaptcha();

    }

  };







  /* ============================= */
  /* HANDLE CREATE                  */
  /* ============================= */

  const handleCreate = async () => {


    setError("");

    setResult(null);



    if (!longUrl.trim()) {

      setError(
        "Please enter a URL."
      );

      return;

    }




    try {

      new URL(longUrl);

    }
    catch {

      setError(
        "Please enter a valid URL (e.g. https://example.com)."
      );

      return;

    }




    if (isAuthenticated) {

      await performCreate();

      return;

    }





    if (!captchaToken) {

      setShowCaptcha(true);

      return;

    }





    await performCreate(captchaToken);

  };







  return (
    <div className="bg-[#1e293b]/60 backdrop-blur-xl border border-white/10 shadow-2xl rounded-2xl w-full max-w-xl overflow-hidden relative">
      <div className="p-8">

        <div className="text-blue-300 font-semibold text-sm mb-5 tracking-wider uppercase">
          Quick Create
        </div>

        <div className="mb-4">
          <label className="text-white text-sm font-medium mb-1.5 block">Long Link to Shorten</label>
          <input
            aria-label="Long URL"
            placeholder={activeTab === "qr" ? "Destination URL *" : "Enter long URL"}
            value={longUrl}
            onChange={(e) => setLongUrl(e.target.value)}
            className="w-full bg-[#0f172a]/60 border border-white/10 focus:border-blue-500 rounded-lg px-4 py-3 text-white placeholder-gray-500 outline-none transition"
          />
        </div>

        <div className="grid grid-cols-2 gap-4 mb-4">
          <div>
            <label className="text-white text-sm font-medium mb-1.5 block">Custom Alias <span className="text-gray-400 font-normal opacity-70">(Optional)</span></label>
            <input
              aria-label="Custom alias"
              placeholder="Custom alias"
              value={alias}
              onChange={(e) => setAlias(e.target.value)}
              className="w-full bg-[#0f172a]/60 border border-white/10 focus:border-blue-500 rounded-lg px-4 py-3 text-white placeholder-gray-500 outline-none transition"
            />
          </div>
          <div>
            <label className="text-white text-sm font-medium mb-1.5 block">Expiration Date <span className="text-gray-400 font-normal opacity-70">(Optional)</span></label>
            <input
              type="datetime-local"
              value={expirationDate}
              onChange={(e) => setExpirationDate(e.target.value)}
              min={new Date().toISOString().slice(0, 16)}
              className="w-full bg-[#0f172a]/60 border border-white/10 focus:border-blue-500 rounded-lg px-4 py-3 text-white placeholder-gray-500 outline-none transition [color-scheme:dark]"
            />
          </div>
        </div>

        {isAuthenticated && (
          <div className="mb-4">
            <label className="text-white text-sm font-medium mb-1.5 block">Tags</label>
            <CreatableSelect
              isMulti
              options={tagOptions}
              value={
                tagOptions.filter(
                  (opt) => tags.includes(opt.value)
                )
              }
              placeholder="Search or add tags..."
              className="text-gray-900"
              styles={{
                control: (base) => ({
                  ...base,
                  backgroundColor: "rgba(15, 23, 42, 0.6)",
                  borderColor: "rgba(255, 255, 255, 0.1)",
                  color: "white"
                }),
                menu: (base) => ({ ...base, color: 'black' }),
                singleValue: (base) => ({ ...base, color: "white" })
              }}
              onChange={(selected) => {
                setTags(
                  selected
                    ? selected.map((t: any) => t.value)
                    : []
                );
              }}
              onCreateOption={(inputValue) => {
                const newOption = {
                  value: inputValue,
                  label: inputValue
                };
                setTagOptions([
                  ...tagOptions,
                  newOption
                ]);
                setTags([
                  ...tags,
                  inputValue
                ]);
              }}
            />
          </div>
        )}

        <div className="flex items-center justify-between mb-6 bg-[#0f172a]/40 p-3 rounded-lg border border-white/5">
          <div className="flex items-center gap-3">
            <div className="bg-white/10 p-1.5 rounded-md">
              <QrCode className="w-5 h-5 text-gray-300" />
            </div>
            <span className="text-gray-200 text-sm font-medium">Generate QR Code</span>
          </div>
          <button
            type="button"
            onClick={() => { setActiveTab(activeTab === "qr" ? "shorten" : "qr"); setResult(null); setError(""); }}
            className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none ${activeTab === 'qr' ? 'bg-blue-500' : 'bg-gray-600'}`}
          >
            <span className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${activeTab === 'qr' ? 'translate-x-6' : 'translate-x-1'}`} />
          </button>
        </div>

        {!isAuthenticated && showCaptcha && siteKey && (
          <div className="mb-4 flex justify-center">
            <Turnstile
              ref={turnstileRef}
              siteKey={siteKey}
              onSuccess={(token) => {
                setCaptchaToken(token);
                setError("");
                // DO NOT call performCreate here
              }}
              onError={() => {
                setError(
                  "CAPTCHA verification failed."
                );
                resetCaptcha();
              }}
              onExpire={() => {
                resetCaptcha();
              }}
            />
          </div>
        )}

        <button
          onClick={handleCreate}
          className="w-full bg-blue-600 hover:bg-blue-500 text-white font-semibold py-3.5 rounded-lg transition shadow-lg shadow-blue-500/20"
        >
          {activeTab === "qr" ? "Generate QR Code" : "Shorten"}
        </button>

        {error && (
          <div className="mt-4 bg-red-500/10 border border-red-500/40 text-red-200 p-3 rounded-lg flex items-start gap-3 w-full">
            <AlertCircle className="w-5 h-5 shrink-0 mt-0.5 text-red-400" />
            <p className="text-sm">{error}</p>
          </div>
        )}

        {result && (
          <div className="mt-6 bg-[#0f172a]/80 border border-blue-500/30 p-5 rounded-xl shadow-inner">
            <div className="flex items-center gap-2 mb-3 text-blue-300">
              <CheckCircle className="w-5 h-5 shrink-0" />
              <p className="font-semibold">Short URL Generated:</p>
            </div>

            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
              <a
                href={result.shortUrl}
                target="_blank"
                rel="noreferrer"
                className="text-white hover:text-blue-400 hover:underline break-all font-medium text-lg transition"
              >
                {result.shortUrl}
              </a>

              <div className="flex items-center gap-2">
                <button
                  onClick={() => setShowQrModal(true)}
                  className="bg-white/10 border border-white/20 text-white hover:bg-white/20 px-3 py-1.5 rounded-lg text-sm font-medium transition flex items-center gap-1.5 shadow-sm"
                >
                  <QrCode className="w-4 h-4" /> QR
                </button>
                {/* {!isAuthenticated && (
                  <Link
                    to="/register"
                    className="bg-white/10 border border-white/20 text-white px-3 py-1.5 rounded-lg text-sm hover:bg-white/20 transition flex items-center shadow-sm"
                  >
                    View Analytics
                  </Link>
                )} */}
              </div>
            </div>
          </div>
        )}

      </div>

      {showQrModal && result && (
        <QrModal
          shortCode={result.shortUrl.split('/').pop() || ""}
          onClose={() => setShowQrModal(false)}
        />
      )}
    </div>
  );
}