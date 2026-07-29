import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import CreatableSelect from "react-select/creatable";
import { Turnstile } from "@marsidev/react-turnstile";

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

    <div className="bg-white shadow-xl rounded-2xl p-8 w-full max-w-md">



      <input

        aria-label="Long URL"

        placeholder="Enter long URL"

        value={longUrl}

        onChange={(e)=>setLongUrl(e.target.value)}

        className="w-full border rounded-lg px-4 py-2 mb-4"

      />





      <input

        aria-label="Custom alias"

        placeholder="Custom alias (optional)"

        value={alias}

        onChange={(e)=>setAlias(e.target.value)}

        className="w-full border rounded-lg px-4 py-2 mb-4"

      />





      {isAuthenticated && (

        <CreatableSelect

          isMulti

          options={tagOptions}

          value={
            tagOptions.filter(
              (opt)=>tags.includes(opt.value)
            )
          }

          placeholder="Search or add tags..."

          className="mb-4"



          onChange={(selected)=>{

            setTags(
              selected
                ? selected.map((t:any)=>t.value)
                : []
            );

          }}



          onCreateOption={(inputValue)=>{


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

        onChange={(e)=>setExpirationDate(e.target.value)}

        min={new Date().toISOString().slice(0,16)}

        className="w-full border rounded-lg px-4 py-2 mb-4"

      />








      {!isAuthenticated && showCaptcha && siteKey && (

        <div className="mb-4 flex justify-center">

          <Turnstile

            ref={turnstileRef}

            siteKey={siteKey}



            onSuccess={(token)=>{


              setCaptchaToken(token);

              setError("");

              // DO NOT call performCreate here

            }}



            onError={()=>{


              setError(
                "CAPTCHA verification failed."
              );


              resetCaptcha();


            }}



            onExpire={()=>{


              resetCaptcha();


            }}

          />

        </div>

      )}








      <button

        onClick={handleCreate}

        className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"

      >

        Generate Short Link

      </button>







      {error && (

        <p className="text-red-500 mt-3 text-sm">

          {error}

        </p>

      )}







      {result && (

        <div className="mt-6 bg-green-50 border border-green-200 p-4 rounded-lg">


          <p className="font-semibold text-green-700">

            Short URL Generated:

          </p>



          <div className="flex items-center justify-between mt-2">


            <a

              href={result.shortUrl}

              target="_blank"

              rel="noreferrer"

              className="text-blue-600 hover:underline break-all"

            >

              {result.shortUrl}

            </a>




            {!isAuthenticated && (

              <Link

                to="/register"

                className="ml-4 bg-white border border-gray-300 text-gray-700 px-3 py-1 rounded text-sm"

              >

                View Analytics

              </Link>

            )}


          </div>


        </div>

      )}


    </div>

  );

}