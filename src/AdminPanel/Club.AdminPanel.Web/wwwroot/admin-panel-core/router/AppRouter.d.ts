import { ReactNode } from 'react';
import { MenuItem, AuthConfig } from '../types';

export interface RouteConfig {
    path: string;
    element: ReactNode;
    children?: RouteConfig[];
    permission?: string | string[];
    role?: string | string[];
    index?: boolean;
}
export interface AppRouterProps {
    /** Menu items for sidebar */
    menuItems?: MenuItem[];
    /** Custom routes */
    routes?: RouteConfig[];
    /** Login page component (defaults to built-in) */
    loginPage?: ReactNode;
    /** Dashboard page component */
    dashboardPage?: ReactNode;
    /** 404 page component (defaults to built-in) */
    notFoundPage?: ReactNode;
    /** Auth configuration */
    authConfig?: Partial<AuthConfig>;
    /** Base path for the app */
    basePath?: string;
    /** Logo component or URL */
    logo?: ReactNode | string;
    /** App title */
    appTitle?: string;
    /** Loading fallback */
    loadingFallback?: ReactNode;
    /** Callback when auth state changes */
    onAuthStateChange?: (isAuthenticated: boolean, user: unknown) => void;
    /** Callback when session expires */
    onSessionExpired?: () => void;
    /** Custom header actions */
    headerActions?: ReactNode;
}
/**
 * App Router Component
 *
 * Sets up routing with authentication, layouts, and protected routes.
 */
export declare function AppRouter({ menuItems, routes, loginPage, dashboardPage, notFoundPage, authConfig, basePath, logo, appTitle, loadingFallback, onAuthStateChange, onSessionExpired, headerActions, }: AppRouterProps): import("react/jsx-runtime").JSX.Element;
export default AppRouter;
