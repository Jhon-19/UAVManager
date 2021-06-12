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
using System.Data.SQLite;
using System.Windows.Threading;

namespace UAV_Info
{
    /// <summary>
    /// Window4.xaml 的交互逻辑
    /// </summary>
    public partial class Window4 : Window
    {
        private DispatcherTimer Timer4 = new DispatcherTimer();
        public Window4(string tag)
        {
            InitializeComponent();
            Init_Timer();
            int id = int.Parse(tag);
            GetPosture(id);
            Timer4.Start();
        }

        #region Timer定时器
        private void Init_Timer()
        {
            Timer4.Interval = new TimeSpan(0, 0, 0, 1);            //500ms刷新一次
            Timer4.Tick += new EventHandler(Timer4_Tick);

        }
        private void Timer4_Tick(object sender, EventArgs e)
        {
            int tid = int.Parse(FilledComboBox2.SelectedValue.ToString());
            GetPosture(tid);
        }
        #endregion
        #region 窗口调整
        private void Windows_Move(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch { }
            }
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        Boolean flag_size = false;
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            flag_size = !flag_size;
            if (flag_size == false)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataDeal.addAllLandTime();
            Close();
        }
        #endregion

        #region 窗口跳转
        private void W1_Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 windowpage = new Window1();
            windowpage.Show();
            Timer4.Stop();
            this.Close();
        }

        private void W2_Button_Click(object sender, RoutedEventArgs e)
        {
            Window2 windowpage = new Window2();
            windowpage.Show();
            Timer4.Stop();
            this.Close();
        }

        private void W3_Button_Click(object sender, RoutedEventArgs e)
        {
            Window3 windowpage = new Window3(GetConfigDefaultId().ToString());
            windowpage.Show();
            Timer4.Stop();
            this.Close();
        }
        //获取Uconfig表中最小ID
        private int GetConfigDefaultId()
        {
            return DataDeal.uav_configs.Count == 0 ? -1 : DataDeal.uav_configs[0].id;
            //SQLiteConnection con = null;
            //int min_id = 10000;
            //try
            //{
            //    con = new SQLiteConnection("data source=" + "UAV-System.db");
            //    con.Open();
            //    SQLiteCommand cmd = con.CreateCommand();
            //    cmd.CommandText = "select * from Uconfig";
            //    SQLiteDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        if (int.Parse(reader[0].ToString()) < min_id)
            //        {
            //            min_id = int.Parse(reader[0].ToString());
            //        }
            //    }
            //    reader.Close();
            //}
            //finally
            //{
            //    if (con != null)
            //    {
            //        con.Close();
            //    }
            //}
            //return min_id;
        }
        private void W5_Button_Click(object sender, RoutedEventArgs e)
        {
            Window5 windowpage = new Window5();
            windowpage.Show();
            Timer4.Stop();
            this.Close();
        }
        
        private void UAV_Setting_Clicked(object sender, RoutedEventArgs e)
        {
            Window3 windowpage = new Window3(GetConfigDefaultId().ToString());
            windowpage.Show();
            Timer4.Stop();
            this.Close();
        }
        #endregion

        #region 右Grid功能按键
        private void ClearFilledComboBox2_Click(object sender, System.Windows.RoutedEventArgs e)
            => FilledComboBox2.SelectedItem = null;
        #endregion


        #region Data
        public class UAV_Posture
        {
            public int Id { set; get; }
            public string Devname { set; get; }
            public double Altitude { set; get; }
            public double Yaw { set; get; }
            public double Pitch { set; get; }
            public double Roll { set; get; }
        }
        public List<UAV_Posture> UAV_Group2 = new List<UAV_Posture>();
        public void GetPosture(int id)
        {
            UAV_Group2.Clear();
            foreach(var uno in DataDeal.uav_nodes.ToArray())
            {
                UAV_Posture entity = new UAV_Posture();
                entity.Altitude = uno.altitude;
                entity.Yaw = uno.yaw;
                entity.Pitch = uno.pitch;
                entity.Roll = uno.roll;
                entity.Id = uno.id;
                entity.Devname = entity.Devname = "UAV-" + entity.Id.ToString();
                UAV_Group2.Add(entity);
            }
            //SQLiteConnection cn = null;
            //try
            //{
            //    string path = "UAV-System.db";
            //    //创建一个sqllite连接
            //    cn = new SQLiteConnection("data source=" + path);
            //    if (cn.State != System.Data.ConnectionState.Open)
            //    {
            //        cn.Open();
            //        SQLiteCommand cmd = cn.CreateCommand();
            //        cmd.CommandText = "select * from Unode";
            //        SQLiteDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            UAV_Posture entity = new UAV_Posture();
            //            for (int i = 0; i < reader.FieldCount; i++)
            //            {
            //                switch (i)
            //                {
            //                    case 0:
            //                        entity.Id = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 1:
            //                        break;
            //                    case 2:
            //                        break;
            //                    case 3:
            //                        entity.Altitude = double.Parse(reader[i].ToString());
            //                        break;
            //                    case 4:
            //                        entity.Yaw = double.Parse(reader[i].ToString());
            //                        break;
            //                    case 5:
            //                        entity.Pitch = double.Parse(reader[i].ToString());
            //                        break;
            //                    case 6:
            //                        entity.Roll = double.Parse(reader[i].ToString());
            //                        break;
            //                }
            //            }
            //            entity.Devname = "UAV" + entity.Id.ToString();
            //            UAV_Group2.Add(entity);
            //        }
            //        reader.Close();
            //    }
            //}
            //finally
            //{
            //    if (cn != null)
            //        cn.Close();
            //}
            FilledComboBox2.ItemsSource = UAV_Group2;
            int id_index = UAV_Group2.FindIndex(item => item.Id.Equals(id));
            if (id_index != -1)
            {
                FilledComboBox2.SelectedValue = id;
                FilledComboBox2.SelectedItem = id_index;
            }
            else if (UAV_Group2.Count != 0)
            {
                FilledComboBox2.SelectedValue = UAV_Group2[0].Id;
                FilledComboBox2.SelectedIndex = 0;
            }
            else FilledComboBox2.SelectedValue = -1;
        }
        #endregion
    }
}
