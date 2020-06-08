using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PScnFin.Models
{
    public class SqliteDataAccess
    {
        public static List<UsersModel> LoadUsers()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UsersModel>("select * from Users", new DynamicParameters());
                return output.ToList();
            }
        }
        public static void AddUser(string nm,string ip)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into users (pc_name, ip) values('" + nm + "', '"+ip+"');");
            }
        }

        public static void UpdateUser(string nm, string ip)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update users set ip='{ip}' where pc_name='{nm}';");
            }
        }

        public static List<ProcessesModel> LoadProcs()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ProcessesModel>("select * from Processes", new DynamicParameters());
                return output.ToList();
            }
        }
        public static void AddProcess(string p)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into processes (process_name) values('" + p + "');");
            }
        }

        public static List<DataModel> LoadData()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DataModel>("select * from Data", new DynamicParameters());
                return output.ToList();
            }
        }
        public static void AddData(int p,int n,string pc, string proc,int scn)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"insert into data (positive_scan,negative_scan,pc_name, scan_id, process_name) values('{p}','{n}','{pc}','{scn}','{proc}');");
            }
        }
        public static void AddScan(string time, string date)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"insert into scans (time, date) values('{time}', '{date}');");
            }
        }
        public static List<DataModel> LoadDataTime(String proc)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DataModel>("select * from Data where process_name='" +proc+"';", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<ScansModel> LoadScans()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ScansModel>("select * from Scans", new DynamicParameters());
                return output.ToList();
            }
        }
        private static string LoadConnectionString(string id = "Default") {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
