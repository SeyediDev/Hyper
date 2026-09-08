import { AuthState, User, LoginRequest, LoginResponse } from '../../types';

/**
 * Async thunk: Login
 */
export declare const login: import('@reduxjs/toolkit').AsyncThunk<LoginResponse, {
    credentials: LoginRequest;
    apiClient: {
        post: (url: string, data: unknown) => Promise<{
            data: LoginResponse;
        }>;
    };
}, {
    rejectValue: string;
    state?: unknown;
    dispatch?: import('redux-thunk').ThunkDispatch<unknown, unknown, import('redux').UnknownAction> | undefined;
    extra?: unknown;
    serializedErrorType?: unknown;
    pendingMeta?: unknown;
    fulfilledMeta?: unknown;
    rejectedMeta?: unknown;
}>;
/**
 * Async thunk: Logout
 */
export declare const logout: import('@reduxjs/toolkit').AsyncThunk<void, {
    apiClient?: {
        post: (url: string) => Promise<void>;
    };
} | undefined, {
    rejectValue: string;
    state?: unknown;
    dispatch?: import('redux-thunk').ThunkDispatch<unknown, unknown, import('redux').UnknownAction> | undefined;
    extra?: unknown;
    serializedErrorType?: unknown;
    pendingMeta?: unknown;
    fulfilledMeta?: unknown;
    rejectedMeta?: unknown;
}>;
/**
 * Async thunk: Refresh Token
 */
export declare const refreshAccessToken: import('@reduxjs/toolkit').AsyncThunk<{
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
}, {
    apiClient: {
        post: (url: string, data: unknown) => Promise<{
            data: {
                accessToken: string;
                refreshToken: string;
                expiresIn: number;
            };
        }>;
    };
}, {
    rejectValue: string;
    state?: unknown;
    dispatch?: import('redux-thunk').ThunkDispatch<unknown, unknown, import('redux').UnknownAction> | undefined;
    extra?: unknown;
    serializedErrorType?: unknown;
    pendingMeta?: unknown;
    fulfilledMeta?: unknown;
    rejectedMeta?: unknown;
}>;
/**
 * Async thunk: Fetch current user
 */
export declare const fetchCurrentUser: import('@reduxjs/toolkit').AsyncThunk<User, {
    apiClient: {
        get: (url: string) => Promise<{
            data: User;
        }>;
    };
}, {
    rejectValue: string;
    state?: unknown;
    dispatch?: import('redux-thunk').ThunkDispatch<unknown, unknown, import('redux').UnknownAction> | undefined;
    extra?: unknown;
    serializedErrorType?: unknown;
    pendingMeta?: unknown;
    fulfilledMeta?: unknown;
    rejectedMeta?: unknown;
}>;
export declare const setUser: import('@reduxjs/toolkit').ActionCreatorWithPayload<User | null, "auth/setUser">, updateUser: import('@reduxjs/toolkit').ActionCreatorWithPayload<Partial<User>, "auth/updateUser">, setTokens: import('@reduxjs/toolkit').ActionCreatorWithPayload<{
    accessToken: string;
    refreshToken?: string;
    expiresIn: number;
}, "auth/setTokens">, updateLastActivity: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"auth/updateLastActivity">, clearAuth: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"auth/clearAuth">, setError: import('@reduxjs/toolkit').ActionCreatorWithPayload<string | null, "auth/setError">, clearError: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"auth/clearError">, sessionExpired: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"auth/sessionExpired">;
export declare const selectUser: (state: {
    auth: AuthState;
}) => User | null;
export declare const selectIsAuthenticated: (state: {
    auth: AuthState;
}) => boolean;
export declare const selectIsLoading: (state: {
    auth: AuthState;
}) => boolean;
export declare const selectAuthError: (state: {
    auth: AuthState;
}) => string | null;
export declare const selectAccessToken: (state: {
    auth: AuthState;
}) => string | null;
export declare const selectExpiresAt: (state: {
    auth: AuthState;
}) => number | null;
/**
 * Check if user has specific permission
 */
export declare const selectHasPermission: (permission: string) => (state: {
    auth: AuthState;
}) => boolean;
/**
 * Check if user has specific role
 */
export declare const selectHasRole: (role: string) => (state: {
    auth: AuthState;
}) => boolean;
declare const _default: import('redux').Reducer<{
    user: User | null;
    accessToken: string | null;
    refreshToken: string | null;
    expiresAt: number | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
    lastActivity: number | null;
}>;
export default _default;
