using Dashboard_ITCom21.Models;
using Dashboard_ITCom21.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard_ITCom21
{
    public partial class ImportData : Form
    {
        DataITCOMDataContext db = new DataITCOMDataContext();
        Dictionary<int, List<int>> listRank = new Dictionary<int, List<int>> { };
        Dictionary<string, int> listCost = new Dictionary<string, int> { };
        DataTable DataSearch = new DataTable();
        static int userId = 0;
        static int role = 0;
        static int pageNumber = 0;
        static double tempPage = 0;
        static string CODE = "";
        static int IDAPPLIQA = 0;
        static int IDAPPLIAD = 0;

        public ImportData()
        {
            InitializeComponent();
            LoadForm();
        }

        #region Main
        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text, "[^0-9]"))
            {
                txtPhone.Clear();
            }
        }
        private void LoadForm()
        {
            lblteam.Text = Login.UserName;
            userId = Login.UserId;
            role = GetRoleByID(userId);

            nudPackage.Controls[0].Enabled = false;
            nudNgoaitru.Controls[0].Enabled = false;
            nudTainan.Controls[0].Enabled = false;
            nudThaisan.Controls[0].Enabled = false;
            nudSinhmang.Controls[0].Enabled = false;
            nudNha.Controls[0].Enabled = false;
            nudITC.Controls[0].Enabled = false;
            nudBv.Controls[0].Enabled = false;
            nudASH.Controls[0].Enabled = false;
            nudtangphibv.Controls[0].Enabled = false;
            nudSinhmangInput.Controls[0].Enabled = false;
            nudTainanInput.Controls[0].Enabled = false;

            listCost.Add("core", 0);
            listCost.Add("out", 0);
            listCost.Add("dental", 0);
            listCost.Add("maternity", 0);

            listRank.Add(1, new List<int> { 1, 3, 0 });
            listRank.Add(2, new List<int> { 4, 6, 0 });
            listRank.Add(3, new List<int> { 7, 9, 0 });
            listRank.Add(4, new List<int> { 10, 18, 0 });
            listRank.Add(5, new List<int> { 19, 30, 0 });
            listRank.Add(6, new List<int> { 31, 40, 0 });
            listRank.Add(7, new List<int> { 41, 50, 0 });
            listRank.Add(8, new List<int> { 51, 60, 0 });
            listRank.Add(9, new List<int> { 61, 65, 0 });
            listRank.Add(10, new List<int> { 1, 18, 5 });
            listRank.Add(11, new List<int> { 18, 65, 6 });
            listRank.Add(12, new List<int> { 19, 65, 5 });

            tabPage1.Enabled = false;
            tabPage3.Enabled = false;
            btnUpdate.Enabled = false;
            groupInfoPackage.Enabled = false;


            var listStatus = new List<KeyValuePair<int, string>> { };
            listStatus.Add(new KeyValuePair<int, string>(0, ""));

            var listInsurance = new List<KeyValuePair<int, string>> { };
            listInsurance.Add(new KeyValuePair<int, string>(0, ""));

            var queryStatus = (from a in db.status_appointment_tbls
                               select new { a.Id, a.name }).ToList();
            if (queryStatus.Any())
            {
                foreach (var i in queryStatus)
                {
                    listStatus.Add(new KeyValuePair<int, string>(i.Id, i.name));
                }
            }

            var queryInsurance = (from a in db.insurance_tbls
                                  select new { a.id, a.name }).ToList();
            if (queryInsurance.Any())
            {
                foreach (var i in queryInsurance)
                {
                    listInsurance.Add(new KeyValuePair<int, string>(i.id, i.name));
                }
            }

            cbbInsurence.DataSource = listInsurance;
            cbbInsurence.DisplayMember = "value";
            cbbInsurence.ValueMember = "key";

            cbbStatus.DataSource = listStatus;
            cbbStatus.DisplayMember = "value";
            cbbStatus.ValueMember = "key";

            setRoleForUser();
        }
        private void btnClose_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Không tìm thấy thông tin.");
                return;
            }

            var querySearch = (from a in db.appointment_tbls
                               where a.ap_phone == txtPhone.Text.Trim() || a.code == txtPhone.Text.Trim() || a.in_cccd == txtPhone.Text.Trim()
                               select a).FirstOrDefault();
            if (querySearch == null || (querySearch.created_user != userId && role == 2) || (querySearch.created_user != userId && role == 4))
            {
                MessageBox.Show("Không tìm thấy thông tin.");
                return;
            }

            tabPage1.Enabled = true;
            tabPage3.Enabled = true;

            btnUpdate.Enabled = true;

            setRoleForUser();
            groupInfoPackage.Enabled = true;

            LoadData(querySearch);

            CODE = querySearch.code;
            tabMain.SelectedTab = tabMain.TabPages[0];
            tabMain.TabPages[0].Focus();

            ClearImage();

        }
        private void LoadData(appointment_tbl querySearch)
        {
            txtCode.Text = querySearch.code;
            txtTSR.Text = querySearch.tsr_name;
            dtpDate_info.Value = DateTime.Parse(querySearch.appointment_date);
            if (querySearch.appointment_time == "ONL")
            {
                cbOnline.Checked = true;
            }
            else
            {
                cbOnline.Checked = false;
                dtpTime_info.Value = DateTime.Parse(querySearch.appointment_time);
            }

            txtCustomerName_info.Text = querySearch.ap_customer;
            txtPhone_info.Text = querySearch.ap_phone;
            txtAddress_info.Text = querySearch.ap_address;
            txtRq_customer.Text = querySearch.rq_customer;
            txtRq_cccd.Text = querySearch.rq_cccd;
            cbbRq_sex.SelectedItem = querySearch.rq_sex;
            dtpBirthday.Value = DateTime.Parse(querySearch.rq_birthday);
            txtRq_sdt.Text = querySearch.rq_phone;
            txtRq_address.Text = querySearch.rq_address;
            txtIn_ndbh.Text = querySearch.in_customer;
            txtIn_sdt.Text = querySearch.in_cccd;
            cbbIn_sex.SelectedItem = querySearch.in_sex;
            dtpIn_birthday.Value = DateTime.Parse(querySearch.in_birthday);
            cbbIn_relationship.SelectedItem = querySearch.in_relationship;
            txtBe_nth.Text = querySearch.be_customer;
            cbbBe_sex.SelectedItem = querySearch.be_sex;
            txtBe_brithday.Text = querySearch.be_birthday;
            txtBe_id.Text = querySearch.be_id;
            cbbBe_relationship.SelectedItem = querySearch.be_relationship;
            txt_note.Text = querySearch.note;
            lblteam.Text = querySearch.team_name;
            cbbStatus.SelectedValue = querySearch.status_ap;
            txtGr_cccd_sdt.Text = querySearch.gr_cccd_sdt;
            nudGr_sl.Value = querySearch.gr_sl == null ? 0 : (decimal)querySearch.gr_sl;
            txtGr_note.Text = querySearch.gr_note;
            cbbCCCD.SelectedItem = querySearch.cccd_status;

            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            DateTime createdDate = querySearch.created_date == null ? DateTime.Now : querySearch.created_date.Value;
            var age = (int.Parse(createdDate.ToString("yyyyMMdd")) - int.Parse(temp)) / 10000;
            lblAge.Text = age.ToString();

            var queryCost = (from a in db.appointment_cost_tbls
                             where a.appointment_id == querySearch.id & a.cost_id == a.cost_tbl.id
                             select new
                             {
                                 id_insurance = a.cost_tbl.insurance_id,
                                 id_package = a.cost_tbl.package_id,
                                 price = a.cost_tbl.price,
                                 id_cost = a.cost_id
                             }).ToList();

            var queryCost2 = (from a in db.appointment_cost_tbls
                              where a.appointment_id == querySearch.id & a.cost_id == null
                              select new
                              {
                                  itc = a.itc_present,
                                  bv = a.bv_present,
                                  sinhmang = a.sinhmang,
                                  tainan = a.tainan,
                                  upbv = a.upbv_present,
                                  ash = a.ash_present
                              }).FirstOrDefault();

            foreach (var i in queryCost)
            {
                if (i.id_package == 1)
                {
                    cbbInsurence.SelectedValue = i.id_insurance;
                    nudPackage.Value = i.price;
                    listCost["core"] = i.id_cost == null ? 0 : (int)i.id_cost;
                }
                if (i.id_package == 2)
                {
                    chbNgoaitru.Checked = true;
                    nudNgoaitru.Value = i.price;
                    listCost["out"] = i.id_cost == null ? 0 : (int)i.id_cost;
                }
                if (i.id_package == 5)
                {
                    chbNha.Checked = true;
                    nudNha.Value = i.price;
                    listCost["dental"] = i.id_cost == null ? 0 : (int)i.id_cost;
                }
                if (i.id_package == 6)
                {
                    chbThaiSan.Checked = true;
                    nudThaisan.Value = i.price;
                    listCost["maternity"] = i.id_cost == null ? 0 : (int)i.id_cost;
                }
            }
            if (queryCost2 != null)
            {
                nudITC.Value = queryCost2.itc == null ? 0 : (decimal)queryCost2.itc;
                nudBv.Value = queryCost2.bv == null ? 0 : (decimal)queryCost2.bv;
                nudtangphibv.Value = queryCost2.upbv == null ? 0 : (decimal)queryCost2.upbv;
                nudASH.Value = queryCost2.ash == null ? 0 : (decimal)queryCost2.ash;

                nudSinhmang.Value = queryCost2.sinhmang == null ? 0 : (decimal)queryCost2.sinhmang;
                nudTainan.Value = queryCost2.tainan == null ? 0 : (decimal)queryCost2.tainan;

                nudSinhmangInput.Value = (decimal)(decimal.ToDouble(nudSinhmang.Value) * 100 / 0.2);
                nudTainanInput.Value = (decimal)(decimal.ToDouble(nudTainan.Value) * 100 / 0.09);
            }

            var queryQc = (from a in db.qaqc_appointments
                           where a.appointment_id == querySearch.id
                           select a).FirstOrDefault();

            if (queryQc != null)
            {
                txtngaygoi.Text = queryQc.call_date;
                cbbthoigiancho.SelectedItem = queryQc.waiting_time;
                nudfcp.Value = (decimal)queryQc.fcp;
                txtlead.Text = queryQc.lead;
                cbb13.SelectedItem = queryQc.note13;
                cbbcovid.SelectedItem = queryQc.covid;
                cbbupsell.SelectedItem = queryQc.upsell;
                cbbupgrade.SelectedItem = queryQc.upgrade;
                cbbgiadinh.SelectedItem = queryQc.family;
                cbbkhonghoanphi.SelectedItem = queryQc.no_reponse;
                txtNoteQAQC.Text = queryQc.note;
            }
            checkSum();
        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            refreshControl();
            string code = createAppointment();
            txtCode.Text = code;
            cbbStatus.SelectedValue = 1;

            tabPage1.Enabled = true;
            tabPage3.Enabled = true;

            btnUpdate.Enabled = true;

            setRoleForUser();
        }
        private string createAppointment()
        {
            var temp = DateTime.Now.ToString("yyyyMMddhhmmss");
            return "ASH" + temp;
        }
        private void cbbInsurence_SelectionChangeCommitted(object sender, EventArgs e)
        {
            clearControl();
            if (cbbInsurence.SelectedValue.ToString() == "0")
            {
                nudPackage.Value = 0;
                listCost["core"] = 0;
                checkSum();
                return;
            }

            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            var rankAge = CheckRangAge(temp, 0);
            int Insurence = (int)cbbInsurence.SelectedValue;

            var queryPrice = (from a in db.cost_tbls
                              where a.package_id == 1 && a.insurance_id == Insurence && a.age_id == rankAge
                              select new { a.id, a.price }).FirstOrDefault();

            if (queryPrice != null)
            {
                nudPackage.Value = queryPrice.price;
                listCost["core"] = queryPrice.id;
            }
            checkSum();
        }
        private void checkSum()
        {
            var sumMoney = nudPackage.Value + nudNgoaitru.Value + nudNha.Value + nudThaisan.Value + nudSinhmang.Value + nudTainan.Value;
            var temp = decimal.ToDouble(nudITC.Value) + decimal.ToDouble(nudBv.Value) + decimal.ToDouble(nudASH.Value);

            double tongtien = decimal.ToDouble(sumMoney);
            double result = 0;

            if (decimal.ToDouble(nudtangphibv.Value) > 0)
            {
                result = tongtien * decimal.ToDouble(nudtangphibv.Value) / 100 + tongtien;
                tongtien = result;
            }
            if (temp > 0)
            {
                result = tongtien - (tongtien * temp / 100);
            }
            if (temp == 0 && decimal.ToDouble(nudtangphibv.Value) == 0)
            {
                result = tongtien;
            }
            lbltongphitruocgiam.Text = String.Format("{0:0,0}", sumMoney);
            lblTongChiPhi.Text = String.Format("{0:0,0}", result);
        }
        private void chbNgoaitru_CheckedChanged(object sender, EventArgs e)
        {
            if (cbbInsurence.SelectedValue == null || cbbInsurence.SelectedValue.ToString() == "0")
            {
                nudPackage.Value = 0;
                listCost["core"] = 0;
                checkSum();
                return;
            }
            if (!chbNgoaitru.Checked)
            {
                nudNgoaitru.Value = 0;
                listCost["out"] = 0;
                checkSum();
                return;
            }

            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            var rankAge = CheckRangAge(temp, 0);
            int Insurence = (int)cbbInsurence.SelectedValue;

            var queryPrice = (from a in db.cost_tbls
                              where a.package_id == 2 && a.insurance_id == Insurence && a.age_id == rankAge
                              select new { a.id, a.price }).FirstOrDefault();

            if (queryPrice != null)
            {
                nudNgoaitru.Value = queryPrice.price;
                listCost["out"] = queryPrice.id;
            }
            checkSum();
        }
        private void chbNha_CheckedChanged(object sender, EventArgs e)
        {
            if (cbbInsurence.SelectedValue == null || cbbInsurence.SelectedValue.ToString() == "0")
            {
                nudPackage.Value = 0;
                listCost["core"] = 0;
                checkSum();
                return;
            }
            if (!chbNha.Checked)
            {
                nudNha.Value = 0;
                listCost["dental"] = 0;
                checkSum();
                return;
            }

            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            var rankAge = CheckRangAge(temp, 5);
            int Insurence = (int)cbbInsurence.SelectedValue;

            var queryPrice = (from a in db.cost_tbls
                              where a.package_id == 5 && a.insurance_id == Insurence && a.age_id == rankAge
                              select new { a.id, a.price }).FirstOrDefault();

            if (queryPrice != null)
            {
                nudNha.Value = queryPrice.price;
                listCost["dental"] = queryPrice.id;
            }

            checkSum();
        }
        private void chbThaiSan_CheckedChanged(object sender, EventArgs e)
        {
            if (cbbInsurence.SelectedValue == null || cbbInsurence.SelectedValue.ToString() == "0")
            {
                nudPackage.Value = 0;
                listCost["core"] = 0;
                checkSum();
                return;
            }
            if (!chbThaiSan.Checked)
            {
                nudThaisan.Value = 0;
                listCost["maternity"] = 0;
                checkSum();
                return;
            }

            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            var rankAge = CheckRangAge(temp, 6);
            int Insurence = (int)cbbInsurence.SelectedValue;

            var queryPrice = (from a in db.cost_tbls
                              where a.package_id == 6 && a.insurance_id == Insurence && a.age_id == rankAge
                              select new { a.id, a.price }).FirstOrDefault();

            if (queryPrice != null)
            {
                nudThaisan.Value = queryPrice.price;
                listCost["maternity"] = queryPrice.id;
            }
            checkSum();
        }
        private int CheckRangAge(string birthday, int package)
        {
            int rankAge = 0;
            try
            {

                int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                int dob = int.Parse(birthday);
                var age = (now - dob) / 10000;

                foreach (var a in listRank)
                {
                    var temp = a.Value;
                    if (temp[0] <= age && age <= temp[1] && package == temp[2])
                    {
                        rankAge = a.Key;
                    }
                }
                return rankAge;
            }
            catch
            {
                return rankAge;
            }
        }
        private void clearSum()
        {
            nudPackage.Value = 0;
            nudNgoaitru.Value = 0;
            nudNha.Value = 0;
            nudThaisan.Value = 0;
            nudSinhmang.Value = 0;
            nudTainan.Value = 0;
            nudITC.Value = 0;
            nudBv.Value = 0;
            nudSinhmangInput.Value = 0;
            nudTainanInput.Value = 0;
            nudtangphibv.Value = 0;
            nudASH.Value = 0;

            cbbInsurence.SelectedValue = "0";
            chbNgoaitru.Checked = false;
            chbNha.Checked = false;
            chbThaiSan.Checked = false;

            listCost["core"] = 0;
            listCost["out"] = 0;
            listCost["dental"] = 0;
            listCost["maternity"] = 0;
        }
        private void clearControl()
        {
            nudPackage.Value = 0;
            nudNgoaitru.Value = 0;
            nudNha.Value = 0;
            nudThaisan.Value = 0;
            nudSinhmang.Value = 0;
            nudTainan.Value = 0;
            nudITC.Value = 0;
            nudBv.Value = 0;
            nudSinhmangInput.Value = 0;
            nudTainanInput.Value = 0;
            nudtangphibv.Value = 0;
            nudASH.Value = 0;

            chbNgoaitru.Checked = false;
            chbNha.Checked = false;
            chbThaiSan.Checked = false;

            listCost["core"] = 0;
            listCost["out"] = 0;
            listCost["dental"] = 0;
            listCost["maternity"] = 0;
        }
        private void nudSinhmangInput_ValueChanged(object sender, EventArgs e)
        {
            var temp = decimal.ToDouble(nudSinhmangInput.Value) * 0.2 / 100;
            nudSinhmang.Value = (decimal)temp;
            checkSum();
        }
        private void nudTainanInput_ValueChanged(object sender, EventArgs e)
        {
            var temp = decimal.ToDouble(nudTainanInput.Value) * 0.09 / 100;
            nudTainan.Value = (decimal)temp;
            checkSum();
        }
        private void nudITC_ValueChanged(object sender, EventArgs e)
        {
            checkSum();
        }
        private void nudBv_ValueChanged(object sender, EventArgs e)
        {
            checkSum();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(lblTongChiPhi.Text) || lblTongChiPhi.Text.Trim() == "0")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }
            if (cbbStatus.SelectedValue == null)
            {
                MessageBox.Show("Chọn trạng thái phiếu.");
                return;
            }
            if (cbbInsurence.SelectedValue == null)
            {
                MessageBox.Show("Chọn gói bảo hiểm.");
                return;
            }

            var tb = checkCreatOrUpdate(txtCode.Text.Trim());

            if (tb.id == 0)
            {
                InsertAppointment();
            }
            else
            {
                UpdateAppointment(tb);
            }
        }
        private void InsertAppointment()
        {
            try
            {
                appointment_tbl ap = new appointment_tbl()
                {
                    code = txtCode.Text.Trim(),
                    tsr_name = txtTSR.Text.Trim(),
                    appointment_date = dtpDate_info.Value.ToString("yyyy-MM-dd"),
                    appointment_time = cbOnline.Checked == true ? "ONL" : dtpTime_info.Value.ToString("HH:mm"),
                    ap_customer = txtCustomerName_info.Text.Trim().ToUpper(),
                    ap_phone = txtPhone_info.Text.Trim(),
                    ap_address = txtAddress_info.Text.Trim(),
                    rq_customer = txtRq_customer.Text.Trim().ToUpper(),
                    rq_cccd = txtRq_cccd.Text.Trim(),
                    rq_sex = cbbRq_sex.SelectedItem == null ? "" : cbbRq_sex.SelectedItem.ToString(),
                    rq_birthday = dtpBirthday.Value.ToString("yyyy-MM-dd"),
                    rq_phone = txtRq_sdt.Text.Trim(),
                    rq_address = txtRq_address.Text.Trim(),
                    in_customer = txtIn_ndbh.Text.Trim().ToUpper(),
                    in_cccd = txtIn_sdt.Text.Trim(),
                    in_sex = cbbIn_sex.SelectedItem == null ? "" : cbbIn_sex.SelectedItem.ToString(),
                    in_birthday = dtpIn_birthday.Value.ToString("yyyy-MM-dd"),
                    in_relationship = cbbIn_relationship.SelectedItem == null ? "" : cbbIn_relationship.SelectedItem.ToString(),
                    be_customer = txtBe_nth.Text.Trim(),
                    be_sex = cbbBe_sex.SelectedItem == null ? "" : cbbBe_sex.SelectedItem.ToString(),
                    be_birthday = txtBe_brithday.Text.Trim(),
                    be_id = txtBe_id.Text.Trim(),
                    be_relationship = cbbBe_relationship.SelectedItem == null ? "" : cbbBe_relationship.SelectedItem.ToString(),
                    note = txt_note.Text.Trim(),
                    team_name = lblteam.Text.Trim(),
                    created_user = userId,
                    created_date = DateTime.Now,
                    status_ap = (int)cbbStatus.SelectedValue,
                    gr_cccd_sdt = txtGr_cccd_sdt.Text.Trim(),
                    gr_sl = (int)nudGr_sl.Value,
                    gr_note = txtGr_note.Text.Trim(),
                    cccd_status = cbbCCCD.SelectedItem == null ? "" : cbbCCCD.SelectedItem.ToString(),
                };
                db.appointment_tbls.InsertOnSubmit(ap);
                db.SubmitChanges();

                if (listCost["core"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = ap.id,
                        cost_id = listCost["core"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["out"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = ap.id,
                        cost_id = listCost["out"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["dental"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = ap.id,
                        cost_id = listCost["dental"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["maternity"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = ap.id,
                        cost_id = listCost["maternity"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                appointment_cost_tbl appCost2 = new appointment_cost_tbl()
                {
                    appointment_id = ap.id,
                    sinhmang = (int)nudSinhmang.Value,
                    tainan = (int)nudTainan.Value,
                    itc_present = (int)nudITC.Value,
                    bv_present = (int)nudBv.Value,
                    upbv_present = (int)nudtangphibv.Value,
                    ash_present = (int)nudASH.Value
                };
                db.appointment_cost_tbls.InsertOnSubmit(appCost2);

                if (role == 1 || role == 3 || role == 4)
                {
                    qaqc_appointment qaqc = new qaqc_appointment()
                    {
                        appointment_id = ap.id,
                        call_date = txtngaygoi.Text.Trim(),
                        waiting_time = cbbthoigiancho.SelectedItem == null ? "" : cbbthoigiancho.SelectedItem.ToString(),
                        fcp = (int)nudfcp.Value,
                        lead = txtlead.Text.Trim(),
                        note13 = cbb13.SelectedItem == null ? "" : cbb13.SelectedItem.ToString(),
                        covid = cbbcovid.SelectedItem == null ? "" : cbbcovid.SelectedItem.ToString(),
                        upsell = cbbupsell.SelectedItem == null ? "" : cbbupsell.SelectedItem.ToString(),
                        upgrade = cbbupgrade.SelectedItem == null ? "" : cbbupgrade.SelectedItem.ToString(),
                        family = cbbgiadinh.SelectedItem == null ? "" : cbbgiadinh.SelectedItem.ToString(),
                        no_reponse = cbbkhonghoanphi.SelectedItem == null ? "" : cbbkhonghoanphi.SelectedItem.ToString(),
                        note = txtNoteQAQC.Text.Trim(),
                        created_user = userId,
                        created_date = DateTime.Now
                    };
                    db.qaqc_appointments.InsertOnSubmit(qaqc);
                }

                db.SubmitChanges();
                MessageBox.Show("Tạo phiếu thành công.");
                refreshControl();
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private void UpdateAppointment(appointment_tbl tb)
        {
            try
            {
                tb.tsr_name = txtTSR.Text.Trim();
                tb.appointment_date = dtpDate_info.Value.ToString("yyyy-MM-dd");
                tb.appointment_time = cbOnline.Checked == true ? "ONL" : dtpTime_info.Value.ToString("HH:mm");
                tb.ap_customer = txtCustomerName_info.Text.Trim().ToUpper();
                tb.ap_phone = txtPhone_info.Text.Trim();
                tb.ap_address = txtAddress_info.Text.Trim();
                tb.rq_customer = txtRq_customer.Text.Trim().ToUpper();
                tb.rq_cccd = txtRq_cccd.Text.Trim();
                tb.rq_sex = cbbRq_sex.SelectedItem == null ? "" : cbbRq_sex.SelectedItem.ToString();
                tb.rq_birthday = dtpBirthday.Value.ToString("yyyy-MM-dd");
                tb.rq_phone = txtRq_sdt.Text.Trim();
                tb.rq_address = txtRq_address.Text.Trim();
                tb.in_customer = txtIn_ndbh.Text.Trim().ToUpper();
                tb.in_cccd = txtIn_sdt.Text.Trim();
                tb.in_sex = cbbIn_sex.SelectedItem == null ? "" : cbbIn_sex.SelectedItem.ToString();
                tb.in_birthday = dtpIn_birthday.Value.ToString("yyyy-MM-dd");
                tb.in_relationship = cbbIn_relationship.SelectedItem == null ? "" : cbbIn_relationship.SelectedItem.ToString();
                tb.be_customer = txtBe_nth.Text.Trim();
                tb.be_sex = cbbBe_sex.SelectedItem == null ? "" : cbbBe_sex.SelectedItem.ToString();
                tb.be_birthday = txtBe_brithday.Text.Trim();
                tb.be_id = txtBe_id.Text.Trim();
                tb.be_relationship = cbbBe_relationship.SelectedItem == null ? "" : cbbBe_relationship.SelectedItem.ToString();
                tb.note = txt_note.Text.Trim();
                tb.team_name = lblteam.Text.Trim();
                tb.updated_user = userId;
                tb.updated_date = DateTime.Now;
                tb.status_ap = (int)cbbStatus.SelectedValue;
                tb.gr_cccd_sdt = txtGr_cccd_sdt.Text.Trim();
                tb.gr_sl = (int)nudGr_sl.Value;
                tb.gr_note = txtGr_note.Text.Trim();
                tb.cccd_status = cbbCCCD.SelectedItem == null ? "" : cbbCCCD.SelectedItem.ToString();

                var querydelete = (from a in db.appointment_cost_tbls
                                   where a.appointment_id == tb.id
                                   select a).ToList();
                if (querydelete.Any())
                {
                    db.appointment_cost_tbls.DeleteAllOnSubmit(querydelete);
                    //db.SubmitChanges();
                }

                if (listCost["core"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = tb.id,
                        cost_id = listCost["core"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["out"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = tb.id,
                        cost_id = listCost["out"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["dental"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = tb.id,
                        cost_id = listCost["dental"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                if (listCost["maternity"] > 0)
                {
                    appointment_cost_tbl appCost = new appointment_cost_tbl()
                    {
                        appointment_id = tb.id,
                        cost_id = listCost["maternity"]
                    };
                    db.appointment_cost_tbls.InsertOnSubmit(appCost);
                }
                appointment_cost_tbl appCost2 = new appointment_cost_tbl()
                {
                    appointment_id = tb.id,
                    sinhmang = (int)nudSinhmang.Value,
                    tainan = (int)nudTainan.Value,
                    itc_present = (int)nudITC.Value,
                    bv_present = (int)nudBv.Value,
                    upbv_present = (int)nudtangphibv.Value,
                    ash_present = (int)nudASH.Value
                };
                db.appointment_cost_tbls.InsertOnSubmit(appCost2);


                var queryQc = (from a in db.qaqc_appointments
                               where a.appointment_id == tb.id
                               select a).FirstOrDefault();
                if (queryQc != null)
                {
                    queryQc.call_date = txtngaygoi.Text.Trim();
                    queryQc.waiting_time = cbbthoigiancho.SelectedItem == null ? "" : cbbthoigiancho.SelectedItem.ToString();
                    queryQc.fcp = (int)nudfcp.Value;
                    queryQc.lead = txtlead.Text.Trim();
                    queryQc.note13 = cbb13.SelectedItem == null ? "" : cbb13.SelectedItem.ToString();
                    queryQc.covid = cbbcovid.SelectedItem == null ? "" : cbbcovid.SelectedItem.ToString();
                    queryQc.upsell = cbbupsell.SelectedItem == null ? "" : cbbupsell.SelectedItem.ToString();
                    queryQc.upgrade = cbbupgrade.SelectedItem == null ? "" : cbbupgrade.SelectedItem.ToString();
                    queryQc.family = cbbgiadinh.SelectedItem == null ? "" : cbbgiadinh.SelectedItem.ToString();
                    queryQc.no_reponse = cbbkhonghoanphi.SelectedItem == null ? "" : cbbkhonghoanphi.SelectedItem.ToString();
                    queryQc.note = txtNoteQAQC.Text.Trim();
                    queryQc.updated_user = userId;
                    queryQc.updated_date = DateTime.Now;
                }
                else
                {
                    qaqc_appointment qaqc = new qaqc_appointment()
                    {
                        appointment_id = tb.id,
                        call_date = txtngaygoi.Text.Trim(),
                        waiting_time = cbbthoigiancho.SelectedItem == null ? "" : cbbthoigiancho.SelectedItem.ToString(),
                        fcp = (int)nudfcp.Value,
                        lead = txtlead.Text.Trim(),
                        note13 = cbb13.SelectedItem == null ? "" : cbb13.SelectedItem.ToString(),
                        covid = cbbcovid.SelectedItem == null ? "" : cbbcovid.SelectedItem.ToString(),
                        upsell = cbbupsell.SelectedItem == null ? "" : cbbupsell.SelectedItem.ToString(),
                        upgrade = cbbupgrade.SelectedItem == null ? "" : cbbupgrade.SelectedItem.ToString(),
                        family = cbbgiadinh.SelectedItem == null ? "" : cbbgiadinh.SelectedItem.ToString(),
                        no_reponse = cbbkhonghoanphi.SelectedItem == null ? "" : cbbkhonghoanphi.SelectedItem.ToString(),
                        note = txtNoteQAQC.Text.Trim(),
                        created_user = userId,
                        created_date = DateTime.Now
                    };

                    db.qaqc_appointments.InsertOnSubmit(qaqc);
                }
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
                MessageBox.Show("Cập nhật phiếu thành công.");
                refreshControl();

            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                MessageBox.Show("Cập nhật phiếu thành công.");
                refreshControl();
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private void refreshControl()
        {
            tabPage1.Enabled = false;
            tabPage3.Enabled = false;
            btnUpdate.Enabled = false;
            groupInfoPackage.Enabled = false;

            clearControl();
            txtPhone.Text = "";
            txtCode.Text = "";
            txtTSR.Text = "";
            txtCustomerName_info.Text = "";
            txtPhone_info.Text = "";
            txtAddress_info.Text = "";
            txtRq_customer.Text = "";
            txtRq_cccd.Text = "";
            cbbRq_sex.SelectedItem = null;
            cbbCCCD.SelectedItem = null;
            txtRq_sdt.Text = "";
            txtRq_address.Text = "";
            txtIn_ndbh.Text = "";
            txtIn_sdt.Text = "";
            cbbIn_sex.SelectedItem = null;
            cbbIn_relationship.SelectedItem = null;
            txtBe_nth.Text = "";
            cbbBe_sex.SelectedItem = null;
            txtBe_brithday.Text = "";
            txtBe_id.Text = "";
            cbbBe_relationship.SelectedItem = null;
            txt_note.Text = "";
            //lblteam.Text = "";
            cbbStatus.SelectedValue = 0;
            txtGr_cccd_sdt.Text = "";
            nudGr_sl.Value = 0;
            txtGr_note.Text = "";

            txtngaygoi.Text = "";
            cbbthoigiancho.SelectedItem = null;
            nudfcp.Value = 1;
            txtlead.Text = "";
            cbb13.SelectedItem = null;
            cbbcovid.SelectedItem = null;
            cbbupsell.SelectedItem = null;
            cbbupgrade.SelectedItem = null;
            cbbgiadinh.SelectedItem = null;
            cbbkhonghoanphi.SelectedItem = null;
            txtNoteQAQC.Text = "";

            DataSearch.Clear();
        }
        private appointment_tbl checkCreatOrUpdate(string code)
        {
            appointment_tbl tb = new appointment_tbl();
            var queryCheck = (from a in db.appointment_tbls
                              where a.code == code.Trim()
                              select a).FirstOrDefault();
            if (queryCheck != null) { return queryCheck; };

            return tb;
        }
        private void dtpIn_birthday_ValueChanged(object sender, EventArgs e)
        {
            var temp = dtpIn_birthday.Value.ToString("yyyyMMdd");
            var age = (int.Parse(DateTime.Now.ToString("yyyyMMdd")) - int.Parse(temp)) / 10000;
            lblAge.Text = age.ToString();
            clearSum();

            if (age > 0)
            {
                groupInfoPackage.Enabled = true;
            }
        }
        private int GetRoleByID(int userId)
        {
            var queryRole = (from a in db.user_role_tbls
                             where a.user_id == userId
                             select a).FirstOrDefault();
            if (queryRole != null)
            {
                return queryRole.role_id;
            }
            return 0;
        }
        private void setRoleForUser()
        {
            //tabAdmin.Enabled = false;
            //tabReport.Enabled = false;
            //tabQA.Enabled = false;
            //tabCCCD.Enabled = false;

            switch (role)
            {
                case 1: // admin
                    // code
                    break;
                case 2: // Team lead
                    tabCCCD.Enabled = false;
                    tabAdmin.Enabled = false;
                    tabReport.Enabled = false;
                    tabQA.Enabled = false;
                    tabPage3.Enabled = false;
                    break;
                case 3: // QA/QC
                    tabCCCD.Enabled = false;
                    tabAdmin.Enabled = false;
                    tabReport.Enabled = false;
                    tabPage1.Enabled = false;
                    groupInfoPackage.Enabled = false;
                    btnCreate.Enabled = false;

                    btnXoaPhieu.Visible = true;
                    break;
                case 4: //RN
                    tabAdmin.Enabled = false;
                    tabReport.Enabled = false;
                    tabQA.Enabled = false;
                    tabCCCD.Enabled = false;
                    break;
            }
        }
        private void GetDataListApp(int pageNumber)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["itcom21ConnectionString"].ConnectionString;
            var connection = new SqlConnection(connStr);
            try
            {
                connection.Open();
                string sql = "1=1";
                if (role != 1 && role != 3)
                {
                    sql = "a.created_user = @user";

                }
                if (string.IsNullOrEmpty(txtSearchList.Text.Trim()) == false)
                {
                    sql += " and (a.ap_customer=N'" + txtSearchList.Text.Trim() + "' or a.in_cccd='" + txtSearchList.Text.Trim() + "') ";
                }

                string queryString = @"select * from (select a.id,a.team_name as 'Team', s.name as 'Trạng thái', a.code as 'Mã phiếu', a.tsr_name as 'Tên TSR/LINE', a.appointment_date as 'Ngày hẹn',
case when a.appointment_time = 'ONL' then 'ONLINE' else a.appointment_time end as 'Giờ hẹn', a.ap_customer as 'Tên KH', a.ap_phone as 'SĐT 1',
a.ap_address as 'Địa chỉ 1', a.gr_cccd_sdt as 'group', a.rq_customer as 'Người yêu cầu', a.rq_cccd as 'CMND/CCCD/HC 1', a.rq_sex as 'Giới tính 1', a.rq_birthday as 'Ngày sinh 1',
a.rq_phone as 'SĐT 2', a.rq_address as 'Địa chỉ 2', a.in_customer as 'Người được BH',a.in_cccd as 'CMND/CCCD/HC 2', a.in_sex as 'Giới tính 2', a.in_birthday as 'Ngày sinh 2',
a.in_relationship as 'Quan hệ với NYCBH 1', a.be_customer as 'Người thụ hưởng', a.be_sex as 'Giới tính be', a.be_birthday as 'Ngày sinh', a.be_id as 'ID 2',
a.be_relationship as 'QUan hệ với NYCBH 2', a.note as 'Thông tin bệnh/ghi chú', 
q.lead as 'Tên Lead', q.call_date as 'Ngày gọi', q.fcp as 'FCP', q.note13 as 'Nghị định 13', q.upgrade as 'Upgrade',
q.upsell as 'Upsell', q.waiting_time as 'Thời gian chờ', q.covid as 'Covid', q.no_reponse as 'Không hoàn phí', q.note as 'QA note'
from appointment_tbl a
inner join status_appointment_tbl s on a.status_ap= s.Id
left join qaqc_appointment q on a.id=q.appointment_id
inner join user_tbl u on a.created_user=u.id
where " + sql + @") as tb111
inner join (select 
	bbb.id as appointment_id,sum([Gói Chính]) as [Gói Chính],sum([Ngoại trú]) as [Ngoại trú], 
	sum([tainan]) as [Tai nạn],sum([sinhmang]) as [Sinh mạng],sum([Nha]) as [Nha],sum([Thai sản]) as [Thai sản], 
	sum(itc_present) as [Giảm phí ITC], sum(bv_present) as [Giảm phí BV], sum(upbv_present) as [Tăng phí BV] from 
	( select id,[Gói Chính],[Ngoại trú],'' as tainan, '' as sinhmang,[Nha],[Thai sản], '' as itc_present ,'' as bv_present,'' as upbv_present  
	from 
(select a.id,c.package_id,p.name from appointment_tbl a 
inner join appointment_cost_tbl ac on a.id=ac.appointment_id
left join cost_tbl c on ac.cost_id=c.id
left join  insurance_tbl i on i.id=c.Insurance_id
left join package_tbl p on p.id=c.package_id) tba
pivot(
	count(tba.package_id)
	for tba.name in ([Gói Chính],[Ngoại trú],[Nha],[Thai sản])
) AS BangChuyen
UNION ALL
select a.id,'' as [Gói Chính],'' as [Ngoại trú],ac.tainan, ac.sinhmang ,'' as [Nha],'' as [Thai sản], ac.itc_present, ac.bv_present, ac.upbv_present from appointment_tbl a 
inner join appointment_cost_tbl ac on a.id=ac.appointment_id
where ac.itc_present is not null and ac.bv_present is not null) as bbb
group by id ) as tb222 on tb111.id=tb222.appointment_id order by id desc";

                var command = new SqlCommand(queryString, connection);

                if (role != 1 && role != 3)
                {
                    command.Parameters.AddWithValue("@user", userId);
                }
                var reader = command.ExecuteReader();
                DataSearch.Clear();
                if (reader.HasRows)
                {
                    DataSearch.Load(reader);
                }
                reader.Close();

                var temp = DataSearch.AsEnumerable();
                var result = new DataTable();

                int pageSize = 30;

                if (temp.Any())
                {
                    float aaa = temp.Count();
                    tempPage = Math.Ceiling(aaa / pageSize);
                    lbltotalRow.Text = pageNumber + "/" + tempPage;
                    var data1 = (from b in temp
                                 select b).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                    result = data1.CopyToDataTable<DataRow>();
                }

                dataGridView1.DataSource = result.AsDataView();
                dataGridView1.PerformLayout();
                if (result.Rows.Count > 0)
                {
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["appointment_id"].Visible = false;

                    dataGridView1.Columns[0].Frozen = true;
                    dataGridView1.Columns[1].Frozen = true;
                    dataGridView1.Columns[2].Frozen = true;
                    dataGridView1.Columns[3].Frozen = true;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Kiểm tra lại kết nối.");
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xả ra");
            }
            finally
            {
                connection.Close();
            }
        }
        private void GetDataListAppPagzing(int pageNumber)
        {
            try
            {
                dataGridView1.DataSource = null;
                var temp = DataSearch.AsEnumerable();
                var result = new DataTable();

                int pageSize = 30;

                if (temp.Any())
                {
                    float aaa = temp.Count();
                    tempPage = Math.Ceiling(aaa / pageSize);
                    lbltotalRow.Text = pageNumber + "/" + tempPage;
                    var data1 = (from b in temp
                                 select b).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                    result = data1.CopyToDataTable<DataRow>();
                }

                dataGridView1.DataSource = result.AsDataView();

                if (result.Rows.Count > 0)
                {
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["appointment_id"].Visible = false;

                    dataGridView1.Columns[0].Frozen = true;
                    dataGridView1.Columns[1].Frozen = true;
                    dataGridView1.Columns[2].Frozen = true;
                    dataGridView1.Columns[3].Frozen = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
            }
        }
        private void btn_getdata_Click(object sender, EventArgs e)
        {
            pageNumber = 1;
            GetDataListApp(pageNumber);
        }
        private void nudtangphibv_ValueChanged(object sender, EventArgs e)
        {
            checkSum();
        }
        private void nudASH_ValueChanged(object sender, EventArgs e)
        {
            checkSum();
        }
        private void nudSinhmangInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                SendKeys.Send("{ENTER}");
                nudSinhmangInput.Select();
                SendKeys.Send("{END}");
            }
        }
        private void nudTainanInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                SendKeys.Send("{ENTER}");
                nudTainanInput.Select();
                SendKeys.Send("{END}");
            }
        }
        private void DataGridViewStyle()
        {
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.WhiteSmoke;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("MS Refence Sans Serif", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (pageNumber > 1)
            {
                pageNumber -= 1;
                if (pageNumber <= tempPage)
                {
                    GetDataListAppPagzing(pageNumber);
                }
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (pageNumber <= tempPage)
            {
                pageNumber += 1;
                if (pageNumber <= tempPage)
                {
                    GetDataListAppPagzing(pageNumber);
                }
            }

        }
        private void ImportData_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Tab Appli - QA
        private void btnUpdateAppliQA_Click(object sender, EventArgs e)
        {
            var flag = checkDataInputQA();
            if (!flag) { MessageBox.Show("Input format error"); return; }
            var appQATable = GetItemTable(IDAPPLIQA);

            if (appQATable == null)
            {
                InsertAppliQA();
            }
            else
            {
                UpdateAppliQA(appQATable);
            }
            btnGetData_QA_Click(sender, e);
        }
        private void UpdateAppliQA(application_qa_tbl qa_Tbl)
        {
            try
            {
                qa_Tbl.policy_type = cbbPolicyType.SelectedItem == null ? "" : cbbPolicyType.SelectedItem.ToString();
                qa_Tbl.renewal_type = cbbRenewalType.SelectedItem == null ? "" : cbbRenewalType.SelectedItem.ToString();
                qa_Tbl.firstpolicy_year = txtFirstPolicyYear.Text.Trim();
                qa_Tbl.pure_introduce = cbbPure.SelectedItem == null ? "" : cbbPure.SelectedItem.ToString();
                qa_Tbl.lead_sourceQA = txtSourceQA.Text.Trim();
                qa_Tbl.lead_source = cbbLeadSource.SelectedItem == null ? "" : cbbLeadSource.SelectedItem.ToString();
                qa_Tbl.upsell_plan = cbbUpsellPlan.SelectedItem == null ? "" : cbbUpsellPlan.SelectedItem.ToString();
                qa_Tbl.rider_plan = cbbRiderUpsell.SelectedItem == null ? "" : cbbRiderUpsell.SelectedItem.ToString();
                qa_Tbl.noofcall = (int)nudNoofcall.Value;
                qa_Tbl.from_list = txtFromList.Text.Trim();
                qa_Tbl.updated_user = userId;
                qa_Tbl.updated_date = DateTime.Now;

                db.SubmitChanges();
                refreshTabAppliQA();
                MessageBox.Show("success");
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private void InsertAppliQA()
        {
            try
            {
                var appointment_id = GetIdByAppointmentCode(CODE);

                var qa_tbl = new application_qa_tbl()
                {
                    appointment_id = appointment_id,
                    policy_type = cbbPolicyType.SelectedItem == null ? "" : cbbPolicyType.SelectedItem.ToString(),
                    renewal_type = cbbRenewalType.SelectedItem == null ? "" : cbbRenewalType.SelectedItem.ToString(),
                    firstpolicy_year = txtFirstPolicyYear.Text.Trim(),
                    pure_introduce = cbbPure.SelectedItem == null ? "" : cbbPure.SelectedItem.ToString(),
                    lead_sourceQA = txtSourceQA.Text.Trim(),
                    lead_source = cbbLeadSource.SelectedItem == null ? "" : cbbLeadSource.SelectedItem.ToString(),
                    upsell_plan = cbbUpsellPlan.SelectedItem == null ? "" : cbbUpsellPlan.SelectedItem.ToString(),
                    rider_plan = cbbRiderUpsell.SelectedItem == null ? "" : cbbRiderUpsell.SelectedItem.ToString(),
                    noofcall = (int)nudNoofcall.Value,
                    from_list = txtFromList.Text.Trim(),
                    created_user = userId,
                    created_date = DateTime.Now
                };

                db.application_qa_tbls.InsertOnSubmit(qa_tbl);
                db.SubmitChanges();
                refreshTabAppliQA();
                MessageBox.Show("success");
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private bool checkDataInputQA()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSourceQA.Text))
                {
                    DateTime parsed;
                    string leadsourceQA = txtSourceQA.Text.Trim();
                    bool valid_leadsourceQA = DateTime.TryParseExact(leadsourceQA, "MM/yyyy",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out parsed);
                    if (!valid_leadsourceQA)
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(txtFirstPolicyYear.Text))
                {
                    DateTime parsed;
                    string firstPolicyYear = txtFirstPolicyYear.Text.Trim();
                    bool valid_firstPolicyYear = DateTime.TryParseExact(firstPolicyYear, "yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out parsed);
                    if (!valid_firstPolicyYear)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private application_qa_tbl GetItemTable(int id)
        {
            try
            {
                application_qa_tbl appQA = new application_qa_tbl();

                appQA = (from a in db.application_qa_tbls
                         where a.id == id
                         select a).FirstOrDefault();
                return appQA;
            }
            catch
            {
                return new application_qa_tbl();
            }
        }
        private int GetIdByAppointmentCode(string appointment_code)
        {
            int resulft = 0;
            resulft = (from a in db.appointment_tbls
                       where a.code == appointment_code
                       select a.id).FirstOrDefault();
            return resulft;
        }
        private void btnGetData_QA_Click(object sender, EventArgs e)
        {
            var dataQA = (from a in db.application_qa_tbls
                          select new AppliQA
                          {
                              id = a.id,
                              appointment_code = a.appointment_tbl.code,
                              policies_type = a.policy_type,
                              renewal_type = a.renewal_type,
                              first_policy_year = a.firstpolicy_year,
                              pure = a.pure_introduce,
                              lead_sourceQA = a.lead_sourceQA,
                              lead_source = a.lead_source,
                              Upsell_plan = a.upsell_plan,
                              rider_upsell = a.rider_plan,
                              noofcall = (int)a.noofcall,
                              from_list = a.from_list
                          }).ToList();
            appliQABindingSource1.DataSource = dataQA;
        }
        private void refreshTabAppliQA()
        {
            cbbPolicyType.SelectedItem = null;
            cbbRenewalType.SelectedItem = null;
            cbbPure.SelectedItem = null;
            cbbUpsellPlan.SelectedItem = null;
            cbbRiderUpsell.SelectedItem = null;
            txtFirstPolicyYear.Text = "";
            txtSourceQA.Text = "";
            cbbLeadSource.SelectedItem = null;
            txtFromList.Text = "";
            nudNoofcall.Value = 1;
        }
        private void dataGridQA_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.RowIndex <= dataGridQA.RowCount)
            {
                if (dataGridQA.Columns[e.ColumnIndex].Name == "edit")
                {
                    LoadDataControlsEditQA(int.Parse(dataGridQA.Rows[e.RowIndex].Cells[1].Value.ToString()));
                }
            }
        }
        private void LoadDataControlsEditQA(int id)
        {
            List<AppliQA> listQA = (List<AppliQA>)appliQABindingSource1.DataSource;
            AppliQA itemQA = listQA.Where(c => c.id == id).FirstOrDefault();
            if (itemQA != null)
            {
                cbbPolicyType.SelectedItem = itemQA.policies_type;
                cbbRenewalType.SelectedItem = itemQA.renewal_type;
                txtFirstPolicyYear.Text = itemQA.first_policy_year;
                cbbPure.SelectedItem = itemQA.pure;
                txtSourceQA.Text = itemQA.lead_sourceQA;
                cbbLeadSource.SelectedItem = itemQA.lead_source;
                cbbUpsellPlan.SelectedItem = itemQA.Upsell_plan;
                cbbRiderUpsell.SelectedItem = itemQA.rider_upsell;
                nudNoofcall.Value = itemQA.noofcall;
                txtFromList.Text = itemQA.from_list;

                IDAPPLIQA = id;
            }
        }
        #endregion

        #region Tab Appli - Admin
        private void btnUpdateAppli_Click(object sender, EventArgs e)
        {
            var flag = CheckDataInputAD();
            if (!flag) { MessageBox.Show("Input format error"); return; }
            var appADTable = GetItemTableAD(IDAPPLIAD);

            if (appADTable == null)
            {
                InsertAppliAD();
            }
            else
            {
                UpdateAppliAD(appADTable);
            }
            btnGetData_Admin_Click(sender, e);
        }
        private void UpdateAppliAD(application_admin_tbl ad_Tbl)
        {
            try
            {
                ad_Tbl.contract_number = txtSohopdongAd.Text.Trim();
                ad_Tbl.from_date = dtpNgayhieuluc.Value;
                ad_Tbl.to_date = dtpNgayhethan.Value;
                ad_Tbl.applied_date = dtpNgaylenhen.Value;
                ad_Tbl.payment_date = dtpNgaythanhtoan.Value;
                ad_Tbl.payment_methoy = txtthanhtoanAd.Text.Trim();
                ad_Tbl.rounder = txtRounder.Text.Trim();
                ad_Tbl.peding_reason = txtPendingreason.Text.Trim();
                ad_Tbl.private_note = txtNotePrivate.Text.Trim();
                ad_Tbl.admin_note = txtNoteAdmin.Text.Trim();
                ad_Tbl.proposer_add = txtProAddAD.Text.Trim();
                ad_Tbl.insured_add = txtInsurAddAD.Text.Trim();
                ad_Tbl.updated_user = userId;
                ad_Tbl.updated_date = DateTime.Now;

                db.SubmitChanges();
                refreshTabAppliAD();
                MessageBox.Show("success");
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private void InsertAppliAD()
        {
            try
            {
                var appointment_id = GetIdByAppointmentCode(CODE);

                var ad_tbl = new application_admin_tbl()
                {
                    appointment_id = appointment_id,
                    contract_number = txtSohopdongAd.Text.Trim(),
                    from_date = dtpNgayhieuluc.Value,
                    to_date = dtpNgayhethan.Value,
                    applied_date = dtpNgaylenhen.Value,
                    payment_date = dtpNgaythanhtoan.Value,
                    payment_methoy = txtthanhtoanAd.Text.Trim(),
                    rounder = txtRounder.Text.Trim(),
                    peding_reason = txtPendingreason.Text.Trim(),
                    private_note = txtNotePrivate.Text.Trim(),
                    admin_note = txtNoteAdmin.Text.Trim(),
                    proposer_add = txtProAddAD.Text.Trim(),
                    insured_add = txtInsurAddAD.Text.Trim(),
                    created_user = userId,
                    created_date = DateTime.Now
                };

                db.application_admin_tbls.InsertOnSubmit(ad_tbl);
                db.SubmitChanges();
                refreshTabAppliAD();
                MessageBox.Show("success");
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
                return;
            }
        }
        private void refreshTabAppliAD()
        {
            txtSohopdongAd.Text = "";
            txtthanhtoanAd.Text = "";
            txtRounder.Text = "";
            txtPendingreason.Text = "";
            txtNotePrivate.Text = "";
            txtNoteAdmin.Text = "";
            txtProAddAD.Text = "";
            txtInsurAddAD.Text = "";
        }
        private application_admin_tbl GetItemTableAD(int id)
        {
            try
            {
                application_admin_tbl appQA = new application_admin_tbl();

                appQA = (from a in db.application_admin_tbls
                         where a.id == id
                         select a).FirstOrDefault();
                return appQA;
            }
            catch
            {
                return new application_admin_tbl();
            }
        }
        private void btnGetData_Admin_Click(object sender, EventArgs e)
        {
            var dataAD = (from a in db.application_admin_tbls
                          select new AppliAdmin
                          {
                              id = a.id,
                              appointment_code = a.appointment_tbl.code,
                              contract_code = a.contract_number,
                              from_date = a.from_date,
                              to_date = a.to_date,
                              appointment_date = a.applied_date,
                              date_of_payment = a.payment_date,
                              payments = a.payment_methoy,
                              rounder = a.rounder,
                              pending_reason = a.peding_reason,
                              note_private = a.private_note,
                              note_admin = a.admin_note,
                              proposer_address = a.proposer_add,
                              Insured_address = a.insured_add
                          }).ToList();
            appliAdminBindingSource.DataSource = dataAD;
        }
        private bool CheckDataInputAD()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSohopdongAd.Text.Trim()) || string.IsNullOrEmpty(txtthanhtoanAd.Text.Trim()) || string.IsNullOrEmpty(txtRounder.Text.Trim()) ||
                    string.IsNullOrEmpty(txtProAddAD.Text.Trim()) || string.IsNullOrEmpty(txtInsurAddAD.Text.Trim()))
                {
                    return false;
                }
                if (dtpNgayhieuluc.Value >= dtpNgayhethan.Value)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void dataGridAdmin_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.RowIndex <= dataGridAdmin.RowCount)
            {
                if (dataGridAdmin.Columns[e.ColumnIndex].Name == "editAd")
                {
                    LoadDataControlsEditAD(int.Parse(dataGridAdmin.Rows[e.RowIndex].Cells[1].Value.ToString()));
                }
            }
        }
        private void LoadDataControlsEditAD(int id)
        {
            List<AppliAdmin> listAD = (List<AppliAdmin>)appliAdminBindingSource.DataSource;
            AppliAdmin itemAD = listAD.Where(c => c.id == id).FirstOrDefault();
            if (itemAD != null)
            {
                txtSohopdongAd.Text = itemAD.contract_code;
                dtpNgayhieuluc.Value = itemAD.from_date ?? DateTime.Now;
                dtpNgayhethan.Value = itemAD.to_date ?? DateTime.Now;
                dtpNgaylenhen.Value = itemAD.appointment_date ?? DateTime.Now;
                dtpNgaythanhtoan.Value = itemAD.date_of_payment ?? DateTime.Now;
                txtthanhtoanAd.Text = itemAD.payments;
                txtRounder.Text = itemAD.rounder;
                txtPendingreason.Text = itemAD.pending_reason;
                txtNotePrivate.Text = itemAD.note_private;
                txtNoteAdmin.Text = itemAD.note_admin;
                txtProAddAD.Text = itemAD.proposer_address;
                txtInsurAddAD.Text = itemAD.Insured_address;

                IDAPPLIAD = id;
            }
        }
        #endregion

        #region Tab Save CCCD
        private void cccd_btnMattruoc_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox_mattruoc.Image = Image.FromFile(open.FileName);
            }
        }
        private void cccd_btnMatsau_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox_matsau.Image = Image.FromFile(open.FileName);
            }
        }
        private void cccd_btnChonKhac_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox_khac.Image = Image.FromFile(open.FileName);
            }
        }
        private void cccd_btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckInfoCCCD())
                {
                    MessageBox.Show("Không tìm thấy thông tin phiếu.");
                    return;
                }

                string path_mattruoc = txtPhone.Text.Trim() + "_truoc.png";
                string path_matsau = txtPhone.Text.Trim() + "_sau.png";
                string path_khac = txtPhone.Text.Trim() + "_khac.png";

                if (pictureBox_mattruoc.Image != null)
                {
                    SavePic(pictureBox_mattruoc.Image, path_mattruoc);
                }
                else
                {
                    DeletePic(path_mattruoc);
                }
                if (pictureBox_matsau.Image != null)
                {
                    SavePic(pictureBox_matsau.Image, path_matsau);
                }
                else
                {
                    DeletePic(path_matsau);
                }
                if (pictureBox_khac.Image != null)
                {
                    SavePic(pictureBox_khac.Image, path_khac);
                }
                else
                {
                    DeletePic(path_khac);
                }

                MessageBox.Show("Cập nhật hình ảnh thành công.");
                ClearImage();

            }
            catch
            {
                MessageBox.Show("Cập nhật hình ảnh không thành công.");
            }
        }
        public void SavePic(Image img, string fileName)
        {
            byte[] data;
            using (MemoryStream m = new MemoryStream())
            {
                img.Save(m, img.RawFormat);
                data = m.ToArray();
            }
            Upload(new MemoryStream(data), fileName);
        }
        public void Upload(MemoryStream stream, string fileName)
        {
            try
            {
                var url = "ftp://192.168.1.5:21/" + fileName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("Administrator", "itcom@2023");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
                FtpWebResponse reponse = (FtpWebResponse)request.GetResponse();
                reponse.Close();
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
            }
        }
        private void DeletePic(string fileName)
        {
            try
            {
                var url = "ftp://192.168.1.5:21/" + fileName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("Administrator", "itcom@2023");
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse reponse = (FtpWebResponse)request.GetResponse();
                reponse.Close();
            }
            catch { }
        }
        private bool CheckInfoCCCD()
        {
            var querySearch = (from a in db.appointment_tbls
                               where a.ap_phone == txtPhone.Text.Trim() || a.code == txtPhone.Text.Trim() || a.in_cccd == txtPhone.Text.Trim()
                               select a).FirstOrDefault();
            if (querySearch == null)
            {
                return false;
            }
            return true;
        }
        private void cccd_btnxoamattruoc_Click(object sender, EventArgs e)
        {
            pictureBox_mattruoc.Image = null;
        }
        private void cccd_btnxoamatsau_Click(object sender, EventArgs e)
        {
            pictureBox_matsau.Image = null;
        }
        private void cccd_btnXoaKhac_Click(object sender, EventArgs e)
        {
            pictureBox_khac.Image = null;
        }
        private void cccd_btnload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckInfoCCCD())
                {
                    MessageBox.Show("Không tìm thấy thông tin phiếu.");
                    return;
                }
                string path_mattruoc = txtPhone.Text.Trim() + "_truoc.png";
                string path_matsau = txtPhone.Text.Trim() + "_sau.png";
                string path_khac = txtPhone.Text.Trim() + "_khac.png";

                var ms1 = LoadImageFromFtp(path_mattruoc);
                if (ms1.Length > 0)
                {
                    pictureBox_mattruoc.Image = Image.FromStream(ms1);
                }

                var ms2 = LoadImageFromFtp(path_matsau);
                if (ms2.Length > 0)
                {
                    pictureBox_matsau.Image = Image.FromStream(ms2);
                }

                var ms3 = LoadImageFromFtp(path_khac);

                if (ms3.Length > 0)
                {
                    pictureBox_khac.Image = Image.FromStream(ms3);
                }
            }
            catch
            {
                MessageBox.Show("Đã có lỗi xảy ra.");
            }
        }
        private MemoryStream LoadImageFromFtp(string fileName)
        {

            MemoryStream m = new MemoryStream();
            try
            {
                const int timeout = 5000;
                var url = "ftp://192.168.1.5:21/" + fileName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("Administrator", "itcom@2023");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;

                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                {
                    StreamReader str = new StreamReader(ftpStream);
                    str.BaseStream.CopyTo(m);
                }
                return m;
            }
            catch
            {
                return m;
            }
        }
        private void ClearImage()
        {
            pictureBox_mattruoc.Image = null;
            pictureBox_matsau.Image = null;
            pictureBox_khac.Image = null;
        }
        #endregion

        #region Report
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string download = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            string fileExport = download + "\\example_" + Timestamp + ".xlsx";
            ExportExcel.CreateExcelExample(fileExport);
        }
        private void btnSearch_appli_Click(object sender, EventArgs e)
        {
            var tb = GetDataExport(dtpfromdate_rp.Value, dtptodate_rp.Value);
            if (!tb.Any())
            {
                MessageBox.Show("Không tìm thấy dữ liệu!");
                return;
            }
            reportBindingSource.DataSource = tb;
        }
        public int tempId = 0;
        Dictionary<string, string> rpCost = new Dictionary<string, string>()
        {
            { "plan", "" },
            { "plan_price", "" },
            { "patient", "" },
            { "dental", "" },
            { "pregnant", "" },
            { "life", "" },
            { "accident", "" }
        };
        private Dictionary<string, string> GetPlan(int apointment_id)
        {
            if (tempId != apointment_id)
            {
                rpCost = new Dictionary<string, string>()
                    {
                        { "plan", "" },
                        { "plan_price", "0" },
                        { "patient", "0" },
                        { "dental", "0" },
                        { "pregnant", "0" },
                        { "life", "0" },
                        { "accident", "0" },
                        { "discount_bv", "0" },
                        { "money_bv", "0" },
                        { "discount_asahi", "0" },
                        { "totalPremiumOfOptionalPlan","0" },
                        { "totalPremium","0" },
                        { "total","0" }
                    };
                var queryCost = (from a in db.appointment_cost_tbls
                                 where a.appointment_id == apointment_id & a.cost_id == a.cost_tbl.id
                                 select new
                                 {
                                     id_insurance = a.cost_tbl.insurance_id,
                                     name_insurance = a.cost_tbl.insurance_tbl.name,
                                     id_package = a.cost_tbl.package_id,
                                     name_packag = a.cost_tbl.package_tbl.name,
                                     price = a.cost_tbl.price,
                                     id_cost = a.cost_id
                                 }).ToList();

                var queryCost2 = (from a in db.appointment_cost_tbls
                                  where a.appointment_id == apointment_id & a.cost_id == null
                                  select new
                                  {
                                      itc = a.itc_present,
                                      bv = a.bv_present,
                                      sinhmang = a.sinhmang,
                                      tainan = a.tainan,
                                      upbv = a.upbv_present,
                                      ash = a.ash_present
                                  }).FirstOrDefault();
                var totalPremiumOfOptionalPlan = 0;
                foreach (var i in queryCost)
                {
                    if (i.id_package == 1)
                    {
                        rpCost["plan"] = i.name_insurance;
                        rpCost["plan_price"] = i.price.ToString("n0");
                    }
                    if (i.id_package == 2)
                    {
                        rpCost["patient"] = i.price.ToString("n0");
                        totalPremiumOfOptionalPlan+= i.price;
                    }
                    if (i.id_package == 5)
                    {
                        rpCost["dental"] = i.price.ToString("n0");
                        totalPremiumOfOptionalPlan += i.price;
                    }
                    if (i.id_package == 6)
                    {
                        rpCost["pregnant"] = i.price.ToString("n0");
                        totalPremiumOfOptionalPlan += i.price;
                    }
                }
                float discount_bv = 0;
                float money_bv = 0;
                if (queryCost2 != null)
                {
                    rpCost["life"] = queryCost2.sinhmang.ToString();
                    rpCost["accident"] = queryCost2.tainan.ToString();
                    rpCost["discount_bv"] = queryCost2.bv.ToString();
                    rpCost["discount_asahi"] = queryCost2.ash.ToString();
                    discount_bv = (int)queryCost2.bv;
                }
                var totalPremium = float.Parse(rpCost["plan_price"]) + totalPremiumOfOptionalPlan;
                rpCost["totalPremiumOfOptionalPlan"] = totalPremiumOfOptionalPlan.ToString("n0");
                rpCost["totalPremium"] = totalPremium.ToString("n0");
                money_bv = totalPremium * discount_bv / 100;
                rpCost["money_bv"] = money_bv.ToString("n0");
                rpCost["total"] = (totalPremium - money_bv).ToString("n0");
                tempId = apointment_id;
            }
            return rpCost;
        }
        private string SubStringText(string s, string type)
        {
            var result = "";
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    switch (type)
                    {
                        case "ext":
                            result = s.Substring(s.Length - 3, 3);
                            break;
                        case "name":
                            result = s.Substring(0, s.Length - 4);
                            break;
                        case "number":
                            result = s.Substring(s.Length - 4, 4);
                            break;
                        default:
                            result = "";
                            break;
                    }
                }
                return result;
            }
            catch
            {
                return result;
            }

        }
        private string SubStringDate(DateTime d, string type)
        {
            var result = "";
            try
            {
                switch (type)
                {
                    case "dd":
                        result = d.Day.ToString();
                        break;
                    case "mm":
                        result = d.Month.ToString();
                        break;
                    case "yyyy":
                        result = d.Year.ToString();
                        break;
                    default:
                        result = "";
                        break;
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
        private string GetAge(string birthday)
        {
            var result = "";
            try
            {
                int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                int dob = int.Parse(birthday.Replace("-", ""));
                var age = (now - dob) / 10000;
                result = age.ToString();
                return result;
            }
            catch
            {
                return result;
            }
        }
        private void btnExport_appli_Click(object sender, EventArgs e)
        {
            try {
                var result = GetDataExport(dtpfromdate_rp.Value, dtptodate_rp.Value);
                if (!result.Any())
                {
                    MessageBox.Show("Không tìm thấy dữ liệu!");
                    return;
                }
                string download = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                string fileExport = download + "\\DailyReport_" + Timestamp + ".xlsx";
                ExportExcel.ExportExcelSingleSheet(fileExport, result);
                MessageBox.Show("Xuất báo cáo thành công.");
            }
            catch {
                MessageBox.Show("Đã có lỗi xảy ra.");
            }
            
        }
        private IEnumerable<Report> GetDataExport(DateTime fromdate, DateTime todate)
        {
            var tb = from a in db.appointment_tbls
                     join b in db.application_admin_tbls on a.id equals b.appointment_id
                     join c in db.application_qa_tbls on a.id equals c.appointment_id
                     where (b.payment_date >= fromdate && b.payment_date <= todate)
                     select new Report
                     {
                         Agent_Ext = SubStringText(a.tsr_name, "ext"),
                         Agent_Name = SubStringText(a.tsr_name, "name"),
                         Appointment = "", // null
                         Policy_Appointment = "", // null
                         DateOfApplication_Date = SubStringDate((DateTime)b.applied_date, "dd"),
                         DateOfApplication_Month = SubStringDate((DateTime)b.applied_date, "mm"),
                         DateOfApplication_Year = SubStringDate((DateTime)b.applied_date, "yyyy"),
                         DateOfPayment_Date = SubStringDate((DateTime)b.payment_date, "dd"),
                         DateOfPayment_Month = SubStringDate((DateTime)b.payment_date, "mm"),
                         DateOfPayment_Year = SubStringDate((DateTime)b.payment_date, "yyyy"),
                         ContractNumber = SubStringText(b.contract_number, "number"),
                         Proposer = a.rq_customer,
                         ProposerGender = a.rq_sex,
                         ProposerProvince = a.rq_address,
                         InsuredCodeID = a.rq_cccd,
                         TheInsured = a.in_customer,
                         InsuredGender = a.in_sex,
                         InsuredProvince = b.insured_add,
                         ProposerInsuredRelationship = a.in_relationship,
                         Age = GetAge(a.in_birthday),
                         Effective_Date = SubStringDate((DateTime)b.from_date, "dd"),
                         Effective_Month = SubStringDate((DateTime)b.from_date, "mm"),
                         Effective_Year = SubStringDate((DateTime)b.from_date, "yyyy"),
                         Expired_Date = SubStringDate((DateTime)b.to_date, "dd"),
                         Expired_Month = SubStringDate((DateTime)b.to_date, "mm"),
                         Expired_Year = SubStringDate((DateTime)b.to_date, "yyyy"),
                         Plan = GetPlan(a.id)["plan"],
                         Patient = GetPlan(a.id)["patient"],
                         Dental = GetPlan(a.id)["dental"],
                         Accident_Personal = GetPlan(a.id)["accident"],
                         Life_Personal = GetPlan(a.id)["life"],
                         Pregnant = GetPlan(a.id)["pregnant"],
                         PremiumOfCorePlan = GetPlan(a.id)["plan_price"],
                         TotalPremiumOfOptionalPlan = GetPlan(a.id)["totalPremiumOfOptionalPlan"],
                         TotalPremium = GetPlan(a.id)["totalPremium"],
                         ActuallyCollectedAfterDiscount = GetPlan(a.id)["total"],
                         DiscountForCustomer = GetPlan(a.id)["discount_bv"],
                         PremiumBVDiscount = GetPlan(a.id)["money_bv"],
                         PremiumAsahiDiscount = GetPlan(a.id)["discount_asahi"],
                         PremiumITComDiscount = "",
                         PremiumTranferToBaoViet = GetPlan(a.id)["total"],
                         PDS_Manual = c.from_list,
                         NumberOfCall = c.noofcall.ToString(),
                         CHANNEL = c.lead_source,
                         FirstPolicyYear = c.firstpolicy_year,
                         LeadMonthYear = c.lead_sourceQA,
                         RN_LeadSource = c.lead_source,
                         RN_PureIntro = c.pure_introduce,
                         RN_UpsellPlan = c.upsell_plan,
                         RN_UpsellRider = c.rider_plan,
                         Note = "",
                         PaymentMethod = b.payment_methoy,
                         Cancelled_Refund = "", // null
                         Total_Premium = "", // null
                         Total_Date = "", // null
                         Total_Up = "", // null
                         Campaign = "", // null
                         PaymentDemand = "", // null
                         RounderName = b.rounder
                     };
            return tb;
        }
        #endregion

        private void btnXoaPhieu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Không tìm thấy thông tin.");
                return;
            }
            string message = string.Format("Bạn có chắc chắn muốn xóa phiếu {0} này không ?",txtPhone.Text);
            string title = "Cảnh báo !";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                var querySearch = (from a in db.appointment_tbls
                                   where a.ap_phone == txtPhone.Text.Trim() || a.code == txtPhone.Text.Trim() || a.in_cccd == txtPhone.Text.Trim()
                                   select a).FirstOrDefault();
                if (querySearch == null || (querySearch.created_user != userId && role == 2) || (querySearch.created_user != userId && role == 4))
                {
                    MessageBox.Show("Không tìm thấy thông tin.");
                    return;
                }
                if (!DeleteData(querySearch))
                {
                    MessageBox.Show("Xóa phiếu không thành công.");
                }
                MessageBox.Show("Xóa phiếu thành công.");
            }
        }
        private bool DeleteData(appointment_tbl appointment)
        {
            try {
                var qa_tbl = (from a in db.qaqc_appointments
                              where a.appointment_id == appointment.id
                              select a).FirstOrDefault();
                if (qa_tbl != null)
                {
                    db.qaqc_appointments.DeleteOnSubmit(qa_tbl);
                }
                var cost_tbl = (from a in db.appointment_cost_tbls
                                where a.appointment_id == appointment.id
                                select a).ToList();
                if (cost_tbl != null & cost_tbl.Any())
                {
                    db.appointment_cost_tbls.DeleteAllOnSubmit(cost_tbl);
                }

                var appli_qa_tbl = (from a in db.application_qa_tbls
                                where a.appointment_id == appointment.id
                                select a).FirstOrDefault();
                if (appli_qa_tbl != null)
                {
                    db.application_qa_tbls.DeleteOnSubmit(appli_qa_tbl);
                }

                var appli_ad_tbl = (from a in db.application_admin_tbls
                                    where a.appointment_id == appointment.id
                                    select a).FirstOrDefault();
                if (appli_ad_tbl != null)
                {
                    db.application_admin_tbls.DeleteOnSubmit(appli_ad_tbl);
                }

                db.appointment_tbls.DeleteOnSubmit(appointment);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        } 
    }
}

