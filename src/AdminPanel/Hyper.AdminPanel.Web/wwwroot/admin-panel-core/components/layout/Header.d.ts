import { default as React } from 'react';

export interface Breadcrumb {
    label: string;
    path?: string;
}
export interface Notification {
    id: string;
    title: string;
    message: string;
    time: string;
    read?: boolean;
    type?: 'info' | 'success' | 'warning' | 'error';
}
export interface UserInfo {
    name: string;
    email?: string;
    avatar?: string;
    role?: string;
}
export interface HeaderProps {
    /** Current user info */
    user?: UserInfo;
    /** Breadcrumb navigation */
    breadcrumbs?: Breadcrumb[];
    /** Notifications list */
    notifications?: Notification[];
    /** Unread notifications count */
    unreadCount?: number;
    /** Current theme */
    theme?: 'light' | 'dark';
    /** Callback when theme changes */
    onThemeChange?: (theme: 'light' | 'dark') => void;
    /** Callback when user clicks logout */
    onLogout?: () => void;
    /** Callback when sidebar toggle is clicked */
    onMenuToggle?: () => void;
    /** Callback when notification is clicked */
    onNotificationClick?: (notification: Notification) => void;
    /** Callback when "View All Notifications" is clicked */
    onViewAllNotifications?: () => void;
    /** Custom actions to render in header */
    actions?: React.ReactNode;
    /** Custom class name */
    className?: string;
    /** Show menu toggle button */
    showMenuToggle?: boolean;
}
/**
 * Header component with user menu, notifications, and theme toggle
 */
export declare const Header: React.FC<HeaderProps>;
export default Header;
