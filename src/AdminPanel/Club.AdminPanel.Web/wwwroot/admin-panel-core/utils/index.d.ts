/**
 * Utility Functions
 * Neo BPMS Admin Panel Core
 */
/**
 * Format number to Persian locale
 */
export declare function formatNumber(value: number, locale?: string): string;
/**
 * Format currency
 */
export declare function formatCurrency(value: number, currency?: string, locale?: string): string;
/**
 * Format file size
 */
export declare function formatFileSize(bytes: number): string;
/**
 * Delay execution
 */
export declare function delay(ms: number): Promise<void>;
/**
 * Debounce function
 */
export declare function debounce<T extends (...args: unknown[]) => unknown>(fn: T, wait: number): (...args: Parameters<T>) => void;
/**
 * Throttle function
 */
export declare function throttle<T extends (...args: unknown[]) => unknown>(fn: T, limit: number): (...args: Parameters<T>) => void;
/**
 * Deep clone object
 */
export declare function deepClone<T>(obj: T): T;
/**
 * Check if object is empty
 */
export declare function isEmpty(obj: unknown): boolean;
/**
 * Generate unique ID
 */
export declare function generateId(prefix?: string): string;
/**
 * Safely access nested object property
 */
export declare function get<T>(obj: unknown, path: string, defaultValue?: T): T | undefined;
/**
 * Convert object to query string
 */
export declare function toQueryString(params: Record<string, unknown>): string;
/**
 * Parse query string to object
 */
export declare function parseQueryString(queryString: string): Record<string, string | string[]>;
/**
 * Truncate text with ellipsis
 */
export declare function truncate(text: string, maxLength: number, suffix?: string): string;
/**
 * Capitalize first letter
 */
export declare function capitalize(text: string): string;
/**
 * Convert to slug
 */
export declare function slugify(text: string): string;
/**
 * Storage helper with JSON support
 */
export declare const storage: {
    get<T>(key: string, defaultValue?: T): T | undefined;
    set<T>(key: string, value: T): void;
    remove(key: string): void;
    clear(): void;
};
/**
 * Class name utility (like clsx but simpler)
 */
export declare function cn(...classes: (string | undefined | null | false)[]): string;
