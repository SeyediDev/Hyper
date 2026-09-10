import { default as React } from 'react';
import { LiveMetric, TimeFrame } from '../types';

export interface LiveOperationsDashboardProps {
    /** API URL for fetching data */
    apiUrl?: string;
    /** Initial time frame */
    initialTimeFrame?: TimeFrame;
    /** Refresh interval in ms */
    refreshInterval?: number;
    /** Custom fetch function */
    fetchFn?: (timeFrame: TimeFrame) => Promise<LiveMetric[]>;
    /** Card click handler */
    onMetricClick?: (metric: LiveMetric) => void;
    /** Show category grouping */
    groupByCategory?: boolean;
    /** Custom title */
    title?: string;
    /** Custom subtitle */
    subtitle?: string;
    /** Additional header content */
    headerExtra?: React.ReactNode;
    /** Custom class name */
    className?: string;
}
export declare const LiveOperationsDashboard: React.FC<LiveOperationsDashboardProps>;
export default LiveOperationsDashboard;
