using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRG282_Project_Test.BLL;
using PRG282_Project_Test.Models;
using System.IO;
using System.ComponentModel;
using System.Data;


namespace PRG282_Project_Test.Presentation
{
    public class MainForm : Form
    {
        // Controls
        private DataGridView dgv;
        private TextBox txtHeroID, txtName, txtAge, txtPower, txtScore;
        private Button btnAdd, btnUpdate, btnDelete, btnLoad, btnReport, btnClear;
        private Label lblTotal, lblAvgAge, lblAvgScore, lblCounts;

        // File paths
        private readonly string dataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "superheroes.txt");
        private readonly string summaryFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "summary.txt");

        private BindingList<Superhero> heroes = new BindingList<Superhero>();

        public MainForm()
        {
            Text = "One Kick Heroes Academy - Superhero Database System";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            EnsureFilesExist();
            LoadHeroesFromFile();
        }

        private void EnsureFilesExist()
        {
            try
            {
                if (!File.Exists(dataFile)) File.WriteAllText(dataFile, "");
                if (!File.Exists(summaryFile)) File.WriteAllText(summaryFile, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ensuring data files exist: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            // DataGridView
            dgv = new DataGridView { Left = 10, Top = 10, Width = 650, Height = 620, ReadOnly = true, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hero ID", DataPropertyName = "HeroID", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 140 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Age", DataPropertyName = "Age", Width = 50 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Superpower", DataPropertyName = "Superpower", Width = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Exam Score", DataPropertyName = "ExamScore", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Rank", DataPropertyName = "Rank", Width = 60 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Threat Level", DataPropertyName = "ThreatLevel", Width = 250 });

            Controls.Add(dgv);

            // Input area
            var lbl1 = new Label { Left = 680, Top = 10, Text = "Hero ID" };
            txtHeroID = new TextBox { Left = 680, Top = 30, Width = 260 };

            var lbl2 = new Label { Left = 680, Top = 60, Text = "Name" };
            txtName = new TextBox { Left = 680, Top = 80, Width = 260 };

            var lbl3 = new Label { Left = 680, Top = 110, Text = "Age" };
            txtAge = new TextBox { Left = 680, Top = 130, Width = 100 };

            var lbl4 = new Label { Left = 680, Top = 160, Text = "Superpower" };
            txtPower = new TextBox { Left = 680, Top = 180, Width = 260 };

            var lbl5 = new Label { Left = 680, Top = 210, Text = "Exam Score (0-100)" };
            txtScore = new TextBox { Left = 680, Top = 230, Width = 100 };

            Controls.AddRange(new Control[] { lbl1, txtHeroID, lbl2, txtName, lbl3, txtAge, lbl4, txtPower, lbl5, txtScore });

            // Buttons
            btnAdd = new Button { Left = 680, Top = 270, Width = 120, Text = "Add New" };
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = new Button { Left = 820, Top = 270, Width = 120, Text = "Update" };
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = new Button { Left = 680, Top = 310, Width = 120, Text = "Delete" };
            btnDelete.Click += BtnDelete_Click;

            btnLoad = new Button { Left = 820, Top = 310, Width = 120, Text = "Refresh / Load" };
            btnLoad.Click += (s, e) => LoadHeroesFromFile();

            btnReport = new Button { Left = 680, Top = 350, Width = 260, Text = "Generate Summary Report" };
            btnReport.Click += BtnReport_Click;

            btnClear = new Button { Left = 680, Top = 390, Width = 260, Text = "Clear Inputs" };
            btnClear.Click += (s, e) => ClearInputs();

            Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnLoad, btnReport, btnClear });

            // Summary labels
            lblTotal = new Label { Left = 680, Top = 440, Width = 260, Height = 30, Text = "Total Heroes: 0" };
            lblAvgAge = new Label { Left = 680, Top = 470, Width = 260, Height = 30, Text = "Average Age: 0" };
            lblAvgScore = new Label { Left = 680, Top = 500, Width = 260, Height = 30, Text = "Average Score: 0" };
            lblCounts = new Label { Left = 680, Top = 530, Width = 260, Height = 120, Text = "Ranks: S=0, A=0, B=0, C=0" };

            Controls.AddRange(new Control[] { lblTotal, lblAvgAge, lblAvgScore, lblCounts });

            // Bind DataGridView
            dgv.DataSource = heroes;
        }

        private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < heroes.Count)
            {
                var h = heroes[e.RowIndex];
                txtHeroID.Text = h.HeroID;
                txtName.Text = h.Name;
                txtAge.Text = h.Age.ToString();
                txtPower.Text = h.Superpower;
                txtScore.Text = h.ExamScore.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtHeroID.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Hero ID and Name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtAge.Text, out int age) || age < 0)
                {
                    MessageBox.Show("Age must be a non-negative integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtScore.Text, out int score) || score < 0 || score > 100)
                {
                    MessageBox.Show("Exam Score must be an integer between 0 and 100.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check duplicate ID
                var all = LoadHeroesFromFileInternal();
                if (all.Any(h => h.HeroID.Equals(txtHeroID.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("A hero with that ID already exists. Use Update instead.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var hero = new Superhero(txtHeroID.Text.Trim(), txtName.Text.Trim(), age, txtPower.Text.Trim(), score);
                AppendHeroToFile(hero);

                // Commit message instruction (for student): stage & commit after add
                // Refresh
                LoadHeroesFromFile();
                ClearInputs();

                MessageBox.Show("Hero added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding hero: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHeroID.Text))
                {
                    MessageBox.Show("Enter the Hero ID to update (or double-click a row to load).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtAge.Text, out int age) || age < 0)
                {
                    MessageBox.Show("Age must be a non-negative integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtScore.Text, out int score) || score < 0 || score > 100)
                {
                    MessageBox.Show("Exam Score must be an integer between 0 and 100.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var list = LoadHeroesFromFileInternal();
                var hero = list.FirstOrDefault(h => h.HeroID.Equals(txtHeroID.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                if (hero == null)
                {
                    MessageBox.Show("Hero not found. Cannot update.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                hero.Name = txtName.Text.Trim();
                hero.Age = age;
                hero.Superpower = txtPower.Text.Trim();
                hero.ExamScore = score;
                hero.CalculateRankAndThreat();

                SaveAllHeroesToFile(list);
                LoadHeroesFromFile();
                MessageBox.Show("Hero updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating hero: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select a hero row to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var row = dgv.SelectedRows[0];
                var hero = row.DataBoundItem as Superhero;
                if (hero == null) return;

                var confirm = MessageBox.Show($"Are you sure you want to delete hero '{hero.Name}' (ID: {hero.HeroID})?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                var list = LoadHeroesFromFileInternal();
                var toRemove = list.FirstOrDefault(h => h.HeroID.Equals(hero.HeroID, StringComparison.OrdinalIgnoreCase));
                if (toRemove != null) list.Remove(toRemove);
                SaveAllHeroesToFile(list);
                LoadHeroesFromFile();
                MessageBox.Show("Hero deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting hero: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            try
            {
                var list = LoadHeroesFromFileInternal();
                int total = list.Count;
                double avgAge = total > 0 ? list.Average(h => h.Age) : 0;
                double avgScore = total > 0 ? list.Average(h => h.ExamScore) : 0;
                int s = list.Count(h => h.Rank == "S");
                int a = list.Count(h => h.Rank == "A");
                int b = list.Count(h => h.Rank == "B");
                int c = list.Count(h => h.Rank == "C");

                lblTotal.Text = $"Total Heroes: {total}";
                lblAvgAge.Text = $"Average Age: {Math.Round(avgAge, 2)}";
                lblAvgScore.Text = $"Average Score: {Math.Round(avgScore, 2)}";
                lblCounts.Text = $"Ranks: S={s}, A={a}, B={b}, C={c}";

                // Save summary
                var sb = new StringBuilder();
                sb.AppendLine("One Kick Heroes Academy - Summary Report");
                sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine($"Total Heroes: {total}");
                sb.AppendLine($"Average Age: {Math.Round(avgAge, 2)}");
                sb.AppendLine($"Average Exam Score: {Math.Round(avgScore, 2)}");
                sb.AppendLine($"Rank Counts: S={s}, A={a}, B={b}, C={c}");

                File.WriteAllText(summaryFile, sb.ToString());

                MessageBox.Show("Summary generated and saved to summary.txt", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Note: Students should stage & commit after report generation (see README for git commands).
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtHeroID.Text = "";
            txtName.Text = "";
            txtAge.Text = "";
            txtPower.Text = "";
            txtScore.Text = "";
        }

        private void AppendHeroToFile(Superhero hero)
        {
            try
            {
                File.AppendAllLines(dataFile, new[] { hero.ToRecord() });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing to data file: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Superhero> LoadHeroesFromFileInternal()
        {
            var list = new List<Superhero>();
            try
            {
                if (!File.Exists(dataFile)) return list;
                var lines = File.ReadAllLines(dataFile);
                foreach (var line in lines)
                {
                    var h = Superhero.FromRecord(line);
                    if (h != null) list.Add(h);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading data file: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }

        private void SaveAllHeroesToFile(List<Superhero> list)
        {
            try
            {
                var lines = list.Select(h => h.ToRecord()).ToArray();
                File.WriteAllLines(dataFile, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data file: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHeroesFromFile()
        {
            var list = LoadHeroesFromFileInternal();
            heroes.Clear();
            foreach (var h in list) heroes.Add(h);
            // Update summary labels as a quick view
            var total = heroes.Count;
            lblTotal.Text = $"Total Heroes: {total}";
            lblAvgAge.Text = $"Average Age: {(total > 0 ? Math.Round(heroes.Average(h => h.Age), 2) : 0)}";
            lblAvgScore.Text = $"Average Score: {(total > 0 ? Math.Round(heroes.Average(h => h.ExamScore), 2) : 0)}";
            lblCounts.Text = $"Ranks: S={heroes.Count(h => h.Rank == "S")}, A={heroes.Count(h => h.Rank == "A")}, B={heroes.Count(h => h.Rank == "B")}, C={heroes.Count(h => h.Rank == "C")}";
        }
    }
}
