import { default as React } from 'react';

export interface MenuItem {
    id: string;
    label: string;
    labelEn?: string;
    icon?: React.ReactNode | string;
    path?: string;
    children?: MenuItem[];
    badge?: string | number;
    badgeColor?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
    permission?: string;
    permissions?: string[];
    roles?: string[];
    divider?: boolean;
    isExternal?: boolean;
    isHidden?: boolean;
    isDivider?: boolean;
}
export interface SidebarProps {
    /** Menu items to display */
    items: MenuItem[];
    /** Whether sidebar is collapsed */
    collapsed?: boolean;
    /** Callback when collapse state changes */
    onCollapsedChange?: (collapsed: boolean) => void;
    /** Logo component or image URL */
    logo?: React.ReactNode | string;
    /** Logo text when expanded */
    logoText?: string;
    /** Footer content */
    footer?: React.ReactNode;
    /** Custom class name */
    className?: string;
    /** Check if user has permission */
    hasPermission?: (permission: string) => boolean;
}
/**
 * Sidebar navigation component with collapsible menu
 */
export declare const Sidebar: React.FC<SidebarProps>;
export default Sidebar;
