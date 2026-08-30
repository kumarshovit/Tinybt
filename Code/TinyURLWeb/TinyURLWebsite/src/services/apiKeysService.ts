import apiClient from './authService';

export interface ApiKeyDto {
    id: number;
    name: string;
    keyPrefix: string;
    createdAt: string;
    expiresAt: string | null;
    lastUsedAt: string | null;
    revokedAt: string | null;
}

export interface GetApiKeysResponse {
    apiKeys: ApiKeyDto[];
}

export interface CreateApiKeyResponse {
    id: number;
    prefix: string;
    rawKey: string;
    createdAt: string;
}

const API_KEYS_URL = `${import.meta.env.VITE_API_URL}/api/v1/api-keys`;

export const apiKeysService = {
    getApiKeys: async (): Promise<ApiKeyDto[]> => {
        const response = await apiClient.get<GetApiKeysResponse>(API_KEYS_URL);
        return response.data.apiKeys;
    },

    createApiKey: async (name: string): Promise<CreateApiKeyResponse> => {
        const response = await apiClient.post<CreateApiKeyResponse>(API_KEYS_URL, { name });
        return response.data;
    },

    revokeApiKey: async (id: number): Promise<void> => {
        await apiClient.delete(`${API_KEYS_URL}/${id}`);
    }
};
