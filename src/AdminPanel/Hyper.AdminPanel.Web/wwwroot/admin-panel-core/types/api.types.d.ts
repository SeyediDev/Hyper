/**
 * API Types
 * Neo BPMS Admin Panel Core
 */
/**
 * Standard API response wrapper
 */
export interface ApiResponse<T = unknown> {
    success: boolean;
    data: T;
    message?: string;
    errors?: ApiError[];
    metadata?: ResponseMetadata;
}
/**
 * Paginated API response
 */
export interface PaginatedResponse<T> extends ApiResponse<T[]> {
    pagination: PaginationInfo;
}
/**
 * Pagination info
 */
export interface PaginationInfo {
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}
/**
 * Pagination request parameters
 */
export interface PaginationParams {
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
}
/**
 * API Error
 */
export interface ApiError {
    code: string;
    message: string;
    field?: string;
    details?: Record<string, unknown>;
}
/**
 * Response metadata
 */
export interface ResponseMetadata {
    requestId: string;
    timestamp: string;
    duration: number;
    version: string;
}
/**
 * HTTP Methods
 */
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
/**
 * Request configuration
 */
export interface RequestConfig {
    method?: HttpMethod;
    headers?: Record<string, string>;
    params?: Record<string, unknown>;
    data?: unknown;
    timeout?: number;
    withCredentials?: boolean;
    responseType?: 'json' | 'blob' | 'text' | 'arraybuffer';
    signal?: AbortSignal;
    onUploadProgress?: (progress: ProgressEvent) => void;
    onDownloadProgress?: (progress: ProgressEvent) => void;
}
/**
 * API Client configuration
 */
export interface ApiClientConfig {
    baseURL: string;
    timeout: number;
    withCredentials: boolean;
    headers: Record<string, string>;
    retryAttempts: number;
    retryDelay: number;
    onUnauthorized?: () => void;
    onServerError?: (error: ApiError) => void;
    onNetworkError?: (error: Error) => void;
}
/**
 * Interceptor types
 */
export interface RequestInterceptor {
    onFulfilled: (config: RequestConfig) => RequestConfig | Promise<RequestConfig>;
    onRejected?: (error: Error) => Promise<never>;
}
export interface ResponseInterceptor<T = unknown> {
    onFulfilled: (response: ApiResponse<T>) => ApiResponse<T> | Promise<ApiResponse<T>>;
    onRejected?: (error: Error) => Promise<never>;
}
/**
 * Upload file request
 */
export interface UploadFileRequest {
    file: File;
    path?: string;
    metadata?: Record<string, unknown>;
    onProgress?: (percent: number) => void;
}
/**
 * Upload file response
 */
export interface UploadFileResponse {
    id: string;
    filename: string;
    originalName: string;
    mimeType: string;
    size: number;
    url: string;
    thumbnailUrl?: string;
}
/**
 * Filter operators for query
 */
export type FilterOperator = 'eq' | 'neq' | 'gt' | 'gte' | 'lt' | 'lte' | 'contains' | 'startsWith' | 'endsWith' | 'in' | 'notIn' | 'isNull' | 'isNotNull';
/**
 * Filter definition
 */
export interface FilterDefinition {
    field: string;
    operator: FilterOperator;
    value: unknown;
}
/**
 * Query parameters for list endpoints
 */
export interface QueryParams extends PaginationParams {
    filters?: FilterDefinition[];
    search?: string;
    searchFields?: string[];
    include?: string[];
    fields?: string[];
}
/**
 * Bulk operation request
 */
export interface BulkOperationRequest<T> {
    items: T[];
    operation: 'create' | 'update' | 'delete';
}
/**
 * Bulk operation response
 */
export interface BulkOperationResponse {
    success: boolean;
    processedCount: number;
    failedCount: number;
    errors: Array<{
        index: number;
        error: ApiError;
    }>;
}
