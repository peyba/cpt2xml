using System;
using System.Collections.Generic;
using NDesk.Options;

namespace cpt2xml
{
    class Config
    {
        #region Fields

        private string inFile = String.Empty;
        private string outFile = String.Empty;
        private string entityName = String.Empty;
        private bool showHelp = false;
        private string[] columnsArray = new string[0];
        private OptionSet configOptionsSet;
	    #endregion // Fields

        #region Properties

        /// <summary>
        /// Get input cpt-file path
        /// </summary>
        public string InFile
        {
            get { return this.inFile; }
        }

        /// <summary>
        /// Get output xml-file path
        /// </summary>
        public string OutFile
        {
            get { return this.outFile; }
        }

        /// <summary>
        /// Get name of converting data block from cpt-file
        /// </summary>
        public string EntityName
        {
            get { return this.entityName; }
        }

        /// <summary>
        /// Get true if user want see help
        /// </summary>
        public bool NeedShowHelp
        {
            get { return this.showHelp; }
        }

        /// <summary>
        /// Get an array of xml-table column(tag) names
        /// </summary>
        public string[] ColumnNames
        {
            get { return this.columnsArray; }
        }
        #endregion // Properties

        #region Constructor

        /// <summary>
        /// Create a new instance of class
        /// </summary>
        /// <param name="args"></param>
        public Config(string[] args)
        {
            #region Options Set

            this.configOptionsSet = new OptionSet()
            {
   	            { 
                    "i|in=",
                    "input cpt-file",
                    v => this.inFile = v 
                },
   	            {
                    "o|out=",
                    "output xml-file",
                    v => this.outFile = v
                },
                {
                    "e|entityname=",
                    "entity name",
                    v => this.entityName = v
                },
   	            {
                    "c|columns=",
                    "column names, separated by comma (,)",
                    v => this.columnsArray = ParceColumnsLine(v)
                },
                {
                    "h|?|help",
                    "show this message and exit", 
                    v => showHelp = v != null 
                }
            };
            #endregion // Options Set

            List<string> options = this.configOptionsSet.Parse(args);
        }
        #endregion // Constructor

        #region Methods

        /// <summary>
        /// Print help message at the console
        /// </summary>
        public void ShowHelp()
        {
            this.configOptionsSet.WriteOptionDescriptions(Console.Out);
        }

        private string[] ParceColumnsLine(string columnsLine)
        {
            return columnsLine.Split(',');
        }
        #endregion // Methods
    }
}
