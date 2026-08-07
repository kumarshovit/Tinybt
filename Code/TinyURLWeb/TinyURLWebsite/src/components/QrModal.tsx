import { useState, useEffect } from "react";
import { Download, Copy, X } from "lucide-react";

interface Props {
    shortCode: string;
    onClose: () => void;
}

export default function QrModal({ shortCode, onClose }: Props) {
    const [copied, setCopied] = useState(false);
    const [blobUrl, setBlobUrl] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    // Construct API endpoint URL properly from env
    const apiUrl = import.meta.env.VITE_API_URL || "";
    const qrEndpoint = `${apiUrl}/api/qrcode/${shortCode}`;

    useEffect(() => {
        async function fetchQR() {
            try {
                const response = await fetch(qrEndpoint);
                if (!response.ok) throw new Error("Failed to load QR code");
                const blob = await response.blob();
                setBlobUrl(URL.createObjectURL(blob));
            } catch (error) {
                console.error(error);
            } finally {
                setLoading(false);
            }
        }
        fetchQR();

        return () => {
            // Cleanup blob URL on unmount
            if (blobUrl) {
                URL.revokeObjectURL(blobUrl);
            }
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [qrEndpoint]);

    const handleDownload = () => {
        if (!blobUrl) return;
        const a = document.createElement("a");
        a.href = blobUrl;
        a.download = `linkbt-${shortCode}-qr.png`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    };

    const handleCopy = async () => {
        if (!blobUrl) return;
        try {
            const response = await fetch(blobUrl);
            const blob = await response.blob();
            await navigator.clipboard.write([
                new ClipboardItem({
                    "image/png": blob,
                }),
            ]);
            setCopied(true);
            setTimeout(() => setCopied(false), 2000);
        } catch (err) {
            console.error("Failed to copy image: ", err);
            alert("Failed to copy image to clipboard.");
        }
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30 backdrop-blur-md p-4 transition-all">
            <div className="bg-white rounded-2xl shadow-2xl overflow-hidden w-full max-w-sm relative flex flex-col pt-6 pb-8 px-6">

                <button
                    onClick={onClose}
                    className="absolute top-4 right-4 text-gray-400 hover:text-gray-700 transition"
                    aria-label="Close"
                >
                    <X className="w-6 h-6" />
                </button>

                <h3 className="text-xl font-bold text-center text-gray-800 mb-2">QR Code</h3>
                <p className="text-sm text-center text-gray-500 mb-6">
                    Scan to visit the short URL for {shortCode}
                </p>

                <div className="flex justify-center mb-6">
                    <div className="w-56 h-56 bg-gray-50 border-2 border-dashed border-gray-200 rounded-lg flex items-center justify-center p-2">
                        {loading ? (
                            <span className="text-gray-400 animate-pulse">Generating...</span>
                        ) : blobUrl ? (
                            <img
                                src={blobUrl}
                                alt={`QR code for ${shortCode}`}
                                className="w-full h-full object-contain"
                            />
                        ) : (
                            <span className="text-red-400 text-sm p-4 text-center">Failed to load</span>
                        )}
                    </div>
                </div>

                <div className="flex flex-col gap-3">
                    <button
                        onClick={handleDownload}
                        disabled={!blobUrl}
                        className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2.5 rounded-lg disabled:opacity-50 transition"
                    >
                        <Download className="w-4 h-4" />
                        Download PNG
                    </button>

                    <button
                        onClick={handleCopy}
                        disabled={!blobUrl}
                        className="w-full flex items-center justify-center gap-2 bg-gray-100 border border-gray-200 hover:bg-gray-200 text-gray-700 py-2.5 rounded-lg disabled:opacity-50 transition"
                    >
                        <Copy className="w-4 h-4" />
                        {copied ? "Copied!" : "Copy Image"}
                    </button>
                </div>
            </div>
        </div>
    );
}
