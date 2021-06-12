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
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace UAV_Info
{
    /// <summary>
    /// Window3.xaml 的交互逻辑
    /// </summary>
    public partial class Window3 : Window
    {
        private DispatcherTimer Timer3 = new DispatcherTimer();
        public Window3(string tag)
        {
            InitializeComponent();
            Init();
            int id = int.Parse(tag);
            GetDevInfo(id);
            //Init_Timer();
            //Timer3.Start();
        }
        #region Timer定时器
        private void Init_Timer()
        {
            Timer3.Interval = new TimeSpan(0, 0, 0, 1);            //500ms刷新一次
            Timer3.Tick += new EventHandler(Timer3_Tick);

        }
        private void Timer3_Tick(object sender, EventArgs e)
        {
            int tid = int.Parse(FilledComboBox.SelectedValue.ToString());
            GetDevInfo(tid);
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
            Timer3.Stop();
            this.Close();
        }

        private void W2_Button_Click(object sender, RoutedEventArgs e)
        {
            Window2 windowpage = new Window2();
            windowpage.Show();
            Timer3.Stop();
            this.Close();
        }

        private void W4_Button_Click(object sender, RoutedEventArgs e)
        {
            Window4 windowpage = new Window4(GetHudDefaultId().ToString());
            windowpage.Show();
            Timer3.Stop();
            this.Close();
        }

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
        //获取Unode表中最小ID
        private int GetHudDefaultId()
        {
            return DataDeal.uav_nodes.Count == 0 ? -1 : DataDeal.uav_nodes[0].id;
            //SQLiteConnection con = null;
            //int min_id = 10000;
            //try
            //{
            //    con = new SQLiteConnection("data source=" + "UAV-System.db");
            //    con.Open();
            //    SQLiteCommand cmd = con.CreateCommand();
            //    cmd.CommandText = "select * from Unode";
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
            Timer3.Stop();
            this.Close();
        }

        private void UAV_Posture_Clicked(object sender, RoutedEventArgs e)
        {
            Window4 windowpage = new Window4(GetHudDefaultId().ToString());
            windowpage.Show();
            Timer3.Stop();
            this.Close();
        }

        #endregion

        #region 右Grid功能键
        private void ClearFilledComboBox_Click(object sender, System.Windows.RoutedEventArgs e)
            => FilledComboBox.SelectedItem = null;

        
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            UAV_Cha.IsEnabled = true;
            UAV_Pow.IsEnabled = true;
            UAV_Bdw.IsEnabled = true;
            UAV_Dis.IsEnabled = true;
            //UAV_NId.IsReadOnly = false;
            UAV_Ip.IsReadOnly = false;
            UAV_Mas.IsReadOnly = false;
            UAV_Gtw.IsReadOnly = false;
            UAV_Dns.IsReadOnly = false;
            Timer3.Stop();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            UAV_Cha.IsEnabled = false;
            UAV_Pow.IsEnabled = false;
            UAV_Bdw.IsEnabled = false;
            UAV_Dis.IsEnabled = false;
            UAV_NId.IsReadOnly = true;
            UAV_Ip.IsReadOnly = true;
            UAV_Mas.IsReadOnly = true;
            UAV_Gtw.IsReadOnly = true;
            UAV_Dns.IsReadOnly = true;

            Save_Setting();
        }
        #endregion


        #region Class
        public class UAV_Setting
        {
            public int Id { set; get; }
            public string Devname { set; get; }
            public int Channel { set; get; }
            public int Power { set; get; }
            public int Bandwidth { set; get; }
            public int Distance { set; get; }
            public string IP { set; get; }
            public string Mask { set; get; }
            public string Gateway { set; get; }
            public string DNS { set; get; }
        }

        private class UAV_Config
        {
            public string method;
            public int channel;
            public int power;
            public int bandwidth;
            public int distance;
            public int id;
            public string ip;
            public string mask;
            public string gateway;
            public string dns;
        }
        #endregion

        #region Init
        //初始化选择框
        public void Init()
        {
            int[] channel = new int[] { 1420, 1425, 1430, 1435, 1440, 1445, 1450, 1455, 1460 };
            List<int> ChaS = new List<int>(channel);
            UAV_Cha.ItemsSource = ChaS;

            int[] power = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 };
            List<int> PowS = new List<int>(power);
            UAV_Pow.ItemsSource = PowS;

            int[] bandwidth = new int[] { 5, 10, 20, 40 };
            List<int> BdwS = new List<int>(bandwidth);
            UAV_Bdw.ItemsSource = BdwS;

            int[] distance = new int[] { 0, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 11000, 12000, 13000, 14000 };
            List<int> DisS = new List<int>(distance);
            UAV_Dis.ItemsSource = DisS;

        }
        #endregion

        #region GetInfo

        List<UAV_Setting> UAV_Group = new List<UAV_Setting>();
        public void GetDevInfo(int id)
        {
            UAV_Group.Clear();
            foreach(var config in DataDeal.uav_configs.ToArray())
            {
                UAV_Setting entity = new UAV_Setting();
                entity.Id = config.id;
                entity.Channel = config.channel;
                entity.Power = config.power;
                entity.Bandwidth = config.bandwidth;
                entity.Distance = config.distance;
                entity.IP = config.ip;
                entity.Gateway = config.gateway;
                entity.Mask = config.mask;
                entity.DNS = config.dns;
                entity.Devname = "UAV-" + entity.Id.ToString();
                UAV_Group.Add(entity);
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
            //        cmd.CommandText = "select * from Uconfig";
            //        SQLiteDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            UAV_Setting entity = new UAV_Setting();
            //            for (int i = 0; i < reader.FieldCount; i++)
            //            {
            //                switch (i)
            //                {
            //                    case 0:
            //                        entity.Id = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 1:
            //                        entity.Channel = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 2:
            //                        entity.Power = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 3:
            //                        entity.Bandwidth = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 4:
            //                        entity.Distance = int.Parse(reader[i].ToString());
            //                        break;
            //                    case 5:
            //                        entity.IP = reader[i] as string;
            //                        break;
            //                    case 6:
            //                        entity.Mask = reader[i] as string;
            //                        break;
            //                    case 7:
            //                        entity.Gateway = reader[i] as string;
            //                        break;
            //                    case 8:
            //                        entity.DNS = reader[i] as string;
            //                        break;

            //                }
            //            }
            //            entity.Devname = "UAV" + entity.Id.ToString();
            //            UAV_Group.Add(entity);
            //        }
            //        reader.Close();
            //    }
            //}
            //finally
            //{
            //    if (cn != null)
            //        cn.Close();
            //}
            FilledComboBox.ItemsSource = UAV_Group;
            int id_index = UAV_Group.FindIndex(item => item.Id.Equals(id));
            if (id_index != -1)
            {
                FilledComboBox.SelectedValue = id;
                FilledComboBox.SelectedIndex = id_index;
            }
            else if (UAV_Group.Count != 0)
            {
                FilledComboBox.SelectedValue = UAV_Group[0].Id;
                FilledComboBox.SelectedIndex = 0;
            }
            else
            {
                FilledComboBox.SelectedValue = -1;
                FilledComboBox.SelectedIndex = -1;
                Save.IsEnabled = false;
                Edit.IsEnabled = false;
            }
        }
        #endregion

        #region Save
        public void Save_Setting()
        {
            int tid = int.Parse(FilledComboBox.SelectedValue.ToString());
            DataDeal.UAV_Config now_setting = DataDeal.uav_configs.Find(config => config.id == tid);
            if (now_setting != null)
            {
                Send_To_Server();
                now_setting.channel = int.Parse(UAV_Cha.SelectedValue.ToString());
                now_setting.power = int.Parse(UAV_Pow.SelectedValue.ToString());
                now_setting.bandwidth = int.Parse(UAV_Bdw.SelectedValue.ToString());
                now_setting.distance = int.Parse(UAV_Dis.SelectedValue.ToString());
                now_setting.id = int.Parse(UAV_NId.Text);
                now_setting.ip = UAV_Ip.Text;
                now_setting.mask = UAV_Mas.Text;
                now_setting.gateway = UAV_Gtw.Text;
                now_setting.dns = UAV_Dns.Text;
                //Updata_DB();
            }
            else
            {
                MessageBox.Show("该节点已从集群退出，无法设置", "提示");
                GetDevInfo(-1);
            }

            Timer3.Start();          
        }



        public void Send_To_Server()
        {
            Socket sosocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint IpServe = new IPEndPoint(IPAddress.Parse(GetCurrentLoginIP()), 10000);
            EndPoint RemotePoint = (EndPoint)(IpServe);
            UAV_Config now_setting = new UAV_Config();
            now_setting.method = "SetConfig";
            //now_setting.id = "UAV" + FilledComboBox.SelectedValue.ToString();
            now_setting.channel = int.Parse(UAV_Cha.SelectedValue.ToString());
            now_setting.power = int.Parse(UAV_Pow.SelectedValue.ToString());
            now_setting.bandwidth = int.Parse(UAV_Bdw.SelectedValue.ToString());
            now_setting.distance = int.Parse(UAV_Dis.SelectedValue.ToString());
            now_setting.id = int.Parse(UAV_NId.Text);
            now_setting.ip = UAV_Ip.Text;
            now_setting.mask = UAV_Mas.Text;
            now_setting.gateway = UAV_Gtw.Text;
            now_setting.dns = UAV_Dns.Text;
            string str = JsonConvert.SerializeObject(now_setting);
            try
            {
                if (str.Length > 0)
                {
                    byte[] mess = Encoding.UTF8.GetBytes(str);//Str 转为 Byte值
                   //发送数据
                    sosocket.SendTo(mess, mess.Length, SocketFlags.None, RemotePoint);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "sendserver");
            }
        }
        private string GetCurrentLoginIP()
        {
            ///获取本地的IP地址
            string ThisloginIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    ThisloginIP = _IPAddress.ToString();
                }
            }

            return ThisloginIP;
        }
        public void Updata_DB()
        {
            int oid = int.Parse(FilledComboBox.SelectedValue.ToString());       //原来的id
            int nid = int.Parse(UAV_NId.Text);                                  //新的id
            int channel = int.Parse(UAV_Cha.SelectedValue.ToString());
            int power = int.Parse(UAV_Pow.SelectedValue.ToString());
            int bandwidth = int.Parse(UAV_Bdw.SelectedValue.ToString());
            int distance = int.Parse(UAV_Dis.SelectedValue.ToString());            
            string ip = UAV_Ip.Text;
            string mask = UAV_Mas.Text;
            string gateway = UAV_Gtw.Text;
            string dns = UAV_Dns.Text;            
            
            SQLiteConnection con = new SQLiteConnection("data source=" + "UAV-System.db");
            con.Open();
            if(nid == oid)
            {
                string sqls = string.Format("UPDATE Uconfig SET Channel = {0}, Power = {1}," +
                "Bandwidth = {2}, Distance = {3}, IP = '{4}', Mask = '{5}', Gateway = '{6}', DNS = '{7}' WHERE Id = {8}", channel, power,
                 bandwidth, distance, ip, mask, gateway, dns, oid);
                SQLiteCommand commands = new SQLiteCommand(sqls, con);
                commands.ExecuteNonQuery();
            }
            else
            {
                string dropcon = string.Format("DELETE FROM Uconfig WHERE Id = {0}", oid);
                SQLiteCommand comdpc = new SQLiteCommand(dropcon, con);
                comdpc.ExecuteNonQuery();
                string sqls = string.Format("INSERT INTO Uconfig(Id, Channel, Power, Bandwidth, Distance, IP, Mask, " +
                                    "Gateway, DNS) VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', '{8}')", nid,
                                    channel, power, bandwidth, distance, ip,
                                    mask, gateway, dns);
                SQLiteCommand command = new SQLiteCommand(sqls, con);
                command.ExecuteNonQuery();
            }
            con.Close();
        }

        #endregion

        private void FilledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FilledComboBox.SelectedIndex != -1)
            {
                Save.IsEnabled = true;
                Edit.IsEnabled = true;
            }
        }
    }
}
