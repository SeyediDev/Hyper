/**
 * Authentication & Authorization Types
 * Neo BPMS Admin Panel Core
 */
/**
 * User identity from backend
 */
export interface User {
    id: number;
    username: string;
    displayName: string;
    email?: string;
    avatar?: string;
    culture: string;
    isAdmin: boolean;
    roles: string[];
    permissions: Permission[];
    metadata?: Record<string, unknown>;
}
/**
 * Permission definition
 */
export interface Permission {
    id: string;
    name: string;
    description?: string;
    category?: string;
}
/**
 * Role definition
 */
export interface Role {
    id: string;
    name: string;
    displayName: string;
    description?: string;
    permissions: string[];
    isSystem?: boolean;
}
/**
 * Login request payload
 */
export interface LoginRequest {
    username: string;
    password: string;
    rememberMe?: boolean;
    captchaToken?: string;
}
/**
 * Login response from server
 */
export interface LoginResponse {
    success: boolean;
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    user: User;
    message?: string;
}
/**
 * Refresh token request
 */
export interface RefreshTokenRequest {
    refreshToken: string;
}
/**
 * Refresh token response
 */
export interface RefreshTokenResponse {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
}
/**
 * Authentication state in Redux store
 */
export interface AuthState {
    user: User | null;
    accessToken: string | null;
    refreshToken: string | null;
    expiresAt: number | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
    lastActivity: number | null;
}
/**
 * Token payload (decoded JWT)
 */
export interface TokenPayload {
    sub: string;
    name: string;
    email?: string;
    roles: string[];
    permissions: string[];
    exp: number;
    iat: number;
    iss: string;
    aud: string;
}
/**
 * Auth context value
 */
export interface AuthContextValue {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
    login: (credentials: LoginRequest) => Promise<LoginResponse>;
    logout: () => Promise<void>;
    refreshToken: () => Promise<void>;
    hasPermission: (permission: string | string[]) => boolean;
    hasRole: (role: string | string[]) => boolean;
    updateUser: (user: Partial<User>) => void;
}
/**
 * Protected route props
 */
export interface ProtectedRouteProps {
    children: React.ReactNode;
    requiredPermissions?: string[];
    requiredRoles?: string[];
    requireAll?: boolean;
    fallback?: React.ReactNode;
    redirectTo?: string;
}
/**
 * Auth configuration
 */
export interface AuthConfig {
    loginEndpoint: string;
    logoutEndpoint: string;
    refreshEndpoint: string;
    userEndpoint: string;
    tokenKey: string;
    refreshTokenKey: string;
    sessionTimeout: number;
    autoRefreshBuffer: number;
    persistSession: boolean;
    redirectAfterLogin: string;
    redirectAfterLogout: string;
    unauthorizedRedirect: string;
}
/**
 * Password change request
 */
export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
}
/**
 * Session info
 */
export interface SessionInfo {
    id: string;
    userId: number;
    ipAddress: string;
    userAgent: string;
    createdAt: string;
    lastActivity: string;
    expiresAt: string;
    isCurrent: boolean;
}
