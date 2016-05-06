using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace CoilSimulater.Utilities.AsciiParser
{
    public class FileParser : ObjectBase
    {
        [DefaultValue(",")]
        [Parameter]
        public string Deliminator
        {
            get { return GetValue<string>(() => this.Deliminator); }
            set { SetValue<string>(() => this.Deliminator, value); }
        }

        [DefaultValue(1)]
        [Parameter]
        public int NumberOfHeaderLines
        {
            get { return GetValue<int>(() => this.NumberOfHeaderLines); }
            set { SetValue<int>(() => this.NumberOfHeaderLines, value); }
        }

        [DefaultValue(1)]
        [Parameter]
        public int HeaderLineIndex
        {
            get { return GetValue<int>(() => this.HeaderLineIndex); }
            set { SetValue<int>(() => this.HeaderLineIndex, value); }
        }

        [Parameter]
        [IsFileName]
        public string FileName
        {
            get { return GetValue<string>(() => this.FileName); }
            set { SetValue<string>(() => this.FileName, value); }
        }

        public DataSet ParseFile()
        {
            try
            {
                if (string.IsNullOrEmpty(FileName))
                    return null;
                using (var sr = File.OpenText(FileName))
                {
                    var result = new DataSet("DefaultSet");
                    var table = result.Tables.Add("DefaultTable");
                    var line = string.Empty;
                    for (int i = 0; i < HeaderLineIndex; i++)
                        line = sr.ReadLine();
                    var columns = line.Split(Deliminator.ToCharArray());
                    foreach (var n in columns)
                    {
                        if (string.IsNullOrEmpty(n))
                            continue;

                        table.Columns.Add(n);
                    }

                    for (int i = HeaderLineIndex; i <= NumberOfHeaderLines; i++)
                        line = sr.ReadLine();

                    while (line != null)
                    {
                        var values = line.Split(Deliminator.ToCharArray());

                        table.Rows.Add(values);
                        line = sr.ReadLine();
                    }

                    return result;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Number of the columns of the data is not equal to the number of the headers", "Error occured");
                return null;
            }
        }

        public IEnumerable<String> GetSamples()
        {
            if (string.IsNullOrEmpty(FileName))
                return null;

            if (!File.Exists(FileName))
                return null;

            return File.ReadLines(FileName).Take(50);
        }
    }
}
