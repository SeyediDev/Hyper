/**
 * 404 Not Found Page
 * Neo BPMS Admin Panel Core
 */
export interface NotFoundPageProps {
    /** Title text */
    title?: string;
    /** Description text */
    description?: string;
    /** Show back button */
    showBackButton?: boolean;
    /** Show home button */
    showHomeButton?: boolean;
    /** Home path */
    homePath?: string;
    /** Custom illustration */
    illustration?: React.ReactNode;
    /** Custom class name */
    className?: string;
}
/**
 * 404 Not Found Page Component
 */
export declare function NotFoundPage({ title, description, showBackButton, showHomeButton, homePath, illustration, className, }: NotFoundPageProps): import("react/jsx-runtime").JSX.Element;
export default NotFoundPage;
