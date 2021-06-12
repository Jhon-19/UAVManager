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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace UAV_Info
{
    /// <summary>
    /// Window5.xaml 的交互逻辑
    /// </summary>
    public partial class Window5 : Window
    {
        private DispatcherTimer Timer5 = new DispatcherTimer();
        List<A_Log> UAV_Logs = new List<A_Log>();
        List<Path_Posture> UAVs_Ontime = new List<Path_Posture>();
        private int marker_no = 0;
        public Window5()
        {
            InitializeComponent();
            Init_Timer();
            GetLogInfo();
        }
        #region Timer定时器
        private void Init_Timer()
        {
            Timer5.Interval = new TimeSpan(0, 0, 0, 0, 100);            //100ms刷新一次
            Timer5.Tick += new EventHandler(Timer5_Tick);

        }
        private void Timer5_Tick(object sender, EventArgs e)
        {
            int max = UAVs_Ontime.Count();
            ShowOntime(UAVs_Ontime[marker_no++]);
            if (marker_no == max)
            {
                Timer5.Stop();
                marker_no = 0;
            }
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
            Timer5.Stop();
            this.Close();
        }

        private void W2_Button_Click(object sender, RoutedEventArgs e)
        {
            Window2 windowpage = new Window2();
            windowpage.Show();
            Timer5.Stop();
            this.Close();
        }

        private void W3_Button_Click(object sender, RoutedEventArgs e)
        {
            Window3 windowpage = new Window3(GetConfigDefaultId().ToString());
            windowpage.Show();
            Timer5.Stop();
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
        private void W4_Button_Click(object sender, RoutedEventArgs e)
        {
            Window4 windowpage = new Window4(GetHudDefaultId().ToString());
            windowpage.Show();
            Timer5.Stop();
            this.Close();
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
        #endregion
        #region 功能按键

        private void Show_Again_Clicked(object sender, RoutedEventArgs e)
        {
            mapControl2.Markers.Clear();
            Timer5.Start();
            
        }
        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            mapControl2.Markers.Clear();
            Timer5.Stop();
            SQLiteConnection cn = null;
            try
            {
                string path = "FlyLog.db";
                //创建一个sqllite连接
                cn = new SQLiteConnection("data source=" + path);
                if (cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    SQLiteCommand cmd = cn.CreateCommand();
                    cmd.CommandText = string.Format("Drop table '{0}'", FilledComboBox3.SelectedValue.ToString());
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("DELETE FROM Tasks WHERE TaskId = '{0}'", FilledComboBox3.SelectedValue.ToString());
                    cmd.ExecuteNonQuery();
                    UAV_Logs.Remove(UAV_Logs.Find(log => log.TaskId == FilledComboBox3.SelectedValue.ToString()));
                    FilledComboBox3.ItemsSource = null;
                    FilledComboBox3.ItemsSource = UAV_Logs;
                    mydatagrid.ItemsSource = null;
                    mydatagrid.ItemsSource = UAV_Logs;
                }
            }
            finally
            {
                if (cn != null)
                    cn.Close();
            }
            FilledComboBox3.SelectedIndex = -1;

        }
        
        private void Delete_All_Clicked(object sender, RoutedEventArgs e)
        {
            mapControl2.Markers.Clear();
            Timer5.Stop();
            SQLiteConnection cn = null;
            try
            {
                string path = "FlyLog.db";
                //创建一个sqllite连接
                cn = new SQLiteConnection("data source=" + path);
                if (cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    foreach(var log in UAV_Logs.ToArray())
                    {
                        SQLiteCommand cmd = cn.CreateCommand();
                        cmd.CommandText = string.Format("Drop table '{0}'", log.TaskId);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("DELETE FROM Tasks WHERE TaskId = '{0}'", log.TaskId);
                        cmd.ExecuteNonQuery();                   
                    }
                    UAV_Logs.Clear();
                    FilledComboBox3.ItemsSource = null;
                    mydatagrid.ItemsSource = null;
                }
            }
            finally
            {
                if (cn != null)
                    cn.Close();
            }
            FilledComboBox3.SelectedIndex = -1;

        }

        private void FilledComboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Timer5.Stop();
            marker_no = 0;
            mapControl2.Markers.Clear();
            if (FilledComboBox3.SelectedIndex != -1)
            {
                GetFlyLog(FilledComboBox3.SelectedValue.ToString());
                delete_bt.IsEnabled = true;
            }              
            else
                delete_bt.IsEnabled = false;
        }
        #endregion

        #region Class
        //日志
        public class A_Log
        {
            public int Id { set; get; }
            public string Devname { set; get; }
            public string TaskId { set; get; }
            public string FlyTime { set; get; }
            public string LandTime { set; get; }
        }

        //路径、姿态
        public class Path_Posture
        {
            public double Latitude { set; get; }
            public double Longitude { set; get; }
            public double Altitude { set; get; }
            public double Yaw { set; get; }
            public double Pitch { set; get; }
            public double Roll { set; get; }
        }
        #endregion

        #region Get Info
        //获得日志表格
        public void GetLogInfo()
        {
            SQLiteConnection cn = null;
            string max_taskid = "0";
            try
            {
                string path = "FlyLog.db";
                //创建一个sqllite连接
                cn = new SQLiteConnection("data source=" + path);
                if (cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    SQLiteCommand cmd = cn.CreateCommand();
                    cmd.CommandText = "select * from Tasks";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        A_Log entity = new A_Log();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    entity.Id = int.Parse(reader[i].ToString());
                                    break;
                                case 1:
                                    entity.Devname = reader[i] as string;
                                    break;
                                case 2:
                                    entity.TaskId = reader[i].ToString();
                                    if(string.Compare(entity.TaskId, max_taskid) > 0)
                                    {
                                        max_taskid = entity.TaskId;
                                    }
                                    break;
                                case 3:
                                    entity.FlyTime = reader[i].ToString();
                                    break;
                                case 4:
                                    entity.LandTime = reader[i].ToString();
                                    break;
                            }
                        }
                        DateTime flytime = Convert.ToDateTime(entity.FlyTime);
                        DateTime landtime = Convert.ToDateTime(entity.LandTime);
                        TimeSpan sp = landtime - flytime;
                        if (sp.TotalSeconds > 0)
                        {
                            UAV_Logs.Add(entity);
                        }
                    }
                    reader.Close();
                }
            }
            finally
            {
                if (cn != null)
                    cn.Close();
            }
            mydatagrid.ItemsSource = UAV_Logs;
            FilledComboBox3.ItemsSource = UAV_Logs;
            delete_bt.IsEnabled = false;
        }
        public void GetFlyLog(string table_name)
        {
            UAVs_Ontime.Clear();
            SQLiteConnection cn = null;
            try
            {
                string path = "FlyLog.db";
                //创建一个sqllite连接
                cn = new SQLiteConnection("data source=" + path);
                if (cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    SQLiteCommand cmd = cn.CreateCommand();
                    cmd.CommandText = string.Format("select * from '{0}'", table_name);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Path_Posture entity = new Path_Posture();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        entity.Latitude = double.Parse(reader[i].ToString());
                                        break;
                                    case 1:
                                        entity.Longitude = double.Parse(reader[i].ToString());
                                        break;
                                    case 2:
                                        entity.Altitude = double.Parse(reader[i].ToString());
                                        break;
                                    case 3:
                                        entity.Yaw = double.Parse(reader[i].ToString());
                                        break;
                                    case 4:
                                        entity.Pitch = double.Parse(reader[i].ToString());
                                        break;
                                    case 5:
                                        entity.Roll = double.Parse(reader[i].ToString());
                                        break;
                                }
                            }
                            UAVs_Ontime.Add(entity);
                        }
                        reader.Close();
                    }
                    
                }
            }
            finally
            {
                if (cn != null)
                    cn.Close();
            }
            MapLoad();
            Timer5.Start();
        }
        #endregion

        #region Map
        public void MapLoad()
        {
            mapControl2.MapProvider = Map.AMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapControl2.ShowCenter = false;
            mapControl2.DragButton = MouseButton.Left;
            mapControl2.Position = GetCenter();
        }

        //获取地图中心点
        public PointLatLng GetCenter()
        {
            double[] lat = new double[UAVs_Ontime.Count];
            double[] lon = new double[UAVs_Ontime.Count];
            int i = 0, j = 0;
            foreach (Path_Posture loc in UAVs_Ontime)
            {
                lat[i++] = Convert.ToDouble(loc.Latitude);
                lon[j++] = Convert.ToDouble(loc.Longitude);
            }
            PointLatLng result = new PointLatLng();
            try
            {
                double max_lat = lat.Max();
                double min_lat = lat.Min();
                double mid_lat = (max_lat + min_lat) / 2;
                double max_lon = lon.Max();
                double min_lon = lon.Min();
                double mid_lon = (max_lon + min_lon) / 2;
                result = new PointLatLng(mid_lat, mid_lon);
            }
            catch (Exception)
            {
                result = new PointLatLng(30.527958961746585, 114.36127855405148);
            }

            return result;
        }

        private void ShowOntime(Path_Posture pa_po)
        {
            PointLatLng pt = new PointLatLng(pa_po.Latitude, pa_po.Longitude);
            GMapMarker marker = new GMapMarker(pt);
            marker.Shape = CreatePinImage(marker, mapControl2.Zoom);
            mapControl2.Markers.Add(marker);
            hud2.Altitude = pa_po.Altitude;
            hud2.YawAngle = pa_po.Yaw;
            hud2.PitchAngle = pa_po.Pitch;
            hud2.RollAngle = pa_po.Roll;
        }

        BitmapImage _pinSrcImage;
        Image CreatePinImage(GMapMarker marker, double zoom)
        {
            Image img = new Image();
            img.Tag = marker;
            img.Width = zoom * 2 + 4;
            img.Height = zoom * 2 + 4;

            if (_pinSrcImage == null)
            {
                //多个标注共用一个图像源，节省内存。
                _pinSrcImage = new BitmapImage(new Uri("uav.png", UriKind.Relative));
                _pinSrcImage.Freeze();
            }
            img.Source = _pinSrcImage;
            //鼠标热点位置
            marker.Offset = new Point(-img.Width / 2, -img.Height / 2);
            return img;
        }
        #endregion

    }
}
