/**
 * Live Operations Types
 * Neo BPMS Admin Panel Core
 */
export type TimeFrame = '1m' | '5m' | '15m' | '1h' | '24h';
export interface TimeFrameOption {
    value: TimeFrame;
    label: string;
    labelEn: string;
    seconds: number;
}
export declare const TIME_FRAME_OPTIONS: TimeFrameOption[];
export type TrendDirection = 'up' | 'down' | 'stable';
export interface LiveMetric {
    id: string;
    name: string;
    nameEn?: string;
    value: number;
    previousValue?: number;
    unit?: string;
    format?: 'number' | 'currency' | 'percent';
    trend?: TrendDirection;
    trendPercent?: number;
    sparklineData?: number[];
    color?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
    icon?: string;
    category?: string;
}
export interface LiveOperationsData {
    metrics: LiveMetric[];
    lastUpdated: Date;
    timeFrame: TimeFrame;
}
export interface LiveOperationsFilter {
    timeFrame: TimeFrame;
    categories?: string[];
    refreshInterval?: number;
}
