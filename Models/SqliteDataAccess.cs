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
        public static void AddUser(string nm, string ip)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into users (pc_name, ip) values('" + nm + "', '" + ip + "');");
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
                var output = cnn.Query<ProcessesModel>("select * from Processes;", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<UsersModel> LoadUserName(string ip)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UsersModel>($"select * from Users where ip='{ip}';", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<UsersModel> LoadUserIp(string nm)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UsersModel>($"select * from Users where pc_name='{nm}';", new DynamicParameters());
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
        public static void AddData(int p, int n, string pc, string proc, int scn)
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
                var output = cnn.Query<DataModel>("select * from Data where process_name='" + proc + "';", new DynamicParameters());
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

        public static List<ListsModel> LoadList(string name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ListsModel>($"select * from Lists where list_name = '{name}';", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<ListsModel> LoadListname()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ListsModel>("select distinct list_name from Lists;", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<ListsModel> LoadListPc(string listname)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ListsModel>($"select distinct pc_name from Lists where list_name='';", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<ListsModel> LoadListPCname(string listname)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ListsModel>($"select distinct pc_name from Lists where list_name='{listname}';", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void AddList(string pcname, string listname, string proc1="", string proc2 = "", string proc3 = "", string proc4 = "", string proc5 = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"insert into lists (pc_name, list_name, proc1, proc2, proc3, proc4, proc5) values('{pcname}', '{listname}', '{proc1}', '{proc2}', '{proc3}', '{proc4}', '{proc5}');");
            }
        }

        public static void DeletefromList(string pcname="", string ip="")
        {
            if (pcname == "")
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute($"delete from Lists where ip='{ip}';");
                }
            }
            else
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute($"delete from Lists where pc_name='{pcname}';");
                }
            }

        }


        private static string LoadConnectionString(string id = "Default") {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
