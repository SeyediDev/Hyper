import { TimeFrame, LiveMetric, LiveOperationsData } from '../types';

export interface UseLiveOperationsOptions {
    /** API base URL */
    apiUrl?: string;
    /** Initial time frame */
    initialTimeFrame?: TimeFrame;
    /** Auto refresh interval in ms (0 to disable) */
    refreshInterval?: number;
    /** Custom fetch function */
    fetchFn?: (timeFrame: TimeFrame) => Promise<LiveMetric[]>;
}
export interface UseLiveOperationsReturn {
    data: LiveOperationsData | null;
    isLoading: boolean;
    error: string | null;
    timeFrame: TimeFrame;
    setTimeFrame: (timeFrame: TimeFrame) => void;
    isPaused: boolean;
    togglePause: () => void;
    refresh: () => Promise<void>;
    lastUpdated: Date | null;
}
export declare const useLiveOperations: ({ apiUrl, initialTimeFrame, refreshInterval, fetchFn, }?: UseLiveOperationsOptions) => UseLiveOperationsReturn;
export default useLiveOperations;
