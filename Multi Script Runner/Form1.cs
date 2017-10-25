using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Multi_Script_Runner
{
    public partial class Form1 : Form
    {
        bool IsConnected = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtUID.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter the required details.");
                    IsConnected = false;
                }
                else
                {
                    lstDbs.Items.Clear();
                    SqlConnection SqlCon = new SqlConnection("server=" + txtServer.Text + ";uid=" + txtUID.Text + ";pwd=" + txtPassword.Text);
                    SqlCon.Open();
                    SqlCommand SqlCom = new SqlCommand();
                    SqlCom.Connection = SqlCon;
                    SqlCom.CommandType = CommandType.StoredProcedure;
                    SqlCom.CommandText = "sp_databases";

                    System.Data.SqlClient.SqlDataReader SqlDR;
                    SqlDR = SqlCom.ExecuteReader();

                    while (SqlDR.Read())
                    {
                        lstDbs.Items.Add(SqlDR.GetString(0));
                    }

                    SqlDR.Close();
                    SqlCon.Close();

                    IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsConnected = false;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                MessageBox.Show("Please connect to the server before executing scripts.");
            }
            else
            {
                panel2.Controls.Clear();
                Point Point = new Point(0, 0);

                foreach (object obj in lstDbs.SelectedItems)
                {
                    SqlConnection SqlCon = new SqlConnection("server=" + txtServer.Text + ";uid=" + txtUID.Text + ";pwd=" + txtPassword.Text + ";Database=" + obj.ToString());
                    SqlCon.Open();

                    SqlCommand SqlCom = new SqlCommand();
                    SqlCom.Connection = SqlCon;
                    SqlCom.CommandType = CommandType.Text;
                    SqlCom.CommandText = txtQuery.Text;

                    DataSet ds = new DataSet();
                    SqlDataAdapter SqlDA = new SqlDataAdapter(SqlCom);
                    SqlDA.Fill(ds);

                    Label lbl = new Label();
                    panel2.Controls.Add(lbl);
                    lbl.Text = "Query executed on DB " + obj.ToString();
                    lbl.Location = new Point(0, panel2.AutoScrollPosition.Y + Point.Y);
                    lbl.Font = new Font(lbl.Font, FontStyle.Bold);
                    lbl.Width = 500;
                    Point.Y += 30;

                    if (ds.Tables.Count > 0)
                    {
                        DataGridView dg1 = new DataGridView();
                        panel2.Controls.Add(dg1);

                        dg1.DataSource = ds.Tables[0];
                        dg1.AutoGenerateColumns = true;
                        dg1.Name = obj.ToString() + "_DB";
                        dg1.Height = 200;
                        dg1.Width = 550;
                        dg1.Location = new Point(0, panel2.AutoScrollPosition.Y + Point.Y);
                        Point.Y += 200;
                    }

                    SqlCon.Close();
                }
            }
        }
    }
}
