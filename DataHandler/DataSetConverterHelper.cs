using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public static class DataSetConverterHelper
    {
        private const double _scaleFactor = 1.0 / 10000.0;

        public static string ConvertToDailyHourData(this DataSet data, List<string> columnMapping, DateTimeRoundSolutions roundSolution = DateTimeRoundSolutions.day)
        {
            StringBuilder sb = new StringBuilder();
            var table = data.Tables["DefaultTable"];
            List<double[]> dataCache = new List<double[]>();
            DateTime currentDate = DateTime.MinValue;
            foreach (var obj in table.Rows)
            {
                var row = obj as DataRow;
                double[] valueTemp;
                DateTime dateTime = ConvertRow(columnMapping, row, out valueTemp);
                dataCache.Add(valueTemp);
                var date = RoundDateTime(dateTime, roundSolution);
                if (currentDate == DateTime.MinValue)
                {
                    currentDate = date;
                }

                if (date != currentDate)
                {
                    //Consolidate the data
                    double open, high, low, close, quant;
                    ConsolidateData(dataCache, out open, out high, out low, out close, out quant);

                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", currentDate.ToString("yyyyMMdd HH:mm"), open, high, low, close, quant));
                    currentDate = date;
                    dataCache.Clear();
                }
            }

            if (dataCache.Count > 0)
            {
                double open, high, low, close, quant;
                ConsolidateData(dataCache, out open, out high, out low, out close, out quant);

                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", currentDate.ToString("yyyMMdd HH:mm"), open, high, low, close, quant));
            }

            return sb.ToString();
        }

        private static DateTime RoundDateTime(DateTime dateTime, DateTimeRoundSolutions roundSolution)
        {
            var date = dateTime.Date;
            switch (roundSolution)
            {
                case DateTimeRoundSolutions.day:
                    return date;
                case DateTimeRoundSolutions.hour:
                    return date.AddHours(dateTime.Hour);
                case DateTimeRoundSolutions.minute:
                    return date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute);
                case DateTimeRoundSolutions.second:
                    return date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute).AddSeconds(dateTime.Second);
                default:
                    return dateTime;
            };
        }

        public static Dictionary<DateTime, string> ConvertToHighFreqData(this DataSet data, List<string> columnMapping, DateTimeRoundSolutions roundSolution)
        {
            DateTime currentDate = DateTime.MinValue;
            var result = new Dictionary<DateTime, string>();
            var table = data.Tables["DefaultTable"];
            DateTime timeTemp = DateTime.MinValue;
            List<double[]> dataTemp = new List<double[]>();
            StringBuilder sb = new StringBuilder();
            foreach (var obj in table.Rows)
            {
                var row = obj as DataRow;
                double[] rowData;
                var dateTime = ConvertRow(columnMapping, row, out rowData);
                var date = RoundDateTime(dateTime, DateTimeRoundSolutions.day);
                var dateTimeRounded = RoundDateTime(dateTime, roundSolution);
                if (currentDate == DateTime.MinValue)
                {
                    currentDate = date;
                }

                if (timeTemp == DateTime.MinValue)
                {
                    timeTemp = dateTimeRounded;
                }
                if (timeTemp != dateTimeRounded)
                {
                    double open, close, low, high, quant;
                    ConsolidateData(dataTemp, out open, out high, out low, out close, out quant);
                    sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", (dateTimeRounded - currentDate).Milliseconds, open, high, low, close, quant));
                    dataTemp.Clear();
                }
                if (date != currentDate)
                {
                    result.Add(currentDate, sb.ToString());
                    sb.Clear();
                    currentDate = date;
                }
            }

            return result;
        }

        private static void ConsolidateData(List<double[]> dataCache, out double open, out double high, out double low, out double close, out double quant)
        {
            open = dataCache[0][0];
            high = dataCache.Select(i => i[1]).Max();
            low = dataCache.Select(i => i[2]).Min();
            close = dataCache[dataCache.Count - 1][3];
            quant = dataCache.Select(i => i[4]).Sum();
        }

        private static DateTime ConvertRow(List<string> columnMapping, DataRow row, out double[] valueTemp)
        {
            var dateTime = DateTime.Parse(row[columnMapping[0]].ToString());
            var open = Convert.ToDouble(row[columnMapping[1]]) / _scaleFactor;
            var high = Convert.ToDouble(row[columnMapping[2]]) / _scaleFactor;
            var low = Convert.ToDouble(row[columnMapping[3]]) / _scaleFactor;
            var close = Convert.ToDouble(row[columnMapping[4]]) / _scaleFactor;
            long quant = 100;
            if (columnMapping.Count > 5)
                quant = Convert.ToInt64(row[columnMapping[5]]);
            valueTemp = new[] { open, high, low, close, quant };
            return dateTime;
        }

    }

    public enum DateTimeRoundSolutions
    {
        none,
        second,
        minute,
        hour,
        day
    }

}
