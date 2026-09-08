import { useAuthContext } from './AuthProvider';
import { User, Permission } from '../types';

/**
 * Hook return type
 */
export interface UseAuthReturn {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
    accessToken: string | null;
    expiresAt: number | null;
    login: ReturnType<typeof useAuthContext>['login'];
    logout: ReturnType<typeof useAuthContext>['logout'];
    refreshToken: ReturnType<typeof useAuthContext>['refreshToken'];
    updateUser: ReturnType<typeof useAuthContext>['updateUser'];
    hasPermission: (permission: string | string[]) => boolean;
    hasRole: (role: string | string[]) => boolean;
    hasAnyPermission: (permissions: string[]) => boolean;
    hasAllPermissions: (permissions: string[]) => boolean;
    hasAnyRole: (roles: string[]) => boolean;
    hasAllRoles: (roles: string[]) => boolean;
    isAdmin: boolean;
    displayName: string;
    permissions: Permission[];
    roles: string[];
}
/**
 * Main authentication hook
 */
export declare function useAuth(): UseAuthReturn;
/**
 * Hook to check authentication status only
 */
export declare function useIsAuthenticated(): boolean;
/**
 * Hook to get current user only
 */
export declare function useCurrentUser(): User | null;
/**
 * Hook to check loading state only
 */
export declare function useAuthLoading(): boolean;
/**
 * Hook to get auth error only
 */
export declare function useAuthError(): string | null;
/**
 * Hook to check specific permission
 */
export declare function usePermission(permission: string | string[]): boolean;
/**
 * Hook to check specific role
 */
export declare function useRole(role: string | string[]): boolean;
/**
 * Hook to require authentication (throws if not authenticated)
 */
export declare function useRequireAuth(): UseAuthReturn;
export default useAuth;
