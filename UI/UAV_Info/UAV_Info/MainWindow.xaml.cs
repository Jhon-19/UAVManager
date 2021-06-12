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
using System.Data.SQLite;

namespace UAV_Info
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User_Pass> Reg_User = new List<User_Pass>();
        public MainWindow()
        {
            InitializeComponent();
            Init_Sys_User();
        }
        /// <summary>
        /// 最小化、关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Windows_Move(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch { }
            }
        }
        /// <summary>
        /// 加载用户表
        /// </summary>
        private class User_Pass
        {
            public string Username;
            public string Password;
        }
        private void Init_Sys_User()
        {
            SQLiteConnection cn = null; 
            try
            {
                string path = "UAV-System.db";
                cn = new SQLiteConnection("data source=" + path);
                if(cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    bool UserTableExist = false;
                    SQLiteCommand cmd = cn.CreateCommand();
                    cmd.CommandText = "select * from sqlite_master where type='table' and name='User' ";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserTableExist = true;
                        }
                    }
                    if (!UserTableExist)
                    {
                        cmd.CommandText = "create table User(Username TEXT, Password TEXT)";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO User(Username, Password) VALUES ('admin', 'admin')";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "select * from User";
                    SQLiteDataReader reader1 = cmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        User_Pass entity = new User_Pass();
                        for(int i = 0; i < 2; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    entity.Username = reader1[i].ToString();
                                    break;
                                case 1:
                                    entity.Password = reader1[i].ToString();
                                    break;
                            }
                        }
                        Reg_User.Add(entity);
                    }
                    reader1.Close();
                }
            }
            finally
            {
                if (cn != null)
                    cn.Close();
            }
        }
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string UserName = TxtUserName.Text;
            string PassWord = TxtPassword.Password;
            if(String.Compare(UserName, "") == 0)
            {
                MessageBox.Show("The User Name can't be empty.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(String.Compare(PassWord, "") == 0)
            {
                MessageBox.Show("The Password can't be empty.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(Is_Registered(UserName, PassWord))
            {
                //用户为已注册用户，开启系统
                DataDeal dataDeal = new DataDeal();
                Window1 windowpage = new Window1();
                //if (dataDeal.Set_Done)
                ///{
                    windowpage.Show();
                    this.Close();
                //}                
            }
        }

        //检验用户名密码是否已注册
        private bool Is_Registered(string name, string pass)
        {
            bool Name_Not_Right = true;
            bool Pass_Not_Right = false;
            foreach(User_Pass uspa in Reg_User)
            {
                if (uspa.Username == name)
                {
                    Name_Not_Right = false;
                    if(uspa.Password == pass)
                    {
                        return true;
                    }
                    else
                    {
                        Pass_Not_Right = true;
                    }
                }
            }
            if (Name_Not_Right)
            {
                MessageBox.Show("The User Name doesn't exist.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (Pass_Not_Right)
            {
                MessageBox.Show("User Name is right, but Password is not right.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return false;
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            string UserName = TxtUserName.Text;
            string PassWord = TxtPassword.Password;
            if(String.Compare(UserName, "") == 0)
            {
                MessageBox.Show("Please input the User Name.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(String.Compare(PassWord, "") == 0)
            {
                MessageBox.Show("Please input the Password.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (Name_Exist(UserName))
            {
                MessageBox.Show("The User Name already exists. Please change another.", "UAV Info", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Register_New(UserName, PassWord);
            }
        }

        private bool Name_Exist(string name)
        {
            foreach(User_Pass usna in Reg_User)
            {
                if(usna.Username == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void Register_New(string name, string pass)
        {
            User_Pass upn = new User_Pass();
            upn.Username = name;
            upn.Password = pass;
            Reg_User.Add(upn);
            SQLiteConnection conn = new SQLiteConnection("data source=" + "UAV-System.db");
            conn.Open();
            string sqlins = string.Format("INSERT INTO User(Username, Password) VALUES ('{0}','{1}')", name, pass);
            SQLiteCommand cmdins = new SQLiteCommand(sqlins, conn);
            cmdins.ExecuteNonQuery();
            conn.Close();
        }

        private void TxtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
