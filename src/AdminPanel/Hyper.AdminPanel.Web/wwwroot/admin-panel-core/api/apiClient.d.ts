import { AxiosInstance, AxiosRequestConfig } from 'axios';
import { ApiResponse, ApiClientConfig } from '../types';

/**
 * API Client class with advanced features
 */
declare class ApiClient {
    private instance;
    private config;
    private isRefreshing;
    private refreshSubscribers;
    constructor(config?: Partial<ApiClientConfig>);
    /**
     * Get the underlying Axios instance
     */
    getAxiosInstance(): AxiosInstance;
    /**
     * Setup request and response interceptors
     */
    private setupInterceptors;
    /**
     * Handle outgoing requests
     */
    private handleRequest;
    /**
     * Handle request errors
     */
    private handleRequestError;
    /**
     * Handle incoming responses
     */
    private handleResponse;
    /**
     * Handle response errors
     */
    private handleResponseError;
    /**
     * Refresh the access token
     */
    private refreshToken;
    /**
     * Notify all subscribers waiting for token refresh
     */
    private notifyRefreshSubscribers;
    /**
     * Get access token from storage
     */
    private getAccessToken;
    /**
     * Clear authentication data
     */
    private clearAuth;
    /**
     * Generate unique request ID
     */
    private generateRequestId;
    /**
     * Create standardized API error
     */
    private createApiError;
    /**
     * Extract API error from response data
     */
    private extractApiError;
    /**
     * Dispatch custom event
     */
    private dispatchEvent;
    /**
     * GET request
     */
    get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
    /**
     * POST request
     */
    post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
    /**
     * PUT request
     */
    put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
    /**
     * PATCH request
     */
    patch<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
    /**
     * DELETE request
     */
    delete<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
    /**
     * Upload file
     */
    uploadFile<T>(url: string, file: File, fieldName?: string, additionalData?: Record<string, unknown>, onProgress?: (percent: number) => void): Promise<ApiResponse<T>>;
    /**
     * Download file
     */
    downloadFile(url: string, filename?: string): Promise<void>;
    /**
     * Extract filename from response headers
     */
    private extractFilename;
    /**
     * Set authorization token manually
     */
    setAuthToken(token: string): void;
    /**
     * Remove authorization token
     */
    removeAuthToken(): void;
    /**
     * Update base URL
     */
    setBaseURL(baseURL: string): void;
}
export declare const apiClient: ApiClient;
export { ApiClient };
export default apiClient;
