import { AuthConfig, ApiClientConfig } from '../types';

/**
 * Default authentication configuration
 */
export declare const DEFAULT_AUTH_CONFIG: AuthConfig;
/**
 * Default API client configuration
 */
export declare const DEFAULT_API_CONFIG: ApiClientConfig;
/**
 * HTTP Status codes
 */
export declare const HTTP_STATUS: {
    readonly OK: 200;
    readonly CREATED: 201;
    readonly NO_CONTENT: 204;
    readonly BAD_REQUEST: 400;
    readonly UNAUTHORIZED: 401;
    readonly FORBIDDEN: 403;
    readonly NOT_FOUND: 404;
    readonly CONFLICT: 409;
    readonly UNPROCESSABLE_ENTITY: 422;
    readonly TOO_MANY_REQUESTS: 429;
    readonly INTERNAL_SERVER_ERROR: 500;
    readonly BAD_GATEWAY: 502;
    readonly SERVICE_UNAVAILABLE: 503;
    readonly GATEWAY_TIMEOUT: 504;
};
/**
 * Error codes
 */
export declare const ERROR_CODES: {
    readonly NETWORK_ERROR: "NETWORK_ERROR";
    readonly TIMEOUT: "TIMEOUT";
    readonly UNAUTHORIZED: "UNAUTHORIZED";
    readonly FORBIDDEN: "FORBIDDEN";
    readonly NOT_FOUND: "NOT_FOUND";
    readonly VALIDATION_ERROR: "VALIDATION_ERROR";
    readonly SERVER_ERROR: "SERVER_ERROR";
    readonly UNKNOWN_ERROR: "UNKNOWN_ERROR";
    readonly TOKEN_EXPIRED: "TOKEN_EXPIRED";
    readonly INVALID_TOKEN: "INVALID_TOKEN";
    readonly SESSION_EXPIRED: "SESSION_EXPIRED";
};
/**
 * Storage keys
 */
export declare const STORAGE_KEYS: {
    readonly ACCESS_TOKEN: "neo_access_token";
    readonly REFRESH_TOKEN: "neo_refresh_token";
    readonly USER: "neo_user";
    readonly THEME: "neo_theme";
    readonly LANGUAGE: "neo_language";
    readonly SIDEBAR_COLLAPSED: "neo_sidebar_collapsed";
    readonly TABLE_SETTINGS: "neo_table_settings";
};
/**
 * Event names for custom events
 */
export declare const EVENTS: {
    readonly AUTH_STATE_CHANGED: "neo:auth:state_changed";
    readonly TOKEN_REFRESHED: "neo:auth:token_refreshed";
    readonly SESSION_EXPIRED: "neo:auth:session_expired";
    readonly UNAUTHORIZED: "neo:auth:unauthorized";
    readonly PERMISSION_DENIED: "neo:auth:permission_denied";
    readonly NETWORK_ERROR: "neo:network:error";
    readonly SERVER_ERROR: "neo:server:error";
};
/**
 * Query keys for React Query
 */
export declare const QUERY_KEYS: {
    readonly USER: readonly ["user"];
    readonly USERS: readonly ["users"];
    readonly ROLES: readonly ["roles"];
    readonly PERMISSIONS: readonly ["permissions"];
    readonly SESSIONS: readonly ["sessions"];
};
/**
 * Default pagination settings
 */
export declare const PAGINATION_DEFAULTS: {
    readonly PAGE: 1;
    readonly PAGE_SIZE: 20;
    readonly PAGE_SIZE_OPTIONS: readonly [10, 20, 50, 100];
    readonly MAX_PAGE_SIZE: 100;
};
