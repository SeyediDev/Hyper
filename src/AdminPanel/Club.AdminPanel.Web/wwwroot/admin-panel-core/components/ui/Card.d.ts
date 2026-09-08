import { HTMLAttributes, ReactNode } from 'react';

export interface CardProps extends HTMLAttributes<HTMLDivElement> {
    /** Card variant */
    variant?: 'default' | 'bordered' | 'elevated' | 'gradient';
    /** Padding size */
    padding?: 'none' | 'sm' | 'md' | 'lg';
    /** Hover effect */
    hoverable?: boolean;
    /** Card is clickable */
    clickable?: boolean;
}
export interface CardHeaderProps extends Omit<HTMLAttributes<HTMLDivElement>, 'title'> {
    /** Header title */
    title?: ReactNode;
    /** Header subtitle */
    subtitle?: ReactNode;
    /** Right side action */
    action?: ReactNode;
}
export interface CardFooterProps extends HTMLAttributes<HTMLDivElement> {
    /** Align content */
    align?: 'left' | 'center' | 'right' | 'between';
}
/**
 * Card Component
 */
export declare const Card: import('react').ForwardRefExoticComponent<CardProps & import('react').RefAttributes<HTMLDivElement>>;
/**
 * Card Header Component
 */
export declare const CardHeader: import('react').ForwardRefExoticComponent<CardHeaderProps & import('react').RefAttributes<HTMLDivElement>>;
/**
 * Card Body Component
 */
export declare const CardBody: import('react').ForwardRefExoticComponent<HTMLAttributes<HTMLDivElement> & import('react').RefAttributes<HTMLDivElement>>;
/**
 * Card Footer Component
 */
export declare const CardFooter: import('react').ForwardRefExoticComponent<CardFooterProps & import('react').RefAttributes<HTMLDivElement>>;
export default Card;
