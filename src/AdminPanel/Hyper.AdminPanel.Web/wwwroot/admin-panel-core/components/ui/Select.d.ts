import { ReactNode } from 'react';

export interface SelectOption<T = string> {
    value: T;
    label: string;
    icon?: ReactNode;
    disabled?: boolean;
    group?: string;
}
export interface SelectProps<T = string> {
    /** Options */
    options: SelectOption<T>[];
    /** Selected value */
    value?: T;
    /** Change handler */
    onChange?: (value: T | undefined) => void;
    /** Placeholder */
    placeholder?: string;
    /** Label */
    label?: string;
    /** Error message */
    error?: string;
    /** Helper text */
    helperText?: string;
    /** Disabled */
    disabled?: boolean;
    /** Clearable */
    clearable?: boolean;
    /** Searchable */
    searchable?: boolean;
    /** Full width */
    fullWidth?: boolean;
    /** Size */
    size?: 'sm' | 'md' | 'lg';
    /** Custom class */
    className?: string;
}
export declare function Select<T = string>({ options, value, onChange, placeholder, label, error, helperText, disabled, clearable, searchable, fullWidth, size, className, }: SelectProps<T>): import("react/jsx-runtime").JSX.Element;
export default Select;
