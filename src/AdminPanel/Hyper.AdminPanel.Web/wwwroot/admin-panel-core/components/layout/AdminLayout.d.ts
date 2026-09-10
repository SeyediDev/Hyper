import { default as React } from 'react';
import { MenuItem } from './Sidebar';
import { Breadcrumb, Notification, UserInfo } from './Header';

export interface AdminLayoutProps {
    /** Main content */
    children: React.ReactNode;
    /** Menu items for sidebar */
    menuItems: MenuItem[];
    /** Current user info */
    user?: UserInfo;
    /** Breadcrumb navigation */
    breadcrumbs?: Breadcrumb[];
    /** Notifications list */
    notifications?: Notification[];
    /** Unread notifications count */
    unreadCount?: number;
    /** Logo component or image URL */
    logo?: React.ReactNode | string;
    /** Logo text */
    logoText?: string;
    /** Sidebar footer content */
    sidebarFooter?: React.ReactNode;
    /** Header custom actions */
    headerActions?: React.ReactNode;
    /** Current theme */
    theme?: 'light' | 'dark';
    /** Callback when theme changes */
    onThemeChange?: (theme: 'light' | 'dark') => void;
    /** Callback when user clicks logout */
    onLogout?: () => void;
    /** Callback when notification is clicked */
    onNotificationClick?: (notification: Notification) => void;
    /** Callback when "View All Notifications" is clicked */
    onViewAllNotifications?: () => void;
    /** Check if user has permission */
    hasPermission?: (permission: string) => boolean;
    /** Initial sidebar collapsed state */
    defaultCollapsed?: boolean;
    /** Custom class names */
    className?: string;
    sidebarClassName?: string;
    headerClassName?: string;
    contentClassName?: string;
}
/**
 * Main admin panel layout with sidebar and header
 */
export declare const AdminLayout: React.FC<AdminLayoutProps>;
export default AdminLayout;
