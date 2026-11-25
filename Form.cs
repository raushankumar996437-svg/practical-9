using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace PlacementDashboard
{
    public partial class Form1 : Form
    {
        private Label lblStudents;
        private Label lblCompanies;
        private Label lblPlacements;
        private Label lblPackage;

        private DataGridView dgvPlacements;

        private BindingSource bindingSource = new BindingSource();

        public Form1()
        {
            InitializeComponent();

            CreateDashboard();
        }

        private void CreateDashboard()
        {
            this.Text = "Placement Dashboard";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            Label heading = new Label();

            heading.Text = "Placement Dashboard";
            heading.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            heading.Location = new Point(30, 20);
            heading.AutoSize = true;

            this.Controls.Add(heading);

            CreateCard("Total Students", "0", 30, out lblStudents);

            CreateCard("Total Companies", "0", 310, out lblCompanies);

            CreateCard("Total Placements", "0", 590, out lblPlacements);

            CreateCard("Average Package", "0 LPA", 870, out lblPackage);

            Label recordsHeading = new Label();

            recordsHeading.Text = "Student Placement Records";
            recordsHeading.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            recordsHeading.Location = new Point(30, 210);
            recordsHeading.AutoSize = true;

            this.Controls.Add(recordsHeading);

            dgvPlacements = new DataGridView();

            dgvPlacements.Location = new Point(30, 260);
            dgvPlacements.Size = new Size(1100, 350);

            dgvPlacements.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvPlacements.BackgroundColor = Color.White;
            dgvPlacements.BorderStyle = BorderStyle.None;

            dgvPlacements.ReadOnly = true;
            dgvPlacements.AllowUserToAddRows = false;
            dgvPlacements.AllowUserToDeleteRows = false;

            dgvPlacements.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvPlacements.RowHeadersVisible = false;

            dgvPlacements.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(35, 55, 80);

            dgvPlacements.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvPlacements.EnableHeadersVisualStyles = false;

            dgvPlacements.DataSource = bindingSource;

            this.Controls.Add(dgvPlacements);

            LoadDashboard();
        }

        private void CreateCard(
            string title,
            string value,
            int x,
            out Label valueLabel)
        {
            Panel card = new Panel();

            card.Location = new Point(x, 100);
            card.Size = new Size(250, 90);
            card.BackColor = Color.FromArgb(240, 245, 250);

            Label titleLabel = new Label();

            titleLabel.Text = title;
            titleLabel.Location = new Point(15, 10);
            titleLabel.Font = new Font("Segoe UI", 11);
            titleLabel.AutoSize = true;

            valueLabel = new Label();

            valueLabel.Text = value;
            valueLabel.Location = new Point(15, 40);
            valueLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            valueLabel.ForeColor = Color.FromArgb(0, 100, 200);
            valueLabel.AutoSize = true;

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            this.Controls.Add(card);
        }

        private void LoadDashboard()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings[
                    "PlacementConnection"].ConnectionString;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                    (SELECT COUNT(*)
                     FROM Placements) AS TotalStudents,

                    (SELECT COUNT(DISTINCT CompanyName)
                     FROM Placements
                     WHERE CompanyName <> 'Not Assigned')
                    AS TotalCompanies,

                    (SELECT COUNT(*)
                     FROM Placements
                     WHERE PlacementStatus = 'Placed')
                    AS TotalPlacements,

                    (SELECT AVG(Package)
                     FROM Placements
                     WHERE PlacementStatus = 'Placed')
                    AS AveragePackage";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblStudents.Text =
                        reader["TotalStudents"].ToString();

                    lblCompanies.Text =
                        reader["TotalCompanies"].ToString();

                    lblPlacements.Text =
                        reader["TotalPlacements"].ToString();

                    decimal averagePackage = 0;

                    if (reader["AveragePackage"] != DBNull.Value)
                    {
                        averagePackage =
                            Convert.ToDecimal(reader["AveragePackage"]);
                    }

                    lblPackage.Text =
                        averagePackage.ToString("0.00") + " LPA";
                }

                reader.Close();
            }

            LoadPlacementRecords();
        }

        private void LoadPlacementRecords()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings[
                    "PlacementConnection"].ConnectionString;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        PlacementID,
                        StudentName,
                        Branch,
                        CompanyName,
                        Package,
                        PlacementStatus
                    FROM Placements";

                SqlDataAdapter adapter =
                    new SqlDataAdapter(query, con);

                DataTable table = new DataTable();

                adapter.Fill(table);

                bindingSource.DataSource = table;
            }
        }
    }
}