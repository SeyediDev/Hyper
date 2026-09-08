import { ReactNode } from 'react';
import { AuthContextValue, User, AuthConfig } from '../types';

/**
 * Auth Provider Props
 */
interface AuthProviderProps {
    children: ReactNode;
    config?: Partial<AuthConfig>;
    onAuthStateChange?: (isAuthenticated: boolean, user: User | null) => void;
    onSessionExpired?: () => void;
    onUnauthorized?: () => void;
}
/**
 * Auth Provider with Redux Provider
 */
export declare function AuthProvider(props: AuthProviderProps): import("react/jsx-runtime").JSX.Element;
/**
 * Hook to access auth context
 */
export declare function useAuthContext(): AuthContextValue;
export default AuthProvider;
