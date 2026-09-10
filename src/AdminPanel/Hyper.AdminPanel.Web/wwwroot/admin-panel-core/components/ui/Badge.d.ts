import { HTMLAttributes, ReactNode } from 'react';

export type BadgeVariant = 'default' | 'primary' | 'success' | 'warning' | 'danger' | 'info';
export type BadgeSize = 'sm' | 'md' | 'lg';
export interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
    /** Badge variant */
    variant?: BadgeVariant;
    /** Badge size */
    size?: BadgeSize;
    /** Dot indicator (no text) */
    dot?: boolean;
    /** Pulsing animation */
    pulse?: boolean;
    /** Icon before text */
    icon?: ReactNode;
    /** Removable badge */
    removable?: boolean;
    /** Remove callback */
    onRemove?: () => void;
}
/**
 * Badge Component
 */
export declare function Badge({ variant, size, dot, pulse, icon, removable, onRemove, className, children, ...props }: BadgeProps): import("react/jsx-runtime").JSX.Element;
export default Badge;
