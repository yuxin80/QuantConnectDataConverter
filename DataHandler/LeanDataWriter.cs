using CoilSimulater.Utilities;
using DataHandler;
using QuantConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public class LeanDataWriter : ObjectBase
    {
        public ConverterParameters Parameters
        {
            get { return GetValue(() => Parameters); }
            set { SetValue(() => Parameters, value); }
        }

        public DataSet Data
        {
            get { return GetValue(() => Data); }
            set { SetValue(() => Data, value); }
        }

        /// <summary>
        /// The mapping of the columns in the DataSet to the properties in Lean's data files
        /// </summary>
        public List<string> HeaderMapping
        {
            get { return GetValue(() => HeaderMapping); }
            set { SetValue(() => HeaderMapping, value); }
        }

        public bool SaveData(out string diagnose)
        {
            diagnose = string.Empty;
            if (Parameters == null)
            {
                diagnose = "No convertion parameter is assigned to.";
                return false;
            }

            if (Data == null)
            {
                diagnose = "No data is imported.";
                return false;
            }

            if (HeaderMapping == null || HeaderMapping.Count < 5)
            {
                diagnose = "Not enough columns selected.";
                return false;
            }

            Boolean result = true;
            switch (Parameters.DataType)
            {
                case DataTypes.daily:
                case DataTypes.hour:
                    result = SaveLowFreqData(out diagnose);
                    break;
                default:
                    result = SaveHighFreqData(out diagnose);
                    break;
            };

            return result;
        }

        private bool SaveHighFreqData(out string diagnose)
        {
            diagnose = string.Empty;
            try
            {
                string dir = Path.Combine(Parameters.DestFolderName, Parameters.StockType.ToString(), Parameters.StockMarket.ToString(), Parameters.DataType.ToString(), Parameters.StockSymbol);

                //Seperate file by date
                var solution = Parameters.DataType == DataTypes.minute ? DateTimeRoundSolutions.minute : (Parameters.DataType == DataTypes.second ? DateTimeRoundSolutions.second : DateTimeRoundSolutions.none);

                Dictionary<DateTime, string> data = Data.ConvertToHighFreqData(HeaderMapping, solution);

                foreach (var d in data)
                {
                    var csvFileName = string.Format("{0}_{1}_second_trade.csv", d.Key.ToString("yyyyMMdd"), Parameters.StockSymbol);
                    var zipFileName = string.Format("{0}_trade.zip", d.Key.ToString("yyyyMMdd"));
                    zipFileName = Path.Combine(dir, zipFileName);
                    var dict = new Dictionary<string, string>();
                    dict.Add(csvFileName, d.Value);
                    Compression.ZipData(zipFileName, dict);
                }

                return true;
            }
            catch (Exception ex)
            {
                diagnose = ex.Message;
                return false;
            }
        }

        private bool SaveLowFreqData(out string diagnose)
        {
            diagnose = string.Empty;
            try
            {
                string dir = Path.Combine(Parameters.DestFolderName, Parameters.StockType.ToString(), Parameters.StockMarket.ToString(), Parameters.DataType.ToString());
                string fileName = Parameters.StockSymbol;
                var roundToDate = Parameters.DataType == DataTypes.daily ? DateTimeRoundSolutions.day : DateTimeRoundSolutions.hour;
                string value = Data.ConvertToDailyHourData(HeaderMapping, roundToDate);
                string csvFileName = string.Format("{0}.csv", fileName);
                string zipFileName = string.Format("{0}.zip", fileName);
                zipFileName = Path.Combine(dir, zipFileName);
                var dict = new Dictionary<string, string>();
                dict.Add(csvFileName, value);
                //TODO: Add the function to merge with the existing data
                Compression.ZipData(zipFileName, dict);
                return true;
            }
            catch (Exception ex)
            {
                diagnose = ex.Message;
                return false;
            }
        }
    }
}
