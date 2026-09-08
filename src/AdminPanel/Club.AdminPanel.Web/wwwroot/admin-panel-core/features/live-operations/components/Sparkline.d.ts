import { default as React } from 'react';

export interface SparklineProps {
    data: number[];
    width?: number;
    height?: number;
    color?: 'primary' | 'success' | 'warning' | 'danger' | 'info' | string;
    showArea?: boolean;
    strokeWidth?: number;
    className?: string;
}
export declare const Sparkline: React.FC<SparklineProps>;
export default Sparkline;
