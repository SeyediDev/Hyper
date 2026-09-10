import { ThemeConfig, Notification } from '../../types';

/**
 * UI State interface
 */
export interface UiState {
    theme: ThemeConfig;
    language: string;
    sidebarCollapsed: boolean;
    sidebarMobileOpen: boolean;
    notifications: Notification[];
    isLoading: boolean;
    loadingMessage: string | null;
    breadcrumbs: Array<{
        label: string;
        path?: string;
    }>;
    pageTitle: string | null;
}
export declare const setTheme: import('@reduxjs/toolkit').ActionCreatorWithPayload<Partial<ThemeConfig>, "ui/setTheme">, toggleThemeMode: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"ui/toggleThemeMode">, setLanguage: import('@reduxjs/toolkit').ActionCreatorWithPayload<string, "ui/setLanguage">, toggleSidebar: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"ui/toggleSidebar">, setSidebarCollapsed: import('@reduxjs/toolkit').ActionCreatorWithPayload<boolean, "ui/setSidebarCollapsed">, toggleMobileSidebar: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"ui/toggleMobileSidebar">, setMobileSidebarOpen: import('@reduxjs/toolkit').ActionCreatorWithPayload<boolean, "ui/setMobileSidebarOpen">, addNotification: import('@reduxjs/toolkit').ActionCreatorWithPayload<Omit<Notification, "id">, "ui/addNotification">, removeNotification: import('@reduxjs/toolkit').ActionCreatorWithPayload<string, "ui/removeNotification">, clearNotifications: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"ui/clearNotifications">, showLoading: import('@reduxjs/toolkit').ActionCreatorWithOptionalPayload<string | undefined, "ui/showLoading">, hideLoading: import('@reduxjs/toolkit').ActionCreatorWithoutPayload<"ui/hideLoading">, setBreadcrumbs: import('@reduxjs/toolkit').ActionCreatorWithPayload<{
    label: string;
    path?: string;
}[], "ui/setBreadcrumbs">, setPageTitle: import('@reduxjs/toolkit').ActionCreatorWithPayload<string | null, "ui/setPageTitle">;
export declare const selectTheme: (state: {
    ui: UiState;
}) => ThemeConfig;
export declare const selectLanguage: (state: {
    ui: UiState;
}) => string;
export declare const selectSidebarCollapsed: (state: {
    ui: UiState;
}) => boolean;
export declare const selectSidebarMobileOpen: (state: {
    ui: UiState;
}) => boolean;
export declare const selectNotifications: (state: {
    ui: UiState;
}) => Notification[];
export declare const selectIsLoading: (state: {
    ui: UiState;
}) => boolean;
export declare const selectLoadingMessage: (state: {
    ui: UiState;
}) => string | null;
export declare const selectBreadcrumbs: (state: {
    ui: UiState;
}) => {
    label: string;
    path?: string;
}[];
export declare const selectPageTitle: (state: {
    ui: UiState;
}) => string | null;
declare const _default: import('redux').Reducer<UiState>;
export default _default;
