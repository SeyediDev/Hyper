import { default as React } from 'react';
import { TimeFrame } from '../types';

export interface TimeFrameSelectorProps {
    value: TimeFrame;
    onChange: (value: TimeFrame) => void;
    size?: 'sm' | 'md' | 'lg';
    showLabels?: boolean;
    className?: string;
}
export declare const TimeFrameSelector: React.FC<TimeFrameSelectorProps>;
export default TimeFrameSelector;
