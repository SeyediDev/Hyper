import { default as React, ReactNode } from 'react';
import { ProtectedRouteProps } from '../types';

/**
 * Protected Route Component
 */
export declare function ProtectedRoute({ children, requiredPermissions, requiredRoles, requireAll, fallback, redirectTo, }: ProtectedRouteProps): import("react/jsx-runtime").JSX.Element;
/**
 * HOC version of ProtectedRoute
 */
export declare function withProtectedRoute<P extends object>(WrappedComponent: React.ComponentType<P>, options?: Omit<ProtectedRouteProps, 'children'>): (props: P) => import("react/jsx-runtime").JSX.Element;
/**
 * Component to show content only if user has permission
 */
export declare function RequirePermission({ permission, children, fallback, }: {
    permission: string | string[];
    children: ReactNode;
    fallback?: ReactNode;
}): import("react/jsx-runtime").JSX.Element;
/**
 * Component to show content only if user has role
 */
export declare function RequireRole({ role, children, fallback, }: {
    role: string | string[];
    children: ReactNode;
    fallback?: ReactNode;
}): import("react/jsx-runtime").JSX.Element;
/**
 * Component to show content only if user is admin
 */
export declare function RequireAdmin({ children, fallback, }: {
    children: ReactNode;
    fallback?: ReactNode;
}): import("react/jsx-runtime").JSX.Element;
export default ProtectedRoute;
