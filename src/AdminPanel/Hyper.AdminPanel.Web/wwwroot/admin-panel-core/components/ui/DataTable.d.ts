import { ReactNode } from 'react';

export interface Column<T> {
    /** Unique column key */
    key: string;
    /** Column header */
    header: ReactNode;
    /** Accessor function or key path */
    accessor?: keyof T | ((row: T) => unknown);
    /** Custom cell renderer */
    render?: (value: unknown, row: T, index: number) => ReactNode;
    /** Column width */
    width?: string | number;
    /** Text alignment */
    align?: 'left' | 'center' | 'right';
    /** Sortable column */
    sortable?: boolean;
    /** Hidden column */
    hidden?: boolean;
}
export interface DataTableProps<T> {
    /** Table data */
    data: T[];
    /** Column definitions */
    columns: Column<T>[];
    /** Row key accessor */
    rowKey: keyof T | ((row: T) => string | number);
    /** Loading state */
    loading?: boolean;
    /** Empty state message */
    emptyMessage?: ReactNode;
    /** Striped rows */
    striped?: boolean;
    /** Hoverable rows */
    hoverable?: boolean;
    /** Compact size */
    compact?: boolean;
    /** Bordered style */
    bordered?: boolean;
    /** Sticky header */
    stickyHeader?: boolean;
    /** Row selection */
    selectable?: boolean;
    /** Selected row keys */
    selectedKeys?: Set<string | number>;
    /** Selection change callback */
    onSelectionChange?: (keys: Set<string | number>) => void;
    /** Row click callback */
    onRowClick?: (row: T, index: number) => void;
    /** Custom class name */
    className?: string;
    /** Max height with scroll */
    maxHeight?: string | number;
}
export declare function DataTable<T>({ data, columns, rowKey, loading, emptyMessage, striped, hoverable, compact, bordered, stickyHeader, selectable, selectedKeys, onSelectionChange, onRowClick, className, maxHeight, }: DataTableProps<T>): import("react/jsx-runtime").JSX.Element;
export default DataTable;
