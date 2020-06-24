using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PScnFin.Models;
using PScnFin;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace PScnFin
{
    /// <summary>
    /// Interaction logic for ListSettings.xaml
    /// </summary>
    public partial class ListSettings : Window
    {
        List<ListsModel> lm = new List<ListsModel>();
        List<ListsModel> listwhole = new List<ListsModel>();
        List<ProcessesModel> PM = new List<ProcessesModel>();
        public ListSettings()
        {
            
            InitializeComponent();
            lm = SqliteDataAccess.LoadListname();
            LoadProcsList();
            foreach (ListsModel x in lm)
            {
                listname.Items.Add(x.list_name);
            }
        }

        private void templist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        

        private void add_Click(object sender, RoutedEventArgs e)
        {
            string ipbeg = beg.Text;
            string ipend = end.Text;
            string[] resultbeg;
            string[] resultend;
            //int dotcount = 0;
            int p3diff;
            int p4diff;
            string tempresult;
            templist.Items.Clear();

            resultbeg = ipbeg.Split('.');
            resultend = ipend.Split('.');

            if (ipbeg.Length > 6 && ipend.Length > 6)
            {
                int p1diff = int.Parse(resultend[0]) - int.Parse(resultbeg[0]);
                int p2diff = int.Parse(resultend[1]) - int.Parse(resultbeg[1]);
                if (p2diff == 0)
                    p3diff = int.Parse(resultend[2]) - int.Parse(resultbeg[2]);
                else
                {
                    p3diff = (255 - int.Parse(resultbeg[2])) + int.Parse(resultend[2]) + (255 * (p2diff - 1));
                }
                if (p3diff == 0)
                    p4diff = int.Parse(resultend[3]) - int.Parse(resultbeg[3]);
                else
                {
                    p4diff = (255 - int.Parse(resultbeg[3])) + int.Parse(resultend[3]) + (255 * (p3diff - 1));
                }
                int x = 0;
                int y = 0;
                int z = 0;
                for (int i = 0; i < p4diff + 1; i++)
                {

                    if ((int.Parse(resultbeg[3]) + x) == 255 && y == 0)
                    {
                        x = 0;
                        y++;
                    }
                    if (x == 255)
                    {
                        x = 0;
                        y++;
                    }
                    if (y == 255)
                    {
                        y = 0;
                        z++;
                    }

                    if (y == 0)
                    {
                        tempresult = resultbeg[0] + '.' + (int.Parse
                            (resultbeg[1]) + z).ToString() + '.' + (int.Parse
                            (resultbeg[2]) + y).ToString() + '.' + (int.Parse
                            (resultbeg[3]) + x).ToString();
                        templist.Items.Add(tempresult);
                        x++;
                    }
                    else
                    {
                        tempresult = resultbeg[0] + '.' + (int.Parse
                                                (resultbeg[1]) + z).ToString() + '.' + (int.Parse
                                                (resultbeg[2]) + y).ToString() + '.' + x;
                        templist.Items.Add(tempresult);
                        x++;
                    }


                }
            }
            if (singleadd.Text.Contains("kd")||singleadd.Text.Contains("KD")||singleadd.Text.Contains("Kd")|| singleadd.Text.Contains("kD"))
            {
                List <UsersModel> um = new List<UsersModel>();
                um = SqliteDataAccess.LoadUsers();
                foreach (UsersModel zz in um)
                {
                    if (zz.pc_name == singleadd.Text)
                    {
                        templist.Items.Add(zz.ip);
                        break;
                    }

                }
            }
            else if (singleadd.Text != "")
                templist.Items.Add(singleadd.Text);
            beg.Text = "";
            end.Text = "";
            //singleadd.Text = "";
        }

        private void LoadProcsList()
        {
            PM = SqliteDataAccess.LoadProcs();
            foreach (ProcessesModel o in PM)
            {
                proc1.Items.Add(o.process_name);
                proc2.Items.Add(o.process_name);
                proc3.Items.Add(o.process_name);
                proc4.Items.Add(o.process_name);
                proc5.Items.Add(o.process_name);
            }
        }

        private void addtolist_Click(object sender, RoutedEventArgs e)
        {
            //templist.SelectedItems
            if (listname.Text == "")
                MessageBox.Show("Wybierz nazwe listy adresów");
            else if (templist.SelectedItems.Count == 0)
            {
                MessageBox.Show("Wybierz adresy do dodania");
            }
            else
            {
                int counterwholelist;
                int onetime = 0;
                foreach (var l in templist.SelectedItems)
                {
                    counterwholelist = 0;
                    /*
                    foreach (var xx in listwhole)
                    {
                        if (xx.pc_name == l.ToString())
                        {
                            counterwholelist++;
                        }
                        
                    }*/
                    foreach (var xx in wholelist.Items)
                    {
                        if (xx.ToString() == l.ToString())
                        {
                            counterwholelist++;
                        }

                    }

                    if (counterwholelist == 0)
                    {
                        wholelist.Items.Add(l);
                        //SqliteDataAccess.AddList(l.ToString(), listname.Text);////////////////////////
                        onetime++;
                    }
                }
                
            }
            
            if (lm.Exists(x => x.list_name == listname.Text) == false)
            {
                listname.Items.Add(listname.Text);
            }
            listwhole = SqliteDataAccess.LoadList(listname.Text);
            //wholelist.Items.Clear();
            /*
            foreach (ListsModel x in listwhole)
            {
                if (wholelist.Items.Contains
                {
                    listname.Items.Add(listname.Text);
                }
                wholelist.Items.Add(x.pc_name);
            }*/
        }

        private void listname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listwhole = SqliteDataAccess.LoadListPCname(listname.Text);//<--
            wholelist.Items.Clear();
            foreach (ListsModel x in listwhole)
            {
                wholelist.Items.Add(x.pc_name);
            }
            lmod = SqliteDataAccess.LoadList(listname.Text.ToString());
            try
            {
                proc1.Text = lmod[lmod.Count - 1].proc1;
                proc2.Text = lmod[lmod.Count - 1].proc2;
                proc3.Text = lmod[lmod.Count - 1].proc3;
                proc4.Text = lmod[lmod.Count - 1].proc4;
                proc5.Text = lmod[lmod.Count - 1].proc5;
            }
            catch (Exception eee)
            {
            }
            //MessageBox.Show("xx"); 
        }
        private void listname_TextChanged(object sender, TextChangedEventArgs e)
        {
            listwhole = SqliteDataAccess.LoadListPCname(listname.Text);//<---
            lmod = SqliteDataAccess.LoadList(listname.Text.ToString());
            try
            {
                proc1.Text = lmod[lmod.Count - 1].proc1;
                proc2.Text = lmod[lmod.Count - 1].proc2;
                proc3.Text = lmod[lmod.Count - 1].proc3;
                proc4.Text = lmod[lmod.Count - 1].proc4;
                proc5.Text = lmod[lmod.Count - 1].proc5;
            }
            catch (Exception eee)
            {
                MessageBox.Show(eee.ToString());
            }
            
            wholelist.Items.Clear();
            foreach (ListsModel x in listwhole)
            {
                wholelist.Items.Add(x.pc_name);
            }
        }

        private void singleadd_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ls = new MainWindow();
            //ls.Owner = this;
            ls.Show();
            this.Close();
        }

        private void proc1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void savelist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proc1.Text.ToString() != "")
                {
                    SqliteDataAccess.AddProcess(proc1.Text.ToString());
                    proc1.Items.Add(proc1.Text.ToString());
                }
                
            }
            catch (Exception ee)
            {

            }
            try
            {
                if (proc1.Text.ToString() != "")
                {
                    SqliteDataAccess.AddProcess(proc2.Text.ToString());
                    proc2.Items.Add(proc2.Text.ToString());
                }
                    
            }
            catch (Exception ee)
            {

            }
            try
            {
                if (proc1.Text.ToString() != "")
                {
                    SqliteDataAccess.AddProcess(proc3.Text.ToString());
                    proc3.Items.Add(proc3.Text.ToString());
                }
            }
            catch (Exception ee)
            {

            }
            try
            {
                if (proc1.Text.ToString() != "")
                {
                    SqliteDataAccess.AddProcess(proc4.Text.ToString());
                    proc4.Items.Add(proc4.Text.ToString());
                }

            }
            catch (Exception ee)
            {

            }
            try
            {
                if (proc1.Text.ToString() != "")
                {
                    SqliteDataAccess.AddProcess(proc5.Text.ToString());
                    proc5.Items.Add(proc5.Text.ToString());
                }
            }
            catch (Exception ee)
            {

            }
            PM = SqliteDataAccess.LoadProcs();
            
            foreach (var x in wholelist.Items)
            {
                SqliteDataAccess.AddList(x.ToString(), listname.Text, proc1.Text, proc2.Text, proc3.Text, proc4.Text, proc5.Text);
            }
        }
        private int timeout = 100;
        public int nFound = 0;
        private SpinWait wait = new SpinWait();
        static object lockObj = new object();
        Stopwatch stopWatch = new Stopwatch();
        private TimeSpan ts;
        List<ListsModel> lmod = new List<ListsModel>();
        List<UsersModel> UM = new List<UsersModel>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rozpoczęto skanowanie");
            RunPingSweep_Async();
            MessageBox.Show("Zakończono skanowanie");
        }
        string ip;
        string BaseIP="10.3.";
        public async void RunPingSweep_Async()
        {
            nFound = 0;
            var tasks = new List<Task>();

            lmod = SqliteDataAccess.LoadList(listname.Text);

            stopWatch.Start();

            for (int i = 2; i < 255; i++)
                for (int j = 2; j < 255; j++)
                {
                    
                        ip = BaseIP + i.ToString() + "." + j.ToString();
                        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();

                        var task = PingAndUpdateAsync(p, ip);
                        tasks.Add(task);
                        if (j == 255) wait.SpinOnce();
                        p.Dispose();
                    
                }
            await Task.WhenAll(tasks).ContinueWith(t =>
            {
                stopWatch.Stop();
                ts = stopWatch.Elapsed;
            });
        }
        private async Task PingAndUpdateAsync(System.Net.NetworkInformation.Ping ping, string ip)
        {

            var reply = await ping.SendPingAsync(ip, timeout);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                //Console.WriteLine(reply.Address+" "+ GetMachineNameFromIPAddress(reply.Address.ToString()));
                if (GetMachineNameFromIPAddress(reply.Address.ToString()).Length > 1)
                {
                    UsersModel x = new UsersModel();
                    x.pc_name = GetMachineNameFromIPAddress(reply.Address.ToString());
                    x.ip = reply.Address.ToString();

                    UM.Add(x);
                    Console.WriteLine(x.ip + " " + x.pc_name);
                    try
                    {
                        SqliteDataAccess.AddUser(x.pc_name, x.ip);
                    }
                    catch (System.Data.SQLite.SQLiteException)
                    {
                        SqliteDataAccess.UpdateUser(x.pc_name, x.ip);
                    }
                }

                lock (lockObj)
                {
                    nFound++;
                    ping.Dispose();
                }
                ping.Dispose();
            }
            if (reply.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                ping.Dispose();
            }


        }

        private static string GetMachineNameFromIPAddress(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
            }
            return machineName;
        }
        bool ipadress = true;
        private void ipandname_Click(object sender, RoutedEventArgs e)
        {
            List<UsersModel> temp = new List<UsersModel>();
            List<UsersModel> temp2 = new List<UsersModel>();
            UsersModel umtemp = new UsersModel();

            if (ipadress == true)
            {
                
                foreach (var x in templist.Items)
                {
                    temp2 = SqliteDataAccess.LoadUserName(x.ToString());
                    if (temp2.Count != 0)
                        temp.Add(temp2.First());
                    else
                    {
                        umtemp = new UsersModel();
                        umtemp.pc_name = x.ToString();
                        temp.Add(umtemp);
                    }
                    
                }
                templist.Items.Clear();
                foreach(UsersModel x in temp)
                {
                    templist.Items.Add(x.pc_name);
                }
                temp = new List<UsersModel>();
                foreach (var x in wholelist.Items)
                {
                    temp2 = SqliteDataAccess.LoadUserName(x.ToString());
                    if (temp2.Count != 0)
                        temp.Add(temp2.First());
                    else
                    {
                        umtemp = new UsersModel();
                        umtemp.pc_name = x.ToString();
                        temp.Add(umtemp);
                    }
                }
                wholelist.Items.Clear();
                foreach (UsersModel x in temp)
                {
                    wholelist.Items.Add(x.pc_name);
                }
                ipadress = false;
            }
            else
            {
                foreach (var x in templist.Items)
                {
                    temp2 = SqliteDataAccess.LoadUserIp(x.ToString());
                    if (temp2.Count != 0)
                        temp.Add(temp2.First());
                    else
                    {
                        umtemp = new UsersModel();
                        umtemp.ip = x.ToString();
                        temp.Add(umtemp);
                    }
                }
                templist.Items.Clear();
                foreach (UsersModel x in temp)
                {
                    templist.Items.Add(x.ip);
                }
                temp = new List<UsersModel>();
                foreach (var x in wholelist.Items)
                {
                    temp2 = SqliteDataAccess.LoadUserIp(x.ToString());
                    if (temp2.Count != 0)
                        temp.Add(temp2.First());
                    else
                    {
                        umtemp = new UsersModel();
                        umtemp.ip = x.ToString();
                        temp.Add(umtemp);
                    }
                }
                wholelist.Items.Clear();
                foreach (UsersModel x in temp)
                {
                    wholelist.Items.Add(x.ip);
                }
                ipadress = true;
            }
            

            
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var x in wholelist.SelectedItems)
            {
                if (x.ToString().Contains("kd") == true || x.ToString().Contains("KD") == true || x.ToString().Contains("Kd") == true)
                {
                    UM=SqliteDataAccess.LoadUserIp(x.ToString());
                    SqliteDataAccess.DeletefromList(UM[0].ip);
                    UM = new List<UsersModel>();
                }   
                else SqliteDataAccess.DeletefromList(x.ToString());
            }
            
            wholelist.Items.Remove(wholelist.SelectedItem);

        }
    }
}
