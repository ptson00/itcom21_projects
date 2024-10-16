using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard_ITCom21
{
    public partial class ChangePassword : Form
    {
        static int userId = 0;
        public ChangePassword()
        {
            InitializeComponent();
            lblusername.Text = Login.UserName;
            userId = Login.UserIdChangepassword;
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            DataITCOMDataContext db = new DataITCOMDataContext();

            var queryPass = (from a in db.user_tbls
                             where a.id == userId && a.password == txtpasscu.Text.Trim()
                             select a).FirstOrDefault();
            if (queryPass != null)
            {
                if (txtpassmoi.Text.Trim() != txtpassxacnhan.Text.Trim())
                {
                    MessageBox.Show("Xác nhận mật khẩu mới không chính xác.");
                    return;
                }
                queryPass.password = txtpassmoi.Text.Trim();
                db.SubmitChanges();
                if (MessageBox.Show("Đổi mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Login lg = new Login();
                    lg.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Mật khẩu không chính xác.");
                return;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }

        private void ChangePassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
