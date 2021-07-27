﻿namespace UtilityWpf.PanelDemo
{
    struct MinMax<T>
    {
        public MinMax(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Min { get; }

        public T Max { get; }
    }
}
