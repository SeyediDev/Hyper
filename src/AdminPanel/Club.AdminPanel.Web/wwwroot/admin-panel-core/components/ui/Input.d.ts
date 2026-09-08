import { InputHTMLAttributes, ReactNode } from 'react';

export type InputSize = 'sm' | 'md' | 'lg';
export interface InputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'size'> {
    /** Input label */
    label?: string;
    /** Helper text below input */
    helperText?: string;
    /** Error message */
    error?: string;
    /** Input size */
    size?: InputSize;
    /** Left icon/addon */
    leftIcon?: ReactNode;
    /** Right icon/addon */
    rightIcon?: ReactNode;
    /** Full width input */
    fullWidth?: boolean;
    /** Show password toggle for password inputs */
    showPasswordToggle?: boolean;
}
/**
 * Input Component
 */
export declare const Input: import('react').ForwardRefExoticComponent<InputProps & import('react').RefAttributes<HTMLInputElement>>;
export default Input;
