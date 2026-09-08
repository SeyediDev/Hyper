import { default as React } from 'react';

export interface LiveOperationsPageProps {
    /** API URL */
    apiUrl?: string;
    /** Refresh interval */
    refreshInterval?: number;
}
export declare const LiveOperationsPage: React.FC<LiveOperationsPageProps>;
export default LiveOperationsPage;
