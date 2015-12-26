﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Prediction_Football_ML
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        LoadFixture load = new LoadFixture();
        LoadFixture DB = new LoadFixture();
        SqlConnection con;
        SqlCommand cm1,cm2,cm3,cm4,cm5,cm6,cm7,cm8;
        DataSet ds, ds1;
        SqlDataAdapter ap,ap1;



        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'standingDataSet._BXH_' table. You can move, or remove it, as needed.
            this.bXH_TableAdapter.Fill(this.standingDataSet._BXH_);
            // TODO: This line of code loads data into the 'ePLDataSet._db_' table. You can move, or remove it, as needed.
            this.db_TableAdapter.Fill(this.ePLDataSet._db_);
            this.LoadDBComboBox();
            //this.LoadDBListBox();
            cmb1.Text = "Chọn vòng đấu";
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            phongdodoinha.Text = phongdodoikhach.Text = "Vui lòng chọn 'Thông Tin Trận Đấu'";
        }

        public void LoadDBComboBox()
        {
            con = load.conDB();
            con.Open();
        }

        #region select round and match
        private void cmb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _select = this.cmb1.GetItemText(this.cmb1.SelectedItem);
            cm1 = new SqlCommand("select VS from [Fixtures].[dbo].[lichthidau$] where ROUND=@round", con);
            cm1.Parameters.Add("@round", SqlDbType.NVarChar, -1);
            cm1.Parameters["@round"].Value = _select;
            
            
            ap = new SqlDataAdapter(cm1);
            ds = new System.Data.DataSet();
            ap.Fill(ds, "[Fixtures].[dbo].[lichthidau$]");
            cmb2.DataSource = ds.Tables[0];
            cmb2.DisplayMember = "VS";
        }
        #endregion

        private void btn_dudoan_Click(object sender, EventArgs e)
        {
            string _select = this.cmb1.GetItemText(this.cmb1.SelectedItem);
            try
            {
                if (_select == "Chọn vòng đấu")
                    MessageBox.Show("Vui Lòng Chọn Vòng Đấu Và Trận Đấu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (HomeWin > AwayWin) MessageBox.Show(Home + " thắng");
                    else
                    {
                        if (AwayWin > HomeWin) MessageBox.Show(Away + " thắng");
                        else
                            MessageBox.Show("Hai đội hòa nhau");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        string Home = "", Away = "";
        int HomeWin = 0, AwayWin = 0, Draw = 0;//, Home_Pos = 0, Away_Pos = 0;

        private void btn_info_Click(object sender, EventArgs e)
        {
            HomeWin = 0; AwayWin = 0; Draw = 0; //Home_Pos = 0; Away_Pos = 0;
            string _select = this.cmb1.GetItemText(this.cmb1.SelectedItem);
            string match = this.cmb2.GetItemText(this.cmb2.SelectedItem);
            try
            {
                if (_select == "Chọn vòng đấu")
                    MessageBox.Show("Vui Lòng Chọn Vòng Đấu Và Trận Đấu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    #region get home name and away name
                    for (int i = 0; i < match.Length; i++)
                    {
                        if (match[i] == '-')
                        {
                            Home = match.Substring(0, i);
                            Away = match.Substring(i + 2, match.Length - i - 2);
                        }
                    }
                    #endregion

                    this.label9.Text = "Phong Độ " + Home;
                    this.label8.Text = "Phong Độ " + Away;

                    this.label2.Text = "5 trận gần nhất";
                    this.label3.Text = "5 trận gần nhất";

                    #region thanh tich doi dau
                    cm2 = new SqlCommand("select * from [EPL].[dbo].[db$] where (HomeTeam=@hometeam and AwayTeam=@awayteam) or (HomeTeam=@awayteam and AwayTeam=@hometeam) ORDER BY Date DESC", con);
                    cm2.Parameters.Add("@hometeam", SqlDbType.NVarChar, -1);
                    cm2.Parameters.Add("@awayteam", SqlDbType.NVarChar, -1);
                    cm2.Parameters["@hometeam"].Value = Home;
                    cm2.Parameters["@awayteam"].Value = Away;
                    ap = new SqlDataAdapter(cm2);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[EPL].[dbo].[db$]");
                    dataGridView1.DataSource = ds.Tables[0];
                    dataGridView1.AllowUserToAddRows = false;
                    DataTable dt = ds.Tables[0];
                    #endregion

                    double[] HomeGoals = new double[dt.Rows.Count];
                    double[] AwayGoals = new double[dt.Rows.Count];
                    double[] BetHome = new double[dt.Rows.Count];
                    double[] BetDraw = new double[dt.Rows.Count];
                    double[] BetAway = new double[dt.Rows.Count];
                    
                    int index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        HomeGoals[index] = (double)row["FTHG"];
                        AwayGoals[index] = (double)row["FTAG"];
                        BetHome[index] = (double)row["B365H"];
                        BetDraw[index] = (double)row["B365D"];
                        BetAway[index] = (double)row["B365A"];
                        index++;
                    }

                    #region phongdodoinha
                    this.phongdodoinha.BackColor = System.Drawing.SystemColors.Window;
                    this.phongdodoinha.ForeColor = System.Drawing.Color.Black;
                    
                    cm3 = new SqlCommand("select TOP 5 HomeResult from [EPL].[dbo].[db$] where (HomeTeam=@hometeam or AwayTeam=@hometeam) ORDER BY Date DESC", con);
                    cm3.Parameters.Add("@hometeam", SqlDbType.NVarChar, -1);
                    cm3.Parameters["@hometeam"].Value = Home;
                    ap = new SqlDataAdapter(cm3);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[EPL].[dbo].[db$]");
                    phongdodoinha.DataSource = ds.Tables[0];
                    phongdodoinha.DisplayMember = "HomeResult";
                    #endregion

                    #region phongdodoikhach
                    this.phongdodoikhach.BackColor = System.Drawing.SystemColors.Window;
                    this.phongdodoikhach.ForeColor = System.Drawing.Color.Black;
                    
                    cm4 = new SqlCommand("select TOP 5 Result from [EPL].[dbo].[db$] where (HomeTeam=@awayteam or AwayTeam=@awayteam) ORDER BY Date DESC", con);
                    cm4.Parameters.Add("@awayteam", SqlDbType.NVarChar, -1);
                    cm4.Parameters["@awayteam"].Value = Away;
                    ap = new SqlDataAdapter(cm4);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[EPL].[dbo].[db$]");
                    phongdodoikhach.DataSource = ds.Tables[0];
                    phongdodoikhach.DisplayMember = "Result";
                    #endregion

                    #region phong do doi nha san nha
                    this.phongdosannha.BackColor = System.Drawing.SystemColors.Window;
                    this.phongdosannha.ForeColor = System.Drawing.Color.Black;
                    cm5 = new SqlCommand("select TOP 5 Result from [EPL].[dbo].[db$] where HomeTeam=@hometeam ORDER BY Date DESC", con);
                    cm5.Parameters.Add("@hometeam", SqlDbType.NVarChar, -1);
                    cm5.Parameters["@hometeam"].Value = Home;
                    ap = new SqlDataAdapter(cm5);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[EPL].[dbo].[db$]");
                    phongdosannha.DataSource = ds.Tables[0];
                    phongdosannha.DisplayMember = "Result";
                    #endregion

                    #region phong do doi khach san khach
                    this.phongdosankhach.BackColor = System.Drawing.SystemColors.Window;
                    this.phongdosankhach.ForeColor = System.Drawing.Color.Black;
                    cm6 = new SqlCommand("select TOP 5 AwayResult from [EPL].[dbo].[db$] where AwayTeam=@awayteam ORDER BY Date DESC", con);
                    cm6.Parameters.Add("@awayteam", SqlDbType.NVarChar, -1);
                    cm6.Parameters["@awayteam"].Value = Away;
                    ap = new SqlDataAdapter(cm6);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[EPL].[dbo].[db$]");
                    phongdosankhach.DataSource = ds.Tables[0];
                    phongdosankhach.DisplayMember = "AwayResult";
                    #endregion

                    #region vi tri bxh
                    cm7 = new SqlCommand("select Xephang from [Standing].[dbo].[BXH$] where (HomeTeam=@hometeam or AwayTeam=@awayteam)", con);
                    cm7.Parameters.Add("@hometeam", SqlDbType.NVarChar, -1);
                    cm7.Parameters.Add("@awayteam", SqlDbType.NVarChar, -1);
                    cm7.Parameters["@hometeam"].Value = Home;
                    cm7.Parameters["@awayteam"].Value = Away;
                    ap = new SqlDataAdapter(cm7);
                    ds = new System.Data.DataSet();
                    ap.Fill(ds, "[Standing].[dbo].[BXH$]");
                    DataTable dt1 = ds.Tables[0];

                    int id=0;
                    double[] Pos = new double[dt1.Rows.Count];
                    foreach (DataRow row in dt1.Rows)
                    {
                        Pos[id++] = (double)row["Xephang"];
                    }
                    #endregion

                    #region du doan
                    #region tieu chi thanh tich doi dau
                    for (int i = 0; i < index; i++)
                    {
                        if (HomeGoals[i] > AwayGoals[i]) HomeWin++;
                        else
                        {
                            if (HomeGoals[i] < AwayGoals[i]) AwayWin++;
                            else
                                Draw++;
                        }
                    }
                    #endregion

                    #region tieu chi bang xep hang
                    if (Math.Abs(Pos[0] - Pos[1]) <= 2)
                    {
                        HomeWin++;
                        AwayWin++;
                        Draw++;
                    }
                    else
                    {
                        if (Pos[0] > Pos[1])
                        {
                            HomeWin++;
                        }
                        else
                            AwayWin++;
                    }
                    #endregion

                    #region reset
                    Array.Clear(HomeGoals, 0, index);
                    Array.Clear(AwayGoals, 0, index);
                    Array.Clear(BetHome, 0, index);
                    Array.Clear(BetAway, 0, index);
                    Array.Clear(BetDraw, 0, index);
                    Array.Clear(Pos, 0, id);
                    #endregion

                    #endregion
                }
            }
            #region ex
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }

        private void btnBXH_Click(object sender, EventArgs e)
        {
            cm8 = new SqlCommand("select * from [Standing].[dbo].[BXH$]", con);
            ap1 = new SqlDataAdapter(cm8);
            ds1 = new System.Data.DataSet();
            ap1.Fill(ds1, "[Standing].[dbo].[BXH$]");
            dataGridView2.DataSource = ds1.Tables[0];
            dataGridView2.AllowUserToAddRows = false;
        }
    }
}