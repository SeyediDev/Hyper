import { ReactNode } from 'react';

export interface DropdownItem {
    key: string;
    label: ReactNode;
    icon?: ReactNode;
    disabled?: boolean;
    danger?: boolean;
    divider?: boolean;
}
export interface DropdownProps {
    /** Trigger element */
    trigger: ReactNode;
    /** Dropdown items */
    items: DropdownItem[];
    /** Item click handler */
    onItemClick?: (item: DropdownItem) => void;
    /** Placement */
    placement?: 'bottom-start' | 'bottom-end' | 'top-start' | 'top-end';
    /** Min width */
    minWidth?: number;
    /** Disabled state */
    disabled?: boolean;
    /** Custom class */
    className?: string;
}
export interface DropdownMenuProps {
    /** Open state */
    isOpen: boolean;
    /** Items */
    items: DropdownItem[];
    /** Click handler */
    onItemClick: (item: DropdownItem) => void;
    /** Close handler */
    onClose: () => void;
    /** Trigger element ref for positioning */
    triggerRef: React.RefObject<HTMLElement>;
    /** Placement */
    placement: 'bottom-start' | 'bottom-end' | 'top-start' | 'top-end';
    /** Min width */
    minWidth: number;
}
export declare function Dropdown({ trigger, items, onItemClick, placement, minWidth, disabled, className, }: DropdownProps): import("react/jsx-runtime").JSX.Element;
export default Dropdown;
