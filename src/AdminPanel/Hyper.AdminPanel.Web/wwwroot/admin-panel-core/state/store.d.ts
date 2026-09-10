import { TypedUseSelectorHook } from 'react-redux';

/**
 * Root reducer combining all slices
 */
declare const rootReducer: import('redux').Reducer<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
}, import('redux').UnknownAction, Partial<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    } | undefined;
    ui: import('.').UiState | undefined;
}>>;
/**
 * Create store with configuration
 */
export declare const createAppStore: (preloadedState?: Partial<RootState>) => import('@reduxjs/toolkit').EnhancedStore<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
}, import('redux').UnknownAction, import('@reduxjs/toolkit').Tuple<[import('redux').StoreEnhancer<{
    dispatch: import('redux-thunk').ThunkDispatch<{
        auth: {
            user: import('..').User | null;
            accessToken: string | null;
            refreshToken: string | null;
            expiresAt: number | null;
            isAuthenticated: boolean;
            isLoading: boolean;
            error: string | null;
            lastActivity: number | null;
        };
        ui: import('.').UiState;
    }, undefined, import('redux').UnknownAction>;
}>, import('redux').StoreEnhancer]>>;
/**
 * Default store instance
 */
export declare const store: import('@reduxjs/toolkit').EnhancedStore<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
}, import('redux').UnknownAction, import('@reduxjs/toolkit').Tuple<[import('redux').StoreEnhancer<{
    dispatch: import('redux-thunk').ThunkDispatch<{
        auth: {
            user: import('..').User | null;
            accessToken: string | null;
            refreshToken: string | null;
            expiresAt: number | null;
            isAuthenticated: boolean;
            isLoading: boolean;
            error: string | null;
            lastActivity: number | null;
        };
        ui: import('.').UiState;
    }, undefined, import('redux').UnknownAction>;
}>, import('redux').StoreEnhancer]>>;
/**
 * Type definitions
 */
export type RootState = ReturnType<typeof rootReducer>;
export type AppStore = ReturnType<typeof createAppStore>;
export type AppDispatch = AppStore['dispatch'];
/**
 * Typed hooks
 */
export declare const useAppDispatch: () => import('redux-thunk').ThunkDispatch<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
}, undefined, import('redux').UnknownAction> & import('redux').Dispatch<import('redux').UnknownAction>;
export declare const useAppSelector: TypedUseSelectorHook<RootState>;
/**
 * Helper to get state outside of React components
 */
export declare const getState: () => {
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
};
export declare const dispatch: import('redux-thunk').ThunkDispatch<{
    auth: {
        user: import('..').User | null;
        accessToken: string | null;
        refreshToken: string | null;
        expiresAt: number | null;
        isAuthenticated: boolean;
        isLoading: boolean;
        error: string | null;
        lastActivity: number | null;
    };
    ui: import('.').UiState;
}, undefined, import('redux').UnknownAction> & import('redux').Dispatch<import('redux').UnknownAction>;
export default store;
