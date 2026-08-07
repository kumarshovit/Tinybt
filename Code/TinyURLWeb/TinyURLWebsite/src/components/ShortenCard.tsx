import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import CreatableSelect from "react-select/creatable";
import { Turnstile } from "@marsidev/react-turnstile";
import { Link2, QrCode, AlertCircle, CheckCircle } from "lucide-react";
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
    <div className="bg-white shadow-xl rounded-2xl w-full max-w-md overflow-hidden">
      <div className="flex border-b text-sm sm:text-base">
        <button
          onClick={() => { setActiveTab("shorten"); setResult(null); setError(""); }}
          className={`flex-1 flex items-center justify-center py-4 font-medium px-2 transition ${activeTab === "shorten"
            ? "text-blue-600 border-b-2 border-blue-600 bg-white"
            : "text-gray-500 hover:text-gray-700 bg-gray-50"
            }`}
        >
          <Link2 className="w-5 h-5 mr-2 shrink-0" />
          Shorten a Link
        </button>
        <button
          onClick={() => { setActiveTab("qr"); setResult(null); setError(""); }}
          className={`flex-1 flex items-center justify-center py-4 font-medium px-2 transition ${activeTab === "qr"
            ? "text-blue-600 border-b-2 border-blue-600 bg-white"
            : "text-gray-500 hover:text-gray-700 bg-gray-50 bg-[#F2F7F6]"
            }`}
        >
          <QrCode className="w-5 h-5 mr-2 shrink-0" />
          Generate QR Code
        </button>
      </div>

      <div className="p-8">
        <input
          aria-label="Long URL"
          placeholder={activeTab === "qr" ? "Destination URL *" : "Enter long URL"}

          value={longUrl}

          onChange={(e) => setLongUrl(e.target.value)}

          className="w-full border rounded-lg px-4 py-2 mb-4"

        />





        <input

          aria-label="Custom alias"

          placeholder="Custom alias (optional)"

          value={alias}

          onChange={(e) => setAlias(e.target.value)}

          className="w-full border rounded-lg px-4 py-2 mb-4"

        />





        {isAuthenticated && (

          <CreatableSelect

            isMulti

            options={tagOptions}

            value={
              tagOptions.filter(
                (opt) => tags.includes(opt.value)
              )
            }

            placeholder="Search or add tags..."

            className="mb-4"



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

        )}







        <input

          type="datetime-local"

          value={expirationDate}

          onChange={(e) => setExpirationDate(e.target.value)}

          min={new Date().toISOString().slice(0, 16)}

          className="w-full border rounded-lg px-4 py-2 mb-4"

        />








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
          className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"
        >
          {activeTab === "qr" ? "Generate QR Code" : "Generate Short Link"}
        </button>







        {error && (
          <div className="mt-4 bg-red-50 border border-red-100 text-red-700 p-3 rounded-lg flex items-start gap-3 w-full">
            <AlertCircle className="w-5 h-5 shrink-0 mt-0.5" />
            <p className="text-sm">{error}</p>
          </div>
        )}







        {result && (
          <div className="mt-6 bg-green-50 border border-green-200 p-5 rounded-xl shadow-sm">
            <div className="flex items-center gap-2 mb-3 text-green-700">
              <CheckCircle className="w-5 h-5 shrink-0" />
              <p className="font-semibold">Short URL Generated:</p>
            </div>

            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
              <a
                href={result.shortUrl}
                target="_blank"
                rel="noreferrer"
                className="text-blue-600 hover:underline break-all font-medium text-lg"
              >
                {result.shortUrl}
              </a>

              <div className="flex items-center gap-2">
                <button
                  onClick={() => setShowQrModal(true)}
                  className="bg-white border text-blue-600 border-blue-200 hover:bg-blue-50 px-3 py-1.5 rounded-lg text-sm font-medium transition flex items-center gap-1.5 shadow-sm"
                >
                  <QrCode className="w-4 h-4" /> QR
                </button>
                {!isAuthenticated && (
                  <Link
                    to="/register"
                    className="bg-white border border-gray-300 text-gray-700 px-3 py-1.5 rounded-lg text-sm hover:bg-gray-50 transition flex items-center shadow-sm"
                  >
                    View Analytics
                  </Link>
                )}
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