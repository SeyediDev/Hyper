import { ReactNode } from 'react';

export type ModalSize = 'sm' | 'md' | 'lg' | 'xl' | 'full';
export interface ModalProps {
    /** Whether modal is open */
    isOpen: boolean;
    /** Callback when modal should close */
    onClose: () => void;
    /** Modal title */
    title?: ReactNode;
    /** Modal description/subtitle */
    description?: ReactNode;
    /** Modal content */
    children: ReactNode;
    /** Footer content */
    footer?: ReactNode;
    /** Modal size */
    size?: ModalSize;
    /** Close on backdrop click */
    closeOnBackdrop?: boolean;
    /** Close on escape key */
    closeOnEscape?: boolean;
    /** Show close button */
    showCloseButton?: boolean;
    /** Center modal vertically */
    centered?: boolean;
    /** Custom class for modal content */
    className?: string;
    /** Custom class for overlay */
    overlayClassName?: string;
}
/**
 * Modal Component
 */
export declare function Modal({ isOpen, onClose, title, description, children, footer, size, closeOnBackdrop, closeOnEscape, showCloseButton, centered, className, overlayClassName, }: ModalProps): import('react').ReactPortal | null;
export default Modal;
