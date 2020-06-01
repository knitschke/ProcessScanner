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
using System.Diagnostics;
using System.ComponentModel;

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
        int counter_scn = 0;
        public MainWindow()
        {
            InitializeComponent();
            LoadProcsList();
        }
        private void LoadProcsList()
        {
            PM = SqliteDataAccess.LoadProcs();
            foreach (ProcessesModel o in PM)
            {
                CB.Items.Add(o.process_name);
            }
            
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
        Stopwatch sw = new Stopwatch();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*
            //sw.Start();
           
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}",
            ts.Hours, ts.Minutes);
            sw.Reset();
            MessageBox.Show("Skanowanie trwało(h:min): "+elapsedTime);
            */

            //var watch = System.Diagnostics.Stopwatch.;

            //b1.Content = "Zakończ skanowanie";
            //b1.Background = Brushes.Turquoise;
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
        }
        private string BaseIP = "10.3.";//"10.3.";
        private string ip;

        private int timeout = 100;
        public int nFound = 0;
        private SpinWait wait = new SpinWait();
        static object lockObj = new object();
        Stopwatch stopWatch = new Stopwatch();
        private TimeSpan ts;

        public async void RunPingSweep_Async()
        {
            nFound = 0;
            var tasks = new List<Task>();
            //LB.Items.Add("dziala??");

            stopWatch.Start();
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
                Console.WriteLine("dziala " + nFound.ToString() + " devices found! Elapsed time: " + ts.ToString());

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
                    SqliteDataAccess.AddUser(x.pc_name, x.ip);
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
        int counter = 0;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            sw.Reset();
            MessageBox.Show("Rozpoczęto skanowanie");
            counter_scn = 0;
            SqliteDataAccess.AddScan(TTB.Text.ToString());
            SM = SqliteDataAccess.LoadScans();
            foreach(ScansModel s in SM)
            {
                counter_scn++;
            }
            int countusrs = 0;
            foreach (UsersModel u in UM)
            {
                countusrs += 1;
            }
            string[,] vec = new string[countusrs, 3 ];
            int v = 0;
            foreach (UsersModel u in UM)//outofarray
            {
                vec[v, 0] = u.pc_name;
                vec[v, 1] = "0";
                vec[v, 2] = "0";
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

                    //int index = DM.FindIndex(item => item.pc_name == u.pc_name);
                    if (procByName.Length > 0)
                    {
                        for (int i = 0; i < v; i++)
                            if (vec[i, 0] == u.pc_name)
                                vec[i, 1] = (int.Parse(vec[i, 1]) + 1).ToString();

                        //DM[index].positive_scan += 1;
                    }
                    //else DM[index].negative_scan += 1;
                    else
                        for (int i = 0; i < v; i++)
                            if (vec[i, 0] == u.pc_name)
                                vec[i, 2] = (int.Parse(vec[i, 2]) + 1).ToString();
                    //Thread.Sleep(1000);


                }
                catch (Exception z)
                {
                    //MessageBox.Show(z.ToString());
                }
                    
                        
                }
            }

            foreach (UsersModel u in UM)
            {
                int p = 0;
                int n = 0;
                for (int i = 0; i < v; i++)
                    if (vec[i, 0] == u.pc_name)
                    {
                        p = int.Parse(vec[i, 1]);
                        n = int.Parse(vec[i, 2]);
                    }


                SqliteDataAccess.AddData(p, n, u.pc_name, CB.Text.ToString(), counter_scn);
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
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BaseIP = IPTB.Text.ToString();
        }
    }
}
