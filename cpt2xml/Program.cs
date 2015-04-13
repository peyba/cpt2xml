using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Reflection;
using cpt2xml.Properties;

namespace cpt2xml
{
    partial class Program
    {
        static void Main(string[] args)
        {
            /* one-exe magic */
            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;

            /* parce cmd line */
            var config = new Config(args);

            /* help */
            if (config.NeedShowHelp)
            {
                config.ShowHelp();
                return;
            }

            if (!IsConfigCool(config)) { return; }

            /* read cpt-file*/
            var cptText = File.ReadAllText(config.InFile);

            XElement xmlTable = CreateXElemntByCptString(config, cptText);

            /* save */
            if (!File.Exists(config.OutFile))
            {
                XElement rootElement = new XElement(XML_ROOT_ELEMENT_NAME);
                rootElement.Save(config.OutFile);
            }
            
            XDocument xmlDataFile = XDocument.Load(config.OutFile);
            xmlDataFile.Root.Add(xmlTable);
            xmlDataFile.Save(config.OutFile);
        }

        /// <summary>
        /// Create xml table from cpt-format file
        /// </summary>
        /// <param name="config">Config object</param>
        /// <param name="cptText">Cpt-format string</param>
        /// <returns>Xml table</returns>
        private static XElement CreateXElemntByCptString(Config config, string cptText)
        {
            string allCptEntityDataPatern = @"^{0}\n(?<{1}>((.+)\n)+)";
            string rowDataPatern = @"^(?<{0}>.+)$";

            /* parce file data by paterns */
            Regex dataTemplate = new Regex(
                String.Format(allCptEntityDataPatern, config.EntityName, DATA_TOKEN),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            var data = dataTemplate.Match(cptText).Groups[DATA_TOKEN].Value;

            Regex rowTemplate = new Regex(
                String.Format(rowDataPatern, ROW_TOKEN),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            List<string> rowsCollection = new List<string>();
            foreach (var item in rowTemplate.Matches(data))
            {
                rowsCollection.Add((item as Match).Groups[ROW_TOKEN].Value);
            }

            List<string[]> separatedRowCollection = new List<string[]>();
            rowsCollection.ForEach((string row) =>
                {
                    separatedRowCollection.Add(row.Split(CPT_FIELD_SEPARATOR));

                }
            );

            /* create xml */
            XElement xmlTable = new XElement(config.EntityName);
            foreach (var separatedRow in separatedRowCollection)
            {
                var row = new XElement(ENTITY_ROW_NODE_NAME);
                for (int i = 0; i < config.ColumnNames.Count(); i++)
                {
                    row.Add(new XElement(config.ColumnNames[i], separatedRow[i]));
                }
                xmlTable.Add(row);
            }
            return xmlTable;
        }

        /// <summary>
        /// Checking config on correctness
        /// </summary>
        /// <param name="config">Config object</param>
        /// <returns>true - if config is correct all rules and false - if uncorrect one</returns>
        private static bool IsConfigCool(Config config)
        {
            /* create message printer object */
            IOutputMessageWrap Message = new CmdMessage();

            /* test input cmd-line data */
            if (!File.Exists(config.InFile))
            {
                Message.Show(String.Format("error: cudn't find cpt-file \"{0}\"", config.InFile));
                Message.Show("print -h for help");
                return false;
            }
            if (String.IsNullOrEmpty(config.OutFile))
            {
                Message.Show("error: you need set output file");
                Message.Show("print -h for help");
                return false;
            }
            if (String.IsNullOrEmpty(config.EntityName))
            {
                Message.Show("error: you need set entity name");
                Message.Show("print -h for help");
                return false;
            }
            if (config.ColumnNames.Count() == 0)
            {
                Message.Show("error: you need set columns");
                Message.Show("print -h for help");
                return false;
            }

            return true;
        }

        /// <summary>
        /// ResolveEventHandler. Load NDesk.Options.dll from Resources
        /// </summary>
        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("NDesk.Options"))
            {
                using (var resource = new MemoryStream(Resources.NDesk_Options))
                {
                    using (var reader = new BinaryReader(resource))
                    {
                        int dllReadBufferSize = 1024;

                        List<byte> libData = new List<byte>();
                        byte[] buffer = new byte[dllReadBufferSize];

                        while (reader.Read(buffer, 0, dllReadBufferSize) > 0)
                        {
                            libData.AddRange(buffer);
                        }

                        return Assembly.Load(libData.ToArray());
                    }
                }
            }

            return null;
        }
    }
}