using CoilSimulater.Utilities;
using QuantConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataHandler
{
    public class QCDataExporter :ObjectBase
    {
        public ConverterParameters Parameters
        {
            get { return GetValue<ConverterParameters>(() => this.Parameters); }
            set { SetValue<ConverterParameters>(() => this.Parameters, value); }
        }

        public DataSet Data
        {
            get { return GetValue<DataSet>(() => this.Data); }
            set { SetValue<DataSet>(() => this.Data, value); }
        }

        /// <summary>
        /// The mapping of the columns in the dataset object. In the sequency of Time, open, high, low, close and quantity
        /// </summary>
        public List<String> ColumnMapping
        {
            get { return GetValue<List<string>>(() => ColumnMapping); }
            set { SetValue<List<string>>(() => ColumnMapping, value); }
        }

        public object Compressor { get; private set; }

        public Boolean SaveData(out string diagnose)
        {
            diagnose = string.Empty;
            if (Data == null)
            {
                diagnose = "no data is set to save security data";
                return false;
            }

            if(Parameters == null)
            {
                throw new ArgumentNullException("No parameter is set.");
            }

            string folder = Parameters.DestFolderName;

            folder = Path.Combine(folder, Parameters.StockType.ToString(), Parameters.StockMarket.ToString(), Parameters.DataType.ToString());

            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //switch among stock types
            switch(Parameters.StockType)
            {
                case StockTypes.equity:
                    SaveEquity(folder);
                    break;
                case StockTypes.forex:
                    SaveForex(folder);
                    break;
            }

            return true;
        }

        private void SaveForex(string folder)
        {
           
        }

        private void SaveEquity(string folder)
        {
            if (Parameters.DataType == DataTypes.daily || Parameters.DataType == DataTypes.hour)
            {
                //one file for a symbol
                string fileName = Parameters.StockSymbol.ToString();
                string csvFileName = string.Format("{0}.csv", fileName);
                string zipFileNmae = string.Format("{0}.zip", fileName);
                zipFileNmae = Path.Combine(folder, zipFileNmae);
                Compression.ZipData()
            }
            else
            {
                //one file for a day
            }
        }
    }
}
