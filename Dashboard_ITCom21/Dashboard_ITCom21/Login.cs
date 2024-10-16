using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard_ITCom21
{
    public partial class Login : Form
    {
        public static string UserName = "";
        public static int UserId = 0;
        public static int UserIdChangepassword = 0;

        public Login()
        {
            InitializeComponent();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataITCOMDataContext db = new DataITCOMDataContext();
                var flag = false;
                var queryUser = (from a in db.user_tbls
                                 where a.user_name == txtUsername.Text.Trim() && a.password == txtPassword.Text.Trim()
                                 select a).FirstOrDefault();

                if (queryUser != null)
                {
                    flag = true;
                    UserName = queryUser.full_name;
                    UserId = queryUser.id;
                }

                if (flag)
                {
                    ImportData fr = new ImportData();
                    fr.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Thông tin không chính xác.");
                }
            }
            catch
            {
                MessageBox.Show("Lỗi kết nối.");
            }
            
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void lbldoimatkhau_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản.");
                return;
            }

            DataITCOMDataContext db = new DataITCOMDataContext();
            var queryUser = (from a in db.user_tbls
                             where a.user_name == txtUsername.Text.Trim()
                             select a).FirstOrDefault();

            if (queryUser==null)
            {
                MessageBox.Show("Tên tài khoản không tồn tại.");
                return;
            }
            UserIdChangepassword = queryUser.id;
            UserName= queryUser.user_name;
            ChangePassword cp = new ChangePassword();
            cp.Show();
            this.Hide();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
