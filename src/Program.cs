using System;
using System.Collections.ObjectModel;

namespace Maptz.TimeZone.Tool
{
    record TimeZoneDisplayItem
    {
        public string ShortName { get; init; }
        public string LongName { get; init; }
        public string CurrentTime { get; init; }
        public TimeSpan OffsetFromUTC { get; init; }
        public bool IsInDaylightSavings { get; init; }
        public TimeSpan? OffsetFromRelative { get; internal set; }
        public TimeZoneInfo RelativeTo { get; internal set; }
        
    }



    class Program
    {
        static void Main(string[] args)
        {
            TimeZoneTableFormatter formatter = new();
            formatter.Print();
        }
    }

}
