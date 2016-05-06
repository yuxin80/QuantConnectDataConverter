using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoilSimulater.Utilities
{
    public static class TimeUtilities
    {
        #region Fileds

        public static CultureInfo AppCulture = Thread.CurrentThread.CurrentCulture;

        public static String DateFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
        public static String TimeFormat = DateTimeFormatInfo.CurrentInfo.LongTimePattern;
        public static String FullTimeFormat = TimeFormat + ".fff";
        public static String PreciseTimeFormat = TimeFormat + ".ffff";
        public static String ParanoidTimeFormat = TimeFormat + ".FFFFFFF";
        public static String FullDateTimeFormat = DateFormat + " " + FullTimeFormat;
        public static String PreciseDateTimeFormat = DateFormat + " " + PreciseTimeFormat;

        #endregion

        #region Methods

        public static void UpdateTimeFormats()
        {
            AppCulture = Thread.CurrentThread.CurrentCulture;
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                DateFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                TimeFormat = DateTimeFormatInfo.CurrentInfo.LongTimePattern;
            }
            FullTimeFormat = TimeFormat + ".fff";
            PreciseTimeFormat = TimeFormat + ".ffff";
            ParanoidTimeFormat = TimeFormat + ".FFFFFFF";
            FullDateTimeFormat = DateFormat + " " + FullTimeFormat;
            PreciseDateTimeFormat = DateFormat + " " + PreciseTimeFormat;
        }

        public static DateTime SafeAddTime(DateTime originalTime, TimeSpan deltaTime)
        {
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                (deltaTime == TimeSpan.MinValue) || (deltaTime == TimeSpan.MaxValue) ||
                (deltaTime > DateTime.MaxValue.Subtract(originalTime)))
            {
                return originalTime;
            }

            return originalTime.Add(deltaTime);
        }

        public static DateTime SafeAddMinutes(DateTime originalTime, Double minutes)
        {
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                ((minutes < 0) && (-minutes > originalTime.Subtract(DateTime.MinValue).TotalMinutes)) ||
                ((minutes > 0) && (minutes > DateTime.MaxValue.Subtract(originalTime).TotalMinutes)) ||
                Double.IsInfinity(minutes) || Double.IsNaN(minutes))
            {
                return originalTime;
            }

            return originalTime.AddMinutes(minutes);
        }

        public static DateTime SafeAddHours(DateTime originalTime, Double hours)
        {
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                ((hours < 0) && (-hours > originalTime.Subtract(DateTime.MinValue).TotalHours)) ||
                ((hours > 0) && (hours > DateTime.MaxValue.Subtract(originalTime).TotalHours)) ||
                Double.IsInfinity(hours) || Double.IsNaN(hours))
            {
                return originalTime;
            }

            return originalTime.AddHours(hours);
        }

        public static DateTime SafeAddSeconds(DateTime originalTime, Double seconds)
        {
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                ((seconds < 0) && (-seconds > originalTime.Subtract(DateTime.MinValue).TotalSeconds)) ||
                ((seconds > 0) && (seconds > DateTime.MaxValue.Subtract(originalTime).TotalSeconds)) ||
                Double.IsInfinity(seconds) || Double.IsNaN(seconds))
            {
                return originalTime;
            }

            return originalTime.AddSeconds(seconds);
        }

        public static DateTime SafeAddSecondsWithTicks(DateTime originalTime, Decimal seconds)
        {
            Double dSeconds = Convert.ToDouble(seconds);
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                ((dSeconds < 0) && (-dSeconds > originalTime.Subtract(DateTime.MinValue).TotalSeconds)) ||
                ((dSeconds > 0) && (dSeconds > DateTime.MaxValue.Subtract(originalTime).TotalSeconds)) ||
                Double.IsInfinity(dSeconds) || Double.IsNaN(dSeconds))
            {
                return originalTime;
            }

            // Use AddTicks to avoid numerical error when the seconds is a small value
            return originalTime.AddTicks((long)Math.Round(seconds * 10000000.0M));
        }

        public static DateTime SafeAddTicks(DateTime originalTime, long ticks)
        {
            if ((ticks == 0) || (originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                ((ticks < 0) && (-ticks > originalTime.Subtract(DateTime.MinValue).Ticks)) ||
                ((ticks > 0) && (ticks > DateTime.MaxValue.Subtract(originalTime).Ticks)))
            {
                return originalTime;
            }

            return originalTime.AddTicks(ticks);
        }

        public static TimeSpan SafeSubtractTime(DateTime originalTime, DateTime timeToAdd)
        {
            if ((originalTime == DateTime.MinValue) || (originalTime == DateTime.MaxValue) ||
                (timeToAdd == DateTime.MinValue) || (timeToAdd == DateTime.MaxValue))
            {
                return new TimeSpan(DateTime.MaxValue.Ticks);
            }

            return originalTime.Subtract(timeToAdd);
        }

        public static Double SafeTimeToDouble(DateTime time)
        {
            try
            {
                return time.ToOADate();
            }
            catch (OverflowException)
            {
                return 0.0;
            }
        }

        public static DateTime SafeDoubleToTime(Double time)
        {
            try
            {
                return DateTime.FromOADate(time);
            }
            catch (ArgumentException)
            {
                return DateTime.MaxValue;
            }
        }

        public static Decimal GetMilliseconds(DateTime time)
        {
            return (Decimal)(time.Ticks % 10000000) / 10000;
        }

        public static DateTime CutMilliseconds(DateTime time)
        {
            return SafeAddSecondsWithTicks(time, -(Decimal)(time.Ticks % 10000000) / 10000000);
        }

        #endregion
    }
}
