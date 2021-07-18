using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Maptz.TimeZone.Tool
{

    class TimeZoneTableFormatter
    {
        private IEnumerable<TimeZoneDisplayItem> PrepareItems()
        {
            //Get the timezones installed on the system. 
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            //foreach(var z in zones)
            //{
            //    Console.WriteLine($"{z.DisplayName}");
            //}
            //return new TimeZoneDisplayItem[0];

            TimeZoneInfo FindTimeZone(string city)
            {
                //return zones.FirstOrDefault(p => string.Equals(p.DisplayName, city, StringComparison.OrdinalIgnoreCase));
                return zones.FirstOrDefault(p => p.DisplayName.Contains(city, StringComparison.OrdinalIgnoreCase));
            }


            var relativeTo = TimeZoneInfo.Local;
            var retval = new List<TimeZoneDisplayItem>();
            retval.Add(ConvertToDisplayItem(TimeZoneInfo.Local, relativeTo, "Melbourne"));
            //retval.Add(ConvertToDisplayItem(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"), relativeTo));
            retval.Add(ConvertToDisplayItem(FindTimeZone("Pacific Time (US & Canada)"), relativeTo, "LA"));
            retval.Add(ConvertToDisplayItem(FindTimeZone("Eastern Time (US & Canada)"), relativeTo, "NYC"));
            retval.Add(ConvertToDisplayItem(FindTimeZone("Dublin, Edinburgh, Lisbon, London"), relativeTo, "London"));
            retval.Add(ConvertToDisplayItem(FindTimeZone("Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"), relativeTo, "Barcelona"));
            retval.Add(ConvertToDisplayItem(FindTimeZone("Kabul"), relativeTo, "Kabul"));
            return retval.ToArray();

        }

        public void Print()
        {
            string istr(double i)
            {
                var sign = i < 0 ? "" : "+";
                var ret = i.ToString();
                if (i >= 0) ret = sign + ret;
                while (ret.Length < 4)
                {
                    ret = " " + ret;
                }
                return ret;
            }
            AnsiConsole.Render(new FigletText("Timezones")
        .LeftAligned()
        .Color(Color.Red));

            //var rule = new Rule("[red]Timezones[/]");
            var rule = new Rule();
            rule.Style = Style.Parse("red dim");
            AnsiConsole.Render(rule);

            var barChar = new BarChart()
                .Width(90)
                .HideValues()
                .RightAlignLabel();

            var items = PrepareItems();
            items = items.OrderBy(p => p.OffsetFromRelative.Value);
            var minV = items.Min(item => item.OffsetFromRelative.HasValue ? item.OffsetFromRelative.Value.TotalHours : 0.0);
            minV = minV - 5;
            foreach (var item in items)
            {
                var relHours = item.OffsetFromRelative.HasValue ? item.OffsetFromRelative.Value.TotalHours : 0.0;
                //AnsiConsole.MarkupLine($"{item.Name} {item.CurrentTime} {relHours}");
                Color col = relHours == 0 ? Color.Green : Color.Yellow;
                var hourDiff = relHours;
                var txt = $"[green]{item.ShortName}[/] - {item.CurrentTime} ({istr(hourDiff)} HR)";
                barChar.AddItem(txt, relHours - minV, col);
            }
            AnsiConsole.Render(barChar);
       
        }

        private TimeZoneDisplayItem ConvertToDisplayItem(TimeZoneInfo timeZoneInfo, TimeZoneInfo relativeTo = null, string shortName = null)
        {
            var utcNow = DateTime.UtcNow;
            var currentTime = TimeZoneInfo.ConvertTime(utcNow, TimeZoneInfo.Utc, timeZoneInfo);
            TimeSpan? relative = relativeTo != null ? currentTime - TimeZoneInfo.ConvertTime(utcNow, TimeZoneInfo.Utc, relativeTo) : null;
            return new TimeZoneDisplayItem
            {
                ShortName = shortName,
                CurrentTime = currentTime.ToString("dd/MM hh:mm tt"),
                IsInDaylightSavings = timeZoneInfo.IsDaylightSavingTime(utcNow),
                LongName = timeZoneInfo.DisplayName,
                OffsetFromUTC = timeZoneInfo.BaseUtcOffset,
                OffsetFromRelative = relative,
                RelativeTo = relativeTo
            };
        }
    }

    class TimeZoneDebugger
    {

        public void Debug()
        {
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo zone in zones)
            {
                Console.WriteLine(zone.DisplayName);
            }


            Console.WriteLine();
            // Display the current time zone name.  
            Console.WriteLine("Local time zone: {0}\n", TimeZoneInfo.Local.DisplayName);
            Console.WriteLine("In Daylight savings: {0}\n", TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.UtcNow));
            Console.WriteLine();
            // Display the current time zone name.  
            Console.WriteLine("Local time zone Name : {0}\n", TimeZoneInfo.Local.DisplayName);
            //Display the current time zone standard name.  
            Console.WriteLine("Local time zone Standard Name : {0}\n", TimeZoneInfo.Local.StandardName);
            //Display the current time zone Daylight name.  
            Console.WriteLine("Local time zone DayLight Name : {0}\n", TimeZoneInfo.Local.DaylightName);
            //Find whether the currenctly selected Time zone support the day light saving time or not  
            Console.WriteLine("Local time zone support Day Light Saving Time : {0}\n", TimeZoneInfo.Local.SupportsDaylightSavingTime);

            //apply base UTC offset  
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            Console.WriteLine("The {0} time zone is {1}:{2} {3} than Coordinated Universal Time.",
            localZone.StandardName,
            Math.Abs(localZone.BaseUtcOffset.Hours),
            Math.Abs(localZone.BaseUtcOffset.Minutes),
            (localZone.BaseUtcOffset >= TimeSpan.Zero) ? "later" : "earlier");

            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var newDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, britishZone);
            Console.WriteLine($"Time now in London is {newDate}. In savingstime {britishZone.IsDaylightSavingTime(DateTime.UtcNow)}");

            //NB On the attached png, title font it Gilroy Heavy, secondary font is Stem Text Stem Text

        }
    }
}