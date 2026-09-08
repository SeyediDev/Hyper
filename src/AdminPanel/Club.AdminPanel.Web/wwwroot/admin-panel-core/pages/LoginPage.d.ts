/**
 * Login Page Component
 * Neo BPMS Admin Panel Core
 *
 * A modern, beautiful login page with RTL support.
 */
export interface LoginPageProps {
    /** Logo component or image URL */
    logo?: React.ReactNode | string;
    /** Application title */
    title?: string;
    /** Subtitle or description */
    subtitle?: string;
    /** Background image or gradient */
    backgroundImage?: string;
    /** Custom footer content */
    footer?: React.ReactNode;
    /** Path to redirect after login */
    redirectPath?: string;
    /** Show remember me checkbox */
    showRememberMe?: boolean;
    /** Show forgot password link */
    showForgotPassword?: boolean;
    /** Forgot password URL */
    forgotPasswordUrl?: string;
    /** Custom class name */
    className?: string;
    /** On login success callback */
    onLoginSuccess?: () => void;
    /** On login error callback */
    onLoginError?: (error: string) => void;
}
/**
 * Modern Login Page Component
 */
export declare function LoginPage({ logo, title, subtitle, backgroundImage, footer, redirectPath, showRememberMe, showForgotPassword, forgotPasswordUrl, className, onLoginSuccess, onLoginError, }: LoginPageProps): import("react/jsx-runtime").JSX.Element;
export default LoginPage;
