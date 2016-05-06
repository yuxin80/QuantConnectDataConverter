using CoilSimulater.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public class ConverterParameters : ObjectBase
    {
        [Parameter]
        [IsFileName]
        [ParameterName("Source file")]
        [ParameterTip("File contains stock data")]
        public string SourceFileName
        {
            get { return GetValue<string>(() => this.SourceFileName); }
            set { SetValue<string>(() => this.SourceFileName, value); }
        }

        [Parameter]
        [IsFolderName]
        [ParameterName("Destination folder")]
        [ParameterTip("The destination folder name where the data will be converted to.")]
        public string DestFolderName
        {
            get { return GetValue<string>(() => this.DestFolderName); }
            set { SetValue<string>(() => this.DestFolderName, value); }
        }

        [Parameter]
        [ParameterName("Security symbol")]
        [ParameterTip("Security symbol. For example \"ibm\" for IBM stock")]
        public string StockSymbol
        {
            get { return GetValue<string>(() => this.StockSymbol); }
            set { SetValue<string>(() => this.StockSymbol, value); }
        }

        [Parameter]
        [ParameterName("Market symbol")]
        [ParameterTip("Where the security is on market, eg. usa")]
        public Market StockMarket
        {
            get { return GetValue<Market>(() => this.StockMarket); }
            set { SetValue<Market>(() => this.StockMarket, value); }
        }

        [Parameter]
        [ParameterName("Type of the stock")]
        [ParameterTip("The type of the stock, eg equity or forex")]
        public StockTypes StockType
        {
            get { return GetValue<StockTypes>(() => this.StockType); }
            set { SetValue<StockTypes>(() => this.StockType, value); }
        }

        [Parameter]
        [ParameterName("Data frequency")]
        [ParameterTip("The frequency of the data, eg tick, second")]
        public DataTypes DataType
        {
            get { return GetValue<DataTypes>(() => this.DataType); }
            set { SetValue<DataTypes>(() => this.DataType, value); }
        }
    }
}
