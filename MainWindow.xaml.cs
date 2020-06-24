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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PScnFin.Models;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.Windows.Threading;
namespace PScnFin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<UsersModel> UM = new List<UsersModel>();
        //List<UsersModel> UM = new List<UsersModel>();
        List<ProcessesModel> PM = new List<ProcessesModel>();
        List<ScansModel> SM = new List<ScansModel>();
        List<DataModel> DM = new List<DataModel>();
        List<UsersModel> UMmanual = new List<UsersModel>();
        int counter_scn = 0;
        int procnb = 1;
        public MainWindow()
        {
            InitializeComponent();
            RB1.IsChecked = true;
            LoadProcsList();
            lm = SqliteDataAccess.LoadListname();
            foreach (ListsModel x in lm)
            {
                listname.Items.Add(x.list_name);
            }
        }
        private void LoadProcsList()
        {
            PM = SqliteDataAccess.LoadProcs();
            foreach (ProcessesModel o in PM)
            {
                CB.Items.Add(o.process_name);
                CB2.Items.Add(o.process_name);
                CB3.Items.Add(o.process_name);
                CB4.Items.Add(o.process_name);
                CB5.Items.Add(o.process_name);
            }
            CB.Text = CB.Items.GetItemAt(0).ToString() ;
            CB2.Text = CB2.Items.GetItemAt(1).ToString();
            CB3.Text = CB3.Items.GetItemAt(2).ToString();
            CB4.Text = CB4.Items.GetItemAt(3).ToString();
            CB5.Text = CB5.Items.GetItemAt(4).ToString();
            /*
            CB.ItemsSource = null;
            CB.ItemsSource = PM;
            
            CB.DisplayMemberPath = "process_name";
            */
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            Statystyki st = new Statystyki();
            st.Owner = this;
            st.Show();
            //this.Close();
        }
        Stopwatch sw = new Stopwatch();/*
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rozpoczęto szukanie aktywnych adresów");
            try { 
            SqliteDataAccess.AddProcess(CB.Text.ToString());
            CB.Items.Add(CB.Text.ToString());
            PM = SqliteDataAccess.LoadProcs();
            }
            catch (Exception ex)
            {
            }
            // 
            UM = new List<UsersModel>();
            RunPingSweep_Async();
            

            MessageBox.Show("Zakończono szykanie adresów");
        }*/

        private int timeout = 100;
        public int nFound = 0;
        private SpinWait wait = new SpinWait();
        static object lockObj = new object();
        Stopwatch stopWatch = new Stopwatch();
        private TimeSpan ts;
        List<ListsModel> lm = new List<ListsModel>();
        List<ListsModel> lmod = new List<ListsModel>();
        public async void RunPingSweep_Async()
        {
            nFound = 0;
            var tasks = new List<Task>();

            lmod = SqliteDataAccess.LoadListPCname(listname.Text);

            stopWatch.Start();

            foreach (ListsModel x in lmod)
            {
                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();

                var task = PingAndUpdateAsync(p, x.pc_name);
                tasks.Add(task);
                wait.SpinOnce();
                p.Dispose();
            }


            /*
            for (int i = 2; i < 255; i++)
                for (int j = 2; j < 255; j++)
                {
                    if (i == 5 || i == 6)
                    {
                        ip = BaseIP + i.ToString() + "." + j.ToString();
                        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();

                        var task = PingAndUpdateAsync(p, ip);
                        tasks.Add(task);
                        if (j == 255) wait.SpinOnce();
                        p.Dispose();
                    }
                }
                */

            /*for (int i = StartIP; i <= StopIP; i++)
    {
        ip = BaseIP + i.ToString();

        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
        var task = PingAndUpdateAsync(p, ip);
        tasks.Add(task);
    }*/

            await Task.WhenAll(tasks).ContinueWith(t =>
            {
                stopWatch.Stop();
                ts = stopWatch.Elapsed;
                Console.WriteLine(nFound.ToString() + " devices found! Elapsed time: " + ts.ToString());

                //  lb.Items.Add(nFound.ToString() + " devices found! Elapsed time: " + ts.ToString());
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

        private void scan()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBox.Show("Rozpoczęto szukanie aktywnych adresów");
                try
                {
                    SqliteDataAccess.AddProcess(CB.Text.ToString());
                    CB.Items.Add(CB.Text.ToString());
                    PM = SqliteDataAccess.LoadProcs();
                }
                catch (Exception ex)
                {
                }
                // 
                UM = new List<UsersModel>();
                RunPingSweep_Async();


                //MessageBox.Show("Zakończono szykanie adresów");




                //->
                sw.Reset();

                counter_scn = 0;
                DateTime now = DateTime.Now;
                MessageBox.Show("Rozpoczęto skanowanie - " + now.ToString());
                try
                {
                    SqliteDataAccess.AddScan(TTB.Text.ToString(), now.ToString());
                    SM = SqliteDataAccess.LoadScans();
                }
                catch (Exception xxx) { MessageBox.Show(xxx.ToString()); }

                foreach (ScansModel s in SM)
                {
                    counter_scn++;
                }
                int countusrs = 0;
                foreach (UsersModel u in UM)
                {
                    countusrs += 1;
                }
                string[,] vec = new string[countusrs, 3];
                string[,] vec2 = new string[countusrs, 3];
                string[,] vec3 = new string[countusrs, 3];
                string[,] vec4 = new string[countusrs, 3];
                string[,] vec5 = new string[countusrs, 3];
                int v = 0;
                foreach (UsersModel u in UM)
                {
                    vec[v, 0] = u.pc_name;
                    vec[v, 1] = "0";
                    vec[v, 2] = "0";
                    vec2[v, 0] = u.pc_name;
                    vec2[v, 1] = "0";
                    vec2[v, 2] = "0";
                    vec3[v, 0] = u.pc_name;
                    vec3[v, 1] = "0";
                    vec3[v, 2] = "0";
                    vec4[v, 0] = u.pc_name;
                    vec4[v, 1] = "0";
                    vec4[v, 2] = "0";
                    vec5[v, 0] = u.pc_name;
                    vec5[v, 1] = "0";
                    vec5[v, 2] = "0";

                    v++;
                }



                sw.Start();
                while (sw.ElapsedMilliseconds <= (Convert.ToInt64(TTB.Text) * 60000))
                {
                    foreach (UsersModel u in UM)
                    {
                        Console.WriteLine(u.full);
                        try
                        {

                            Process[] procByName = Process.GetProcessesByName(CB.Text.ToString(), u.pc_name);
                            if (procByName.Length > 0)
                            {
                                for (int i = 0; i < v; i++)
                                    if (vec[i, 0] == u.pc_name)
                                        vec[i, 1] = (int.Parse(vec[i, 1]) + 1).ToString();
                            }
                            else
                                for (int i = 0; i < v; i++)
                                    if (vec[i, 0] == u.pc_name)
                                        vec[i, 2] = (int.Parse(vec[i, 2]) + 1).ToString();
                            if (procnb > 1)
                            {
                                procByName = Process.GetProcessesByName(CB2.Text.ToString(), u.pc_name);
                                if (procByName.Length > 0)
                                {
                                    for (int i = 0; i < v; i++)
                                        if (vec2[i, 0] == u.pc_name)
                                            vec2[i, 1] = (int.Parse(vec2[i, 1]) + 1).ToString();
                                }
                                else
                                    for (int i = 0; i < v; i++)
                                        if (vec2[i, 0] == u.pc_name)
                                            vec2[i, 2] = (int.Parse(vec2[i, 2]) + 1).ToString();
                            }
                            if (procnb > 2)
                            {
                                procByName = Process.GetProcessesByName(CB3.Text.ToString(), u.pc_name);
                                if (procByName.Length > 0)
                                {
                                    for (int i = 0; i < v; i++)
                                        if (vec3[i, 0] == u.pc_name)
                                            vec3[i, 1] = (int.Parse(vec3[i, 1]) + 1).ToString();
                                }
                                else
                                    for (int i = 0; i < v; i++)
                                        if (vec3[i, 0] == u.pc_name)
                                            vec3[i, 2] = (int.Parse(vec3[i, 2]) + 1).ToString();
                            }
                            if (procnb > 3)
                            {
                                procByName = Process.GetProcessesByName(CB4.Text.ToString(), u.pc_name);
                                if (procByName.Length > 0)
                                {
                                    for (int i = 0; i < v; i++)
                                        if (vec4[i, 0] == u.pc_name)
                                            vec4[i, 1] = (int.Parse(vec4[i, 1]) + 1).ToString();
                                }
                                else
                                    for (int i = 0; i < v; i++)
                                        if (vec4[i, 0] == u.pc_name)
                                            vec4[i, 2] = (int.Parse(vec4[i, 2]) + 1).ToString();
                            }
                            if (procnb > 4)
                            {
                                procByName = Process.GetProcessesByName(CB5.Text.ToString(), u.pc_name);
                                if (procByName.Length > 0)
                                {
                                    for (int i = 0; i < v; i++)
                                        if (vec5[i, 0] == u.pc_name)
                                            vec5[i, 1] = (int.Parse(vec5[i, 1]) + 1).ToString();
                                }
                                else
                                    for (int i = 0; i < v; i++)
                                        if (vec5[i, 0] == u.pc_name)
                                            vec5[i, 2] = (int.Parse(vec5[i, 2]) + 1).ToString();
                            }



                        }
                        catch (Exception z)
                        {
                            //MessageBox.Show(z.ToString());
                        }


                    }
                }

                foreach (UsersModel u in UM)
                {
                    int p = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0;
                    int n = 0, n2 = 0, n3 = 0, n4 = 0, n5 = 0;
                    for (int i = 0; i < v; i++)
                    {
                        if (vec[i, 0] == u.pc_name)
                        {
                            p = int.Parse(vec[i, 1]);
                            n = int.Parse(vec[i, 2]);
                        }
                        if (vec2[i, 0] == u.pc_name)
                        {
                            p2 = int.Parse(vec2[i, 1]);
                            n2 = int.Parse(vec2[i, 2]);
                        }
                        if (vec3[i, 0] == u.pc_name)
                        {
                            p3 = int.Parse(vec3[i, 1]);
                            n3 = int.Parse(vec3[i, 2]);
                        }
                        if (vec4[i, 0] == u.pc_name)
                        {
                            p4 = int.Parse(vec4[i, 1]);
                            n4 = int.Parse(vec4[i, 2]);
                        }
                        if (vec5[i, 0] == u.pc_name)
                        {
                            p5 = int.Parse(vec5[i, 1]);
                            n5 = int.Parse(vec5[i, 2]);
                        }
                    }

                    SqliteDataAccess.AddData(p, n, u.pc_name, CB.Text.ToString(), counter_scn);
                    if (procnb > 1)
                        SqliteDataAccess.AddData(p2, n2, u.pc_name, CB2.Text.ToString(), counter_scn);
                    if (procnb > 2)
                        SqliteDataAccess.AddData(p3, n3, u.pc_name, CB3.Text.ToString(), counter_scn);
                    if (procnb > 3)
                        SqliteDataAccess.AddData(p4, n4, u.pc_name, CB4.Text.ToString(), counter_scn);
                    if (procnb > 4)
                        SqliteDataAccess.AddData(p5, n5, u.pc_name, CB5.Text.ToString(), counter_scn);
                }



                /*
                foreach (UsersModel u in UM)
                {
                    LB.Items.Add(u.full);
                    //umtemp = u;
                    //SqliteDataAccess.AddUser(u.pc_name, u.ip);

                }
                /*
                try
                {

                    Process[] remoteByName = Process.GetProcessesByName("notepad.exe", UM[counter].ip);
                    MessageBox.Show(remoteByName[0].ToString());
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                    counter++;
                }*/

                MessageBox.Show("Skanowanie zakończone");
            }));
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(scan);
            thread.Start();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //BaseIP = IPTB.Text.ToString();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CB2.Visibility = Visibility.Visible;
            CB3.Visibility = Visibility.Hidden;
            CB4.Visibility = Visibility.Hidden;
            CB5.Visibility = Visibility.Hidden;
            procnb = 2;
            LabProc.Content = "Nazwy procesów:";
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            procnb = 1;
            CB2.Visibility = Visibility.Hidden;
            CB3.Visibility = Visibility.Hidden;
            CB4.Visibility = Visibility.Hidden;
            CB5.Visibility = Visibility.Hidden;
            LabProc.Content = "Nazwa procesu:";
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            CB2.Visibility = Visibility.Visible;
            CB3.Visibility = Visibility.Visible;
            CB4.Visibility = Visibility.Hidden;
            CB5.Visibility = Visibility.Hidden;
            procnb = 3;
            LabProc.Content = "Nazwy procesów:";
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            CB2.Visibility = Visibility.Visible;
            CB3.Visibility = Visibility.Visible;
            CB4.Visibility = Visibility.Visible;
            CB5.Visibility = Visibility.Hidden;
            procnb = 4;
            LabProc.Content = "Nazwy procesów:";
        }

        private void RadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            CB2.Visibility = Visibility.Visible;
            CB3.Visibility = Visibility.Visible;
            CB4.Visibility = Visibility.Visible;
            CB5.Visibility = Visibility.Visible;
            procnb = 5;
            LabProc.Content = "Nazwy procesów:";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ListSettings ls = new ListSettings();
            //ls.Owner = this;
            ls.Show();
            this.Close();
            
        }

        private void listname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lmod = SqliteDataAccess.LoadList(listname.SelectedItem.ToString());
            if (lmod[lmod.Count-1].proc5 != "")
            {
                CB2.Visibility = Visibility.Visible;
                CB3.Visibility = Visibility.Visible;
                CB4.Visibility = Visibility.Visible;
                CB5.Visibility = Visibility.Visible;
                procnb = 5;
                LabProc.Content = "Nazwy procesów:";
                CB.Text = lmod[lmod.Count - 1].proc1;
                CB2.Text = lmod[lmod.Count - 1].proc2;
                CB3.Text = lmod[lmod.Count - 1].proc3;
                CB4.Text = lmod[lmod.Count - 1].proc4;
                CB5.Text = lmod[lmod.Count - 1].proc5;
                RB5.IsChecked = true;
            }
            else if (lmod[lmod.Count - 1].proc4 != "")
            {
                CB2.Visibility = Visibility.Visible;
                CB3.Visibility = Visibility.Visible;
                CB4.Visibility = Visibility.Visible;
                CB5.Visibility = Visibility.Hidden;
                procnb = 4;
                LabProc.Content = "Nazwy procesów:";
                CB.Text = lmod[lmod.Count - 1].proc1;
                CB2.Text = lmod[lmod.Count - 1].proc2;
                CB3.Text = lmod[lmod.Count - 1].proc3;
                CB4.Text = lmod[lmod.Count - 1].proc4;
                RB4.IsChecked = true;
            }
            else if (lmod[lmod.Count - 1].proc3 != "")
            {
                CB2.Visibility = Visibility.Visible;
                CB3.Visibility = Visibility.Visible;
                CB4.Visibility = Visibility.Hidden;
                CB5.Visibility = Visibility.Hidden;
                LabProc.Content = "Nazwy procesów:";
                procnb = 3;
                CB.Text = lmod[lmod.Count - 1].proc1;
                CB2.Text = lmod[lmod.Count - 1].proc2;
                CB3.Text = lmod[lmod.Count - 1].proc3;
                RB3.IsChecked = true;
            }
            else if (lmod[lmod.Count - 1].proc2 != "")
            {
                CB2.Visibility = Visibility.Visible;
                CB3.Visibility = Visibility.Hidden;
                CB4.Visibility = Visibility.Hidden;
                CB5.Visibility = Visibility.Hidden;
                procnb = 2;
                LabProc.Content = "Nazwy procesów:";
                CB.Text = lmod[lmod.Count - 1].proc1;
                CB2.Text = lmod[lmod.Count - 1].proc2;
                RB2.IsChecked = true;
            }
            else
            {
                CB2.Visibility = Visibility.Hidden;
                CB3.Visibility = Visibility.Hidden;
                CB4.Visibility = Visibility.Hidden;
                CB5.Visibility = Visibility.Hidden;
                procnb = 1;
                LabProc.Content = "Nazwa procesu:";
                CB.Text = lmod[lmod.Count - 1].proc1;
                RB1.IsChecked = true;
            }



        }

        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
