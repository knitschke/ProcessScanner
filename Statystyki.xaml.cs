using PScnFin.Models;
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
using System.Data;

namespace PScnFin
{
    /// <summary>
    /// Interaction logic for Statystyki.xaml
    /// </summary>
    public partial class Statystyki : Window
    {
        List<UsersModel> UM = new List<UsersModel>();
        List<ProcessesModel> PM = new List<ProcessesModel>();
        List<ScansModel> SM = new List<ScansModel>();
        List<DataModel> DM = new List<DataModel>();
        List<DataModel> DMtime = new List<DataModel>();
        float time = 0;
        //float timeforall = 0;
        string slcted = "";
        string slctedproc = "";
        public Statystyki()
        {
            InitializeComponent();
            
            LoadUsersList();
            LoadProcsList();
            LoadScanList();
        }
        private void LoadScanList()
        {
            SM = SqliteDataAccess.LoadScans();
        }
        private void LoadProcsList()
        {
            PM = SqliteDataAccess.LoadProcs();
            foreach (ProcessesModel o in PM)
            {
                CB.Items.Add(o.process_name);
            }
            CB.SelectedIndex = 0;
            /*
            CB.ItemsSource = null;
            CB.ItemsSource = PM;
            
            CB.DisplayMemberPath = "process_name";
            */
        }
        private void LoadUsersList()
        {
            UM = SqliteDataAccess.LoadUsers();
            /*LB.ItemsSource = null;
            LB.ItemsSource = UM;
            LB.DisplayMemberPath = "full";*/

            foreach (UsersModel o in UM)
            {
                LB.Items.Add(o.full);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            slctedproc = CB.SelectedItem.ToString();
            int countp = 0;
            int countpall = 0;
            int countn = 0;
            int countnall = 0;
            time = 0;
            DM = SqliteDataAccess.LoadData();
            string xx = "";
            for (int i = 0; i < slcted.Length; i++)
            {
                if (slcted[i] != ' ')
                    xx += slcted[i];
                else break;
            }
            foreach (DataModel o in DM)
            {
                if (o.pc_name==xx && o.process_name ==slctedproc)
                {
                    countp += o.positive_scan;
                    countn += o.negative_scan;
                    foreach(ScansModel oo in SM)
                    {
                        if (oo.scan_id == o.scan_id)
                            time += oo.time;
                    }
                }
                
            }
            float x = 0;
            if (countp + countn > 0)
                x= (countp * 100) / (countp + countn);
            T1.Text=x.ToString() + "%";
            T2.Text = (time).ToString();//narazie

            DMtime = SqliteDataAccess.LoadDataTime(slctedproc);
            foreach(DataModel t in DMtime)
            {
                countpall += t.positive_scan;
                countnall += t.negative_scan;
            }
            if ((countnall + countpall) == 0)
                T3.Text = "0";
            else
            T3.Text = ((countpall * 100) / (countpall+countnall)).ToString() + "%";


        }

        private void LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(LB.SelectedValue.ToString());
            slcted = LB.SelectedItem.ToString();
            
        }
    }
}
