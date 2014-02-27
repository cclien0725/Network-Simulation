using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace Network_Simulation
{
    class SQLiteUtility
    {
        private string baseDirectory = Environment.CurrentDirectory + @"\Log";
        private string fileName;

        public SQLiteUtility(string fileName)
        {
            this.fileName = baseDirectory + @"\" + fileName + ".db";

            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            if (!File.Exists(this.fileName))
            {
                SQLiteConnection.CreateFile(this.fileName);


            }
        }
    }
}
