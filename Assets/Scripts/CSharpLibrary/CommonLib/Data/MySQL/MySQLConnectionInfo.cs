using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace Shiftup.CommonLib.Data.MySQL
{
    public class MySQLConnectionInfo : ConnectionInfo
    {
        public readonly string Address;
        public readonly string UserId;
        public readonly string Password;
        public readonly string DBName;
        private string charSet;

        public MySQLConnectionInfo(string a, string u, string p, string d, string cs)
        {
            this.Address = a;
            this.UserId = u;
            this.Password = p;
            this.DBName = d;
            this.charSet = cs;
        }

        public MySQLConnectionInfo Clone(string newDatabase)
        {
            return new MySQLConnectionInfo(Address, UserId, Password, newDatabase, charSet);
        }
        static public MySQLConnectionInfo Decrypt(string msg)
        {
            string[] list = DecryptData(msg);

            switch (list.Length)
            {
                case 4:
                    return new MySQLConnectionInfo(list[0], list[1], list[2], list[3], null);
                case 5:
                    return new MySQLConnectionInfo(list[0], list[1], list[2], list[3], list[4]);

                default:
                    throw new DataException("Invalid crypt string");
            }
        }

        public string GetDumpString(string outputFilename)
        {
            return String.Format("-h {0} -u{1} -p{2} {3} --default-character-set=utf8 --result-file={4}",
                Address, UserId, Password, DBName, outputFilename);
        }
        public override string GetInfo()
        {
            return String.Format("server = {0}; uid = {1}; database = {2} {3}", Address, UserId, DBName, getCharSet());
        }

        public override string ToString()
        {
            return String.Format("server = {0}; uid = {1}; pwd = {2}; database = {3}; Allow Zero Datetime=True {4}", Address, UserId, Password, DBName, getCharSet());
        }

        private string getCharSet()
        {
            if (this.charSet == null)
                return String.Empty;

            return String.Format("; CharSet={0}", this.charSet);
        }
    }
}

