/**
 * Types Index
 * Neo BPMS Admin Panel Core
 */
export * from './auth.types';
export * from './api.types';
export interface Dictionary<T = unknown> {
    [key: string]: T;
}
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type ValueOf<T> = T[keyof T];
export type DeepPartial<T> = {
    [P in keyof T]?: T[P] extends object ? DeepPartial<T[P]> : T[P];
};
export type AsyncFunction<T = void> = () => Promise<T>;
export type AsyncFunctionWithArgs<TArgs extends unknown[], TResult = void> = (...args: TArgs) => Promise<TResult>;
/**
 * Menu item definition
 */
export interface MenuItem {
    id: string;
    label: string;
    labelEn?: string;
    icon?: string;
    path?: string;
    permissions?: string[];
    roles?: string[];
    children?: MenuItem[];
    badge?: string | number;
    badgeColor?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
    isExternal?: boolean;
    isDivider?: boolean;
    isHidden?: boolean;
}
/**
 * Theme configuration
 */
export interface ThemeConfig {
    mode: 'light' | 'dark' | 'system';
    primaryColor: string;
    borderRadius: 'none' | 'sm' | 'md' | 'lg' | 'full';
    fontFamily: string;
    direction: 'ltr' | 'rtl';
}
/**
 * Language configuration
 */
export interface LanguageConfig {
    code: string;
    name: string;
    nativeName: string;
    direction: 'ltr' | 'rtl';
    dateFormat: string;
    numberFormat: string;
    currency: string;
}
/**
 * Notification types
 */
export interface Notification {
    id: string;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
    duration?: number;
    action?: {
        label: string;
        onClick: () => void;
    };
}
/**
 * Breadcrumb item
 */
export interface BreadcrumbItem {
    label: string;
    path?: string;
    icon?: string;
}
/**
 * Table column definition
 */
export interface TableColumn<T = unknown> {
    key: keyof T | string;
    label: string;
    sortable?: boolean;
    filterable?: boolean;
    width?: string | number;
    align?: 'left' | 'center' | 'right';
    render?: (value: unknown, row: T, index: number) => React.ReactNode;
    hidden?: boolean;
}
/**
 * Form field definition
 */
export interface FormField {
    name: string;
    label: string;
    type: 'text' | 'number' | 'email' | 'password' | 'select' | 'textarea' | 'checkbox' | 'radio' | 'date' | 'file';
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    options?: Array<{
        value: string | number;
        label: string;
    }>;
    validation?: {
        min?: number;
        max?: number;
        minLength?: number;
        maxLength?: number;
        pattern?: string;
        message?: string;
    };
}
