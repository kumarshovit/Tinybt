import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import Footer from "../components/Footer";
import SEO from "../components/SEO";
import { apiKeysService, type ApiKeyDto, type CreateApiKeyResponse } from "../services/apiKeysService";
import { Copy, Plus, Trash2, KeyRound, AlertTriangle, Key } from "lucide-react";

export default function DeveloperDashboard() {
    const [apiKeys, setApiKeys] = useState<ApiKeyDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [newKeyName, setNewKeyName] = useState("");
    const [generatedKey, setGeneratedKey] = useState<CreateApiKeyResponse | null>(null);
    const [isCreating, setIsCreating] = useState(false);

    useEffect(() => {
        loadApiKeys();
    }, []);

    const loadApiKeys = async () => {
        setLoading(true);
        setError("");
        try {
            const keys = await apiKeysService.getApiKeys();
            setApiKeys(keys);
        } catch (err: any) {
            console.error("Failed to load API keys", err);
            setError(err.response?.data?.message || "Failed to load API keys and developer configurations.");
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async () => {
        if (!newKeyName.trim()) return;
        setIsCreating(true);
        setError("");
        try {
            const result = await apiKeysService.createApiKey(newKeyName);
            setGeneratedKey(result);
            setNewKeyName("");
            loadApiKeys(); // Refresh the list seamlessly
        } catch (err: any) {
            console.error("Creation failed", err);
            setError(err.response?.data?.message || "Creation failed. You may have reached your key limits.");
        } finally {
            setIsCreating(false);
        }
    };

    const handleCopyNewKey = () => {
        if (generatedKey?.rawKey) {
            navigator.clipboard.writeText(generatedKey.rawKey);
            alert("API Key copied to clipboard! Save it securely now—you will never see it again.");
        }
    };

    const handleRevoke = async (id: number) => {
        if (!confirm("Are you sure you want to revoke this API key? This action is permanent and immediate.")) return;

        try {
            await apiKeysService.revokeApiKey(id);
            loadApiKeys();
        } catch (err: any) {
            alert("Failed to revoke: " + (err.response?.data?.message || "Please try again later."));
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            <SEO
                title="Developer API – LinkBT"
                description="Manage your Developer API keys for automated URL shortening."
                noindex={true}
            />
            <Navbar />

            <main className="flex-grow container mx-auto px-4 py-8 max-w-5xl">

                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">Developer API Configuration</h1>
                    <p className="text-gray-600">
                        Generate and manage API Keys to integrate LinkBT directly into your backend architecture and CI/CD operations.
                    </p>
                </div>

                {error && (
                    <div className="bg-red-50 border-l-4 border-red-500 p-4 mb-6">
                        <div className="flex">
                            <AlertTriangle className="h-5 w-5 text-red-500 mr-2" />
                            <p className="text-sm text-red-700">{error}</p>
                        </div>
                    </div>
                )}

                {generatedKey && (
                    <div className="bg-amber-50 border border-amber-300 rounded-lg p-6 mb-8 shadow-sm">
                        <div className="flex items-start mb-4">
                            <KeyRound className="h-6 w-6 text-amber-600 mr-3 mt-1" />
                            <div>
                                <h3 className="text-lg font-bold text-amber-900">Important: New API Key Generated</h3>
                                <p className="text-amber-800 text-sm mt-1">
                                    Please copy your key immediately. We only store a cryptographic hash of this key natively to ensure maximum security.
                                    <strong className="block mt-1">You will not be able to see this token again!</strong>
                                </p>
                            </div>
                        </div>

                        <div className="flex items-center gap-4 bg-white border border-amber-200 rounded p-3">
                            <code className="text-gray-900 font-mono text-sm break-all flex-grow">
                                {generatedKey.rawKey}
                            </code>
                            <button
                                onClick={handleCopyNewKey}
                                className="flex items-center gap-2 px-4 py-2 bg-amber-600 hover:bg-amber-700 text-white rounded text-sm font-medium transition-colors"
                            >
                                <Copy size={16} />
                                Copy
                            </button>
                        </div>

                        <button
                            className="mt-4 text-sm text-amber-900 underline hover:text-amber-700"
                            onClick={() => setGeneratedKey(null)}
                        >
                            I have saved this key securely. Close this panel.
                        </button>
                    </div>
                )}

                <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden mb-8">
                    <div className="p-6 border-b border-gray-200 bg-gray-50 flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h2 className="text-xl font-bold text-gray-800">Generate Key</h2>
                            <p className="text-sm text-gray-500 mt-1">Create a new key to authorize headless connections.</p>
                        </div>
                        <div className="flex gap-2">
                            <input
                                type="text"
                                placeholder="Key Name (e.g., Production AWS)"
                                value={newKeyName}
                                onChange={e => setNewKeyName(e.target.value)}
                                maxLength={40}
                                className="px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 w-full sm:w-64"
                                disabled={isCreating}
                            />
                            <button
                                onClick={handleCreate}
                                disabled={isCreating || !newKeyName.trim()}
                                className="flex items-center gap-2 bg-blue-600 text-white px-5 py-2 rounded-lg font-medium hover:bg-blue-700 disabled:opacity-50 transition-colors whitespace-nowrap"
                            >
                                <Plus size={18} />
                                Create
                            </button>
                        </div>
                    </div>
                </div>

                <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                    <div className="p-6 border-b border-gray-200 bg-gray-50">
                        <h2 className="text-xl font-bold text-gray-800">Active API Keys</h2>
                        <p className="text-sm text-gray-500 mt-1">Revokable active tokens associated strictly with your accounts.</p>
                    </div>

                    <div className="p-6">
                        {loading ? (
                            <div className="py-8 flex justify-center">
                                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                            </div>
                        ) : apiKeys.length === 0 ? (
                            <div className="text-center py-12">
                                <Key className="mx-auto h-12 w-12 text-gray-300 mb-3" />
                                <p className="text-gray-500">No API keys found. Generate one above to begin.</p>
                            </div>
                        ) : (
                            <div className="overflow-x-auto">
                                <table className="w-full text-left border-collapse">
                                    <thead>
                                        <tr className="border-b border-gray-200 text-gray-600 text-sm">
                                            <th className="py-3 px-4 font-medium">Name</th>
                                            <th className="py-3 px-4 font-medium">Prefix (Masked)</th>
                                            <th className="py-3 px-4 font-medium">Status</th>
                                            <th className="py-3 px-4 font-medium">Created On</th>
                                            <th className="py-3 px-4 font-medium text-right">Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {apiKeys.map(key => (
                                            <tr key={key.id} className={`border-b border-gray-100 last:border-0 ${key.revokedAt ? 'opacity-60 bg-gray-50' : 'hover:bg-gray-50'}`}>
                                                <td className="py-3 px-4 font-medium text-gray-900">{key.name}</td>
                                                <td className="py-3 px-4 text-gray-500 font-mono text-sm">{key.keyPrefix}...</td>
                                                <td className="py-3 px-4">
                                                    {key.revokedAt ? (
                                                        <span className="px-2.5 py-1 text-xs font-medium bg-red-100 text-red-700 rounded-full">Revoked</span>
                                                    ) : (
                                                        <span className="px-2.5 py-1 text-xs font-medium bg-emerald-100 text-emerald-700 rounded-full">Active</span>
                                                    )}
                                                </td>
                                                <td className="py-3 px-4 text-gray-500 text-sm">
                                                    {new Date(key.createdAt).toLocaleDateString()}
                                                </td>
                                                <td className="py-3 px-4 text-right">
                                                    <button
                                                        onClick={() => handleRevoke(key.id)}
                                                        disabled={!!key.revokedAt}
                                                        title="Revoke Key"
                                                        className="text-gray-400 hover:text-red-500 transition-colors p-2 disabled:opacity-30 disabled:hover:text-gray-400"
                                                    >
                                                        <Trash2 size={18} />
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </div>
                </div>
            </main>

            <Footer />
        </div>
    );
}
