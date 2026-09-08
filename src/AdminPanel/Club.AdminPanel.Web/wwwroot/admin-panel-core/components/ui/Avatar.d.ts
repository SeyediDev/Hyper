import { ImgHTMLAttributes } from 'react';

export type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl';
export type AvatarShape = 'circle' | 'square';
export interface AvatarProps extends Omit<ImgHTMLAttributes<HTMLImageElement>, 'size'> {
    /** Image source */
    src?: string;
    /** Alt text / name for initials */
    name?: string;
    /** Avatar size */
    size?: AvatarSize;
    /** Avatar shape */
    shape?: AvatarShape;
    /** Show online indicator */
    online?: boolean;
    /** Custom background for initials */
    color?: string;
}
export interface AvatarGroupProps {
    /** Maximum visible avatars */
    max?: number;
    /** Avatar size */
    size?: AvatarSize;
    /** Children (Avatar components) */
    children: React.ReactNode;
    /** Custom class */
    className?: string;
}
/**
 * Avatar Component
 */
export declare function Avatar({ src, name, size, shape, online, color, className, alt, ...props }: AvatarProps): import("react/jsx-runtime").JSX.Element;
/**
 * Avatar Group Component
 */
export declare function AvatarGroup({ max, size, children, className, }: AvatarGroupProps): import("react/jsx-runtime").JSX.Element;
export default Avatar;
