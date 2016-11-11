using System;

namespace Gallery.Domain.ValueObjects
{
    public class Frame
    {
        public Filter Filter { get; set; }
        public int Duration { get; set; }
        public int Number { get; set; }
        public Binning Binning { get; set; }
        public DateTime[] Dates { get; set; }
        public int SensorTemperature { get; set; }
        public int DarkCount { get; set; }
        public int FlatCount { get; set; }
        public int BiasCount { get; set; }
    }
}
