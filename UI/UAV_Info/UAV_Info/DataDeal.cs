using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;

namespace UAV_Info
{
    class DataDeal
    {
        #region 全局变量
        //网络相关
        private Socket SocClient;
        private IPEndPoint IpClient;
        private EndPoint RemotePoint;
        private IPEndPoint IpServer;

        //全局数据
        public static  List<UAV_Node> uav_nodes = new List<UAV_Node>();
        public static List<UAV_Link> uav_links = new List<UAV_Link>();
        public static List<UAV_Config> uav_configs = new List<UAV_Config>();

        //数据库
        public static SQLiteConnection cn = null;             //连接系统表格
        public static SQLiteConnection conn = null;           //连接历史飞行数据表

        //定时器相关
        private DispatcherTimer TimerNode = new DispatcherTimer();
        private DispatcherTimer TimerLog = new DispatcherTimer();

        //飞行任务字典
        public static Dictionary<int, string> LogDictionary = new Dictionary<int, string>();

        public bool Set_Done = false;

        private int Ins_Log = 0;
        #endregion

        //主函数
        public DataDeal()
        {
            ConnectServer();        //连接服务器
            InitDataBase();         //初始化数据库
            RequestNode();          //请求节点信息
            InitTimer();            //定时器设置

        }
        #region 定时器
        void InitTimer()
        {
            TimerNode.Interval = new TimeSpan(0, 0, 0, 1);
            TimerNode.Tick += new EventHandler(TimerNode_Tick);
            TimerNode.Start();

            //TimerLog.Interval = new TimeSpan(0, 0, 0, 1);
            //TimerLog.Tick += new EventHandler(TimerLog_Tick);
            //TimerLog.Start();
        }

        private void TimerNode_Tick(object sender, EventArgs e)
        {
            RequestNode();
            //Ins_Log++;
            //if(Ins_Log % 30 == 0)
            //{
            //    foreach (UAV_Node uAV in uav_nodes)
            //    {
            //        int id = uAV.id;
            //        if (LogDictionary.ContainsKey(id))
            //        {
            //            string sqlinslog = string.Format("INSERT INTO '{0}'(Latitude, Longitude, Altitude, Yaw, Pitch, Roll) VALUES ({1}, " +
            //                "{2}, {3}, {4}, {5}, {6})", LogDictionary[id], uAV.latitude, uAV.longitude, uAV.altitude, uAV.yaw, uAV.pitch,
            //                uAV.roll);
            //            SQLiteCommand cmdinslog = new SQLiteCommand(sqlinslog, conn);
            //            cmdinslog.ExecuteNonQuery();
            //        }
            //    }
            //}
        }

        private void TimerLog_Tick(object sender, EventArgs e)
        {
            foreach (var uAV in uav_nodes.ToArray())
            {
                int id = uAV.id;
                if (LogDictionary.ContainsKey(id))
                {
                    string sqlinslog = string.Format("INSERT INTO '{0}'(Latitude, Longitude, Altitude, Yaw, Pitch, Roll) VALUES ({1}, " +
                        "{2}, {3}, {4}, {5}, {6})", LogDictionary[id], uAV.latitude, uAV.longitude, uAV.altitude, uAV.yaw, uAV.pitch,
                        uAV.roll);
                    SQLiteCommand cmdinslog = new SQLiteCommand(sqlinslog, conn);
                    cmdinslog.ExecuteNonQuery();
                }
            }
            
        }
        #endregion
        #region UDP相关
        //连接服务器
        void ConnectServer()
        {
            try
            {
                //设置服务器IP和端口
                IpServer = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 10000);
                RemotePoint = (EndPoint)(IpServer);
                //定义UDP连接
                SocClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //设置本程序的IP和端口，此处自动获取本机IP，端口设置为9000
                IpClient = new IPEndPoint(IPAddress.Parse(GetIpAddress()), 9000);
                SocClient.Bind(IpClient);       //绑定IP和端口

                Thread thread = new Thread(new ThreadStart(ListenServer));
                thread.IsBackground = false;
                thread.Start();
            }
            catch(Exception ex)
            {   //输出错误信息
                MessageBox.Show(ex.Message, "提示");
            }
        }
        //获取本机IP
        string GetIpAddress()
        {
            string ThisLoginIp = string.Empty;
            foreach(IPAddress iPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {   //获取IPV4地址
                if(iPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    ThisLoginIp = iPAddress.ToString();
                }
            }
            return ThisLoginIp; 
        }
        //监听服务器发送来的报文数据
        void ListenServer()
        {
            bool isLost = false;
            Console.WriteLine("start listening");
            while (true)
            {
                try
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);//定义要发送的计算机的地址
                    EndPoint Remote = (EndPoint)(sender);//
                    int recv;       //返回收到的字节数
                                    //启动新线程，接收数据并进行处理
                    while (true)
                    {
                        byte[] data = new byte[1024 * 1024];
                        recv = SocClient.ReceiveFrom(data, ref Remote);
                        isLost = false;
                        Console.WriteLine("haha");
                        string MsgStr = Encoding.UTF8.GetString(data, 0, recv);
                        try
                        {

                            JObject json = JObject.Parse(MsgStr);
                            if (json["method"].ToString() == "nodetopo")
                            {

                                NodetopoJson Unodetopo = JsonConvert.DeserializeObject<NodetopoJson>(MsgStr);
                                DealNode(Unodetopo.node);
                                DealLink(Unodetopo);
                            }
                            else if (json["method"].ToString() == "devconfig")
                            {

                                UAV_Config aV_Config = new UAV_Config();
                                aV_Config.id = Convert.ToInt32(json["id"].ToString());
                                aV_Config.channel = Convert.ToInt32(json["channel"].ToString());
                                aV_Config.power = Convert.ToInt32(json["power"].ToString());
                                aV_Config.bandwidth = Convert.ToInt32(json["bandwidth"].ToString());
                                aV_Config.distance = Convert.ToInt32(json["distance"].ToString());
                                aV_Config.ip = json["ip"].ToString();
                                aV_Config.mask = json["mask"].ToString();
                                aV_Config.gateway = json["gateway"].ToString();
                                aV_Config.dns = json["dns"].ToString();
                                bool IdExist = uav_configs.Any(u => u.id == aV_Config.id);
                                if (!IdExist)       //未存在本id的数据
                                {
                                    uav_configs.Add(aV_Config);
                                    //string sqls = string.Format("INSERT INTO Uconfig(Id, Channel, Power, Bandwidth, Distance, IP, Mask, " +
                                    //    "Gateway, DNS) VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', '{8}')", aV_Config.id,
                                    //    aV_Config.channel, aV_Config.power, aV_Config.bandwidth, aV_Config.distance, aV_Config.ip,
                                    //    aV_Config.mask, aV_Config.gateway, aV_Config.dns);
                                    //SQLiteCommand command = new SQLiteCommand(sqls, cn);
                                    //command.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message, "listenServer1");
                        }
                    }
                }
                catch (Exception ex)
                {   //输出错误信息
                    if(!isLost)
                        MessageBox.Show(ex.Message, "listening thread");
                    isLost = true;
                }
            }
        }
        //请求节点信息
        void RequestNode()
        {
            RequestNodeJson request = new RequestNodeJson();
            request.method = "nodetopo";
            string order = JsonConvert.SerializeObject(request);
            try
            {
                if(order.Length > 0)
                {
                    byte[] mess = Encoding.UTF8.GetBytes(order);
                    SocClient.SendTo(mess, mess.Length, SocketFlags.None, RemotePoint);
                }
            }
            catch (Exception ex) { Console.WriteLine("requestNode Eception"); }
        }
        
        //请求id节点的设置信息
        void RequestConfig(int tid)
        {
            RequestConfigJson request = new RequestConfigJson();
            request.method = "devconfig";
            request.id = tid;
            string order = JsonConvert.SerializeObject(request);
            try
            {
                if (order.Length > 0)
                {
                    byte[] mess = Encoding.UTF8.GetBytes(order);
                    SocClient.SendTo(mess, mess.Length, SocketFlags.None, RemotePoint);
                }
            }
            catch (Exception ex) { Console.WriteLine("RequestConfig Eception"); }
        }

        /// <summary>
        /// 报文处理
        /// </summary>
        //处理节点
        void DealNode(List<UAV_Node> uAVs)
        {
            if (uAVs[0] == null) return;
            if(uav_nodes.Count != 0)
            {
                foreach(var uno in uav_nodes.ToArray())      //删除新节点中不存在的旧节点
                {
                    bool IsDelete = uAVs.Any(u => u.id == uno.id);
                    if (!IsDelete)
                    {
                        //UAV_Node tmp = uav_nodes.Find(u => u.id == uno.id);
                        //uav_nodes.Remove(tmp);
                        //string dropnode = string.Format("DELETE FROM Unode WHERE Id = {0}", uno.id);
                        //SQLiteCommand comdpn = new SQLiteCommand(dropnode, cn);
                        //comdpn.ExecuteNonQuery();
                        addLandTime(uno.id);
                        LogDictionary.Remove(uno.id);
                        uav_nodes.Remove(uno);                        
                    }
                }
            }
            foreach(UAV_Node uvn in uAVs)
            {
                bool IdAlready = uav_nodes.Any(u => u.id == uvn.id);
                if (IdAlready)          //更新新节点中存在的旧节点
                {
                    UAV_Node tmp = uav_nodes.Find(u => u.id == uvn.id);
                    uav_nodes.Remove(tmp);      //先删除该旧节点                                                
                    uav_nodes.Add(uvn);         //再添加该新节点
                    //更新数据库
                    //string sqlupt = string.Format("UPDATE Unode SET Latitude = {0}, Longitude = {1}, Altitude = {2}, Yaw = {3}, Pitch = " +
                    //    "{4}, Roll = {5} WHERE Id = {6}", uvn.latitude, uvn.longitude, uvn.altitude, uvn.yaw, uvn.pitch, uvn.roll, uvn.id);
                    //SQLiteCommand comupt = new SQLiteCommand(sqlupt, cn);
                    //comupt.ExecuteNonQuery();
                    string sqlinslog = string.Format("INSERT INTO '{0}'(Latitude, Longitude, Altitude, Yaw, Pitch, Roll) VALUES ({1}, " +
                        "{2}, {3}, {4}, {5}, {6})", LogDictionary[uvn.id], uvn.latitude, uvn.longitude, uvn.altitude, uvn.yaw, uvn.pitch,
                        uvn.roll);
                    SQLiteCommand cmdinslog = new SQLiteCommand(sqlinslog, conn);
                    cmdinslog.ExecuteNonQuery();
                }
                else                        //添加新节点中新出现的节点
                {
                    uav_nodes.Add(uvn);
                    //string sqladd = string.Format("INSERT INTO Unode(Id, Latitude, Longitude, Altitude, Yaw, Pitch, Roll) VALUES" +
                    //    "({0}, {1}, {2}, {3}, {4}, {5}, {6})", uvn.id, uvn.latitude, uvn.longitude, uvn.altitude, uvn.yaw, uvn.pitch, 
                    //    uvn.roll);
                    //SQLiteCommand cmdadd = new SQLiteCommand(sqladd, cn);
                    //cmdadd.ExecuteNonQuery();

                    //FlyLog
                    CreateLog(uvn);
                }
            }
        }
        void CreateLog(UAV_Node unode)//创建新的历史飞行任务
        {
            DateTime dt = DateTime.Now;
            string dname = "UAV" + unode.id.ToString();
            string taski = dt.ToFileTimeUtc().ToString();
            //string ftime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
            string ftime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
            LogDictionary.Add(unode.id, taski);
            string sqllog = string.Format("INSERT INTO Tasks(Id, Devname, TaskId, FlyTime, LandTime) VALUES ({0}, '{1}', '{2}', '{3}','{4}')", unode.id,
                dname, taski, ftime, ftime);
            SQLiteCommand cmdlog = new SQLiteCommand(sqllog, conn);
            cmdlog.ExecuteNonQuery();
            string createlog = string.Format("create table '{0}'(Latitude DOUBLE, Longitude DOUBLE, Altitude DOUBLE, Yaw DOUBLE, Pitch " +
                "DOUBLE, Roll DOUBLE)", taski);
            SQLiteCommand cmdcrelog = new SQLiteCommand(createlog, conn);
            cmdcrelog.ExecuteNonQuery();
            Console.WriteLine("Create new log");
            string sqlfirst = string.Format("INSERT INTO '{0}'(Latitude, Longitude, Altitude, Yaw, Pitch, Roll) VALUES ({1}, {2}, {3}," +
                "{4}, {5}, {6})", taski, unode.latitude, unode.longitude, unode.altitude, unode.yaw, unode.pitch, unode.roll);
            SQLiteCommand cmdfirst = new SQLiteCommand(sqlfirst, conn);
            cmdfirst.ExecuteNonQuery();
        }
        void addLandTime(int uavId)
        {
            if (LogDictionary.ContainsKey(uavId))
            {
                string addlandtime = string.Format("UPDATE Tasks SET LandTime = '{0}' WHERE TaskId = '{1}'", string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now), LogDictionary[uavId]);
                SQLiteCommand cmdinslog = new SQLiteCommand(addlandtime, conn);
                cmdinslog.ExecuteNonQuery();
            }

        }
        public static void addAllLandTime()
        {
            foreach(string taskID in LogDictionary.Values)
            {
                string addlandtime = string.Format("UPDATE Tasks SET LandTime = '{0}' WHERE TaskId = '{1}'", string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now), taskID);
                SQLiteCommand cmdinslog = new SQLiteCommand(addlandtime, conn);
                cmdinslog.ExecuteNonQuery();
            }

        }
        //处理连接
        void DealLink(NodetopoJson nodetp)
        {
            List<UAV_Link> uAV_Links = new List<UAV_Link>();
            int len = nodetp.link_serial.Count();
            for(int i = 0; i < len; i++)        //将连接质量数组改为连接列表
            {
                int linkfrom = nodetp.link_serial[i];
                for(int j = 0; j < len; j++)
                {
                    int linkto = nodetp.link_serial[j];
                    int linkqua = nodetp.link[i][j];
                    UAV_Link tlink = new UAV_Link();
                    tlink.source = linkfrom;
                    tlink.target = linkto;
                    tlink.quality = linkqua;
                    if (linkfrom != linkto)
                    {
                        uAV_Links.Add(tlink);
                    }
                }
            }
            if (uav_links.Count != 0)        //删除新link中不存在的旧link
            {
                foreach (var uli in uav_links.ToArray())
                {
                    bool IsExist = uAV_Links.Any(u => u.source == uli.source && u.target == uli.target);
                    if (!IsExist)
                    {
                        //string droplink = string.Format("DELETE FROM Ulink WHERE Source = {0} AND Target = {1}", uli.source, uli.target);
                        //SQLiteCommand cmddplin = new SQLiteCommand(droplink, cn);
                        //cmddplin.ExecuteNonQuery();
                        uav_links.Remove(uli);
                    }
                }
            }
            foreach (UAV_Link nul in uAV_Links)
            {
                bool LinkExist = uav_links.Any(u => u.source == nul.source && u.target == nul.target);
                if (LinkExist)                  //更新连接质量
                {
                    UAV_Link tmp = uav_links.Find(u => u.source == nul.source && u.target == nul.target);
                    uav_links.Remove(tmp);
                    uav_links.Add(nul);
                    //string sqlupt = string.Format("UPDATE Ulink SET Quality = {0} WHERE Source = {1} AND Target = {2}",
                    //    nul.quality, nul.source, nul.target);
                    //SQLiteCommand cmdupt = new SQLiteCommand(sqlupt, cn);
                    //cmdupt.ExecuteNonQuery();
                }
                else                        //添加新连接
                {
                    uav_links.Add(nul);
                    //string sqladd = string.Format("INSERT INTO Ulink(Source, Target, Quality) VALUES ({0}, {1}, {2})", nul.source, nul.target,
                    //    nul.quality);
                    //SQLiteCommand cmdadd = new SQLiteCommand(sqladd, cn);
                    //cmdadd.ExecuteNonQuery();
                }                
            }
            //检查节点设置
            CheckConfig(nodetp.link_serial);
        }
        //检查节点设置
        void CheckConfig(List<int> node)
        {
            foreach(int uavn in node)       //请求新节点的设置信息
            {
                bool ConInfo = uav_configs.Any(u => u.id == uavn);
                if (!ConInfo)
                {
                    RequestConfig(uavn);
                }
            }
            foreach(var uvc in uav_configs.ToArray())          //删除旧的config
            {
                bool IsStill = node.Any(u => u == uvc.id);
                if (!IsStill)
                {
                    //string dropcon = string.Format("DELETE FROM Uconfig WHERE Id = {0}", uvc.id);
                    //SQLiteCommand cmddpcon = new SQLiteCommand(dropcon, cn);
                    //cmddpcon.ExecuteNonQuery();
                    uav_configs.Remove(uvc);
                }
            }
            if(Set_Done == false)
            {
                Set_Done = true;
            }
        }

        /// <summary>
        /// JSON报文类
        /// </summary>
        public class NodetopoJson   //节点报文
        {
            [JsonIgnore]
            public string method;

            public List<UAV_Node> node;         //节点
            public List<List<int>> link;        //连接质量，二维数组
            public List<int> link_serial;       //连接节点序号

        }
        private class RequestNodeJson       //请求节点的报文
        {
            public string method;
        }
        private class RequestConfigJson     //请求设置报文
        {
            public string method;
            public int id;
        }

        #endregion

        #region 全局数据类
        //节点类
        public class UAV_Node
        {
            public int id;              //id
            public double latitude;     //纬度
            public double longitude;    //经度
            public double altitude;     //海拔
            public double yaw;          //偏航角
            public double pitch;        //俯仰角
            public double roll;         //滚动角(翻滚角)
        }
        //连接类
        public class UAV_Link
        {
            public int source;      //连接起点
            public int target;      //连接终点
            public int quality;     //质量
        }
        //节点设置类
        public class UAV_Config
        {
            public int id;              //id
            public int channel;         //信道
            public int power;           //功率
            public int bandwidth;       //带宽
            public int distance;        //距离
            public string ip;           //IP
            public string mask;         //掩码
            public string gateway;      //网关
            public string dns;          //DNS
        }
        #endregion

        #region 数据库相关
        //初始化数据库
        void InitDataBase()
        {
            InitSystem();           //初始化飞行数据
            InitFlyLog();           //连接飞行记录
        }
        //初始化飞行数据
        void InitSystem()
        {
            string path = "UAV-System.db";
            //创建连接
            cn = new SQLiteConnection("data source=" + path);
            cn.Open();
            bool tableExist = false;
            SQLiteCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select * from sqlite_master where type='table' and name='Unode' ";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    tableExist = true;
                }
            }
            if(tableExist)      //系统表格存在，删除旧表
            {
                cmd.CommandText = "drop table Unode";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "drop table Ulink";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "drop table Uconfig";
                cmd.ExecuteNonQuery();                
            }
            cmd.CommandText = "create table Unode(Id INTEGER, Latitude DOUBLE, Longitude DOUBLE, Altitude DOUBLE, Yaw DOUBLE," +
                    "Pitch DOUBLE, Roll DOUBLE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "create table Ulink(Source INTEGER, Target INTEGER, Quality INTEGER)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "create table Uconfig(Id INTEGER, Channel INTEGER, Power INTEGER, Bandwidth INTEGER, Distance INTEGER," +
                "IP TEXT, Mask TEXT, Gateway TEXT, DNS TEXT)";
            cmd.ExecuteNonQuery();
        }

        void InitFlyLog()
        {
            string path = "FlyLog.db";
            //连接数据库
            conn = new SQLiteConnection("data source=" + path);
            conn.Open();
            bool tableTaskExist = false;
            SQLiteCommand commd = conn.CreateCommand();
            commd.CommandText = "select * from sqlite_master where type='table' and name='Tasks' ";
            using (SQLiteDataReader reader = commd.ExecuteReader())
            {
                if (reader.Read())
                {
                    tableTaskExist = true;
                }
            }
            if (!tableTaskExist)
            {           //创建索引表
                commd.CommandText = "create table Tasks(Id INTEGER, Devname TEXT, TaskId TEXT, FlyTime TEXT, LandTime TEXT)";
                commd.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
