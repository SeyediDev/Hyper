import { default as React } from 'react';
import { LiveMetric } from '../types';

export interface LiveMetricCardProps {
    metric: LiveMetric;
    loading?: boolean;
    onClick?: () => void;
    className?: string;
}
export declare const LiveMetricCard: React.FC<LiveMetricCardProps>;
export default LiveMetricCard;
