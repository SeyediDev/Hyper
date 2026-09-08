import { ReactNode } from 'react';

export interface StatCardProps {
    title: string;
    value: string | number;
    change?: number;
    changeLabel?: string;
    icon?: ReactNode;
    color?: 'blue' | 'green' | 'purple' | 'orange' | 'red' | 'cyan';
    onClick?: () => void;
}
export interface QuickActionProps {
    label: string;
    icon: ReactNode;
    onClick: () => void;
    color?: string;
}
export interface ActivityItem {
    id: string;
    title: string;
    description?: string;
    time: string;
    icon?: ReactNode;
    type?: 'success' | 'warning' | 'error' | 'info';
}
export interface DashboardPageProps {
    /** Page title */
    title?: string;
    /** Welcome message */
    welcomeMessage?: string;
    /** User name for greeting */
    userName?: string;
    /** Stat cards to display */
    stats?: StatCardProps[];
    /** Quick action buttons */
    quickActions?: QuickActionProps[];
    /** Recent activity items */
    recentActivity?: ActivityItem[];
    /** Custom widgets */
    widgets?: ReactNode;
    /** Loading state */
    isLoading?: boolean;
    /** Custom class name */
    className?: string;
}
/**
 * Dashboard Page Component
 */
export declare function DashboardPage({ title, welcomeMessage, userName, stats, quickActions, recentActivity, widgets, isLoading, className, }: DashboardPageProps): import("react/jsx-runtime").JSX.Element;
export default DashboardPage;
