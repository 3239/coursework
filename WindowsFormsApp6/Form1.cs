using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        bool isLoadListMark = false;
        private SQLiteConnection SQLiteConn;
        Marka MarkGen = new Marka(0);
        Marka MarkFive = new Marka(0);
        int CountBlocks = 0;
        List<Marka> ListMarks = new List<Marka>();
        string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public Form1()
        {
            InitializeComponent();
            comboBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            tabPage2.Parent = null;
            tabPage3.Parent = null;
            tabPage4.Parent = null;
            tabPage5.Parent = null;

        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();


        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    FillCbSelectBlocks();
                    DrawListBox(lbLeft, MarkGen.DTable, isLoadListMark);
                    isLoadListMark = true;
                    break;
                case 3:
                    FillCbSelectBlocksTab3();
                    nudCountSunBlocka_ValueChanged(nudCountSunBlocka, null);
                    break;
                case 4:
                    DrawListBox(lbLeftFive, MarkGen.DTable);
                    break;
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPage4)
            {
                e.Cancel = !IsFullPointsFromBlocls();
            }
        }

        private bool IsFullPointsFromBlocls()
        {
            if (ListMarks == null)
                return false;
            if (ListMarks.Count == 0)
                return false;
            if (ListMarks.FindAll(x => x.DTable == null).Count != 0)
                return false;
            else
                return true;
        }

        private void btChangeLb_Click(object sender, EventArgs e)
        {
            Button bt = ((Button)sender);
            switch (bt.Name)
            {
                case "bt1Add":
                    if (lbLeft.SelectedIndex < 0)
                        return;
                    var item = lbLeft.Items[lbLeft.SelectedIndex];
                    lbRight.Items.Add(item);
                    lbLeft.Items.RemoveAt(lbLeft.SelectedIndex);
                    bt1Add.Enabled = ValidationCountPointsAdd(lbRight);
                    bt1Del.Enabled = ValidationCountPointsDel(lbRight);
                    break;
                case "bt1Del":
                    if (lbRight.SelectedIndex < 0)
                        return;
                    var item1 = lbRight.Items[lbRight.SelectedIndex];
                    lbLeft.Items.Add(item1);
                    lbRight.Items.RemoveAt(lbRight.SelectedIndex);
                    bt1Add.Enabled = ValidationCountPointsAdd(lbRight);
                    bt1Del.Enabled = ValidationCountPointsDel(lbRight);
                    break;
                case "btAddFive":
                    if (lbLeftFive.SelectedIndex < 0)
                        return;
                    var item2 = lbLeftFive.Items[lbLeftFive.SelectedIndex];
                    lbRightFive.Items.Add(item2);
                    lbLeftFive.Items.RemoveAt(lbLeftFive.SelectedIndex);
                    //btAddFive.Enabled = ValidationCountPoints(lbRightFive);
                    break;
                case "btDelFive":
                    if (lbRightFive.SelectedIndex < 0)
                        return;
                    var item3 = lbRightFive.Items[lbRightFive.SelectedIndex];
                    lbLeftFive.Items.Add(item3);
                    lbRightFive.Items.RemoveAt(lbRightFive.SelectedIndex);
                    break;
                case "btAddTab3":
                    if (lbLeftTab3.SelectedIndex < 0)
                        return;
                    var item4 = lbLeftTab3.Items[lbLeftTab3.SelectedIndex];
                    lbRigtTab3.Items.Add(item4);
                    lbLeftTab3.Items.RemoveAt(lbLeftTab3.SelectedIndex);

                    btAddTab3.Enabled = ValidationCountPointsAddTab3(lbRigtTab3);
                    btDelTab3.Enabled = ValidationCountPointsDelTab3(lbRigtTab3);
                    break;
                case "btDelTab3":
                    if (lbRigtTab3.SelectedIndex < 0)
                        return;
                    var item5 = lbRigtTab3.Items[lbRigtTab3.SelectedIndex];
                    lbLeftTab3.Items.Add(item5);
                    lbRigtTab3.Items.RemoveAt(lbRigtTab3.SelectedIndex);

                    btAddTab3.Enabled = ValidationCountPointsAddTab3(lbRigtTab3);
                    btDelTab3.Enabled = ValidationCountPointsDelTab3(lbRigtTab3);
                    break;
            }
        }

        private void LoadParamm()
        {
            string SQLQuery = "SELECT A, E, CountBlocks FROM Схема";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            tbA1.Text = reader["A"].ToString();
            tbA2.Text = tbA1.Text;
            tbE1.Text = reader["E"].ToString();
            tbE2.Text = tbE1.Text;
            textBox1.Text = tbE1.Text;
            tb3.Text = reader["CountBlocks"].ToString();
            CountBlocks = Convert.ToInt32(tb3.Text);
            MarkGen.A = Convert.ToDouble(tbA1.Text);
            MarkGen.E = Convert.ToDouble(tbE1.Text);
        }

        #region Первая вкладка

        private void button5_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(dgvMFault[1, dgvMFault.Rows.Count - 1].Value) == "Не стабильное")
            {
                MessageBox.Show("Вам следует перейти на второй уровень. Прогнозное состояние объекта не стабильное.");
            }
            else
                MessageBox.Show("Переход на второй уровень не требуется. Прогнозное состояние объекта стабильное.");
        }

        private bool OpenDBFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Все файлы(*.*)|*.*|Текстовые файлы (*.db)|*.db";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version=3;");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;
                return true;
            }
            else return false;
        }
        private void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
        }
        private string SQL_AllTable(string columns)
        {
            return "SELECT " + columns + " FROM [" + comboBox1.SelectedItem + "] order by 1";
        }

        private string SQL_FilterByProduct()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "]" + "WHERE [Количество (коробки)] <=2;";
        }
        private void ShowTable(string SQLQuery)
        {
            DataTable dt = GetTableFromDB(SQLQuery);
            MarkGen = new Marka(0, dt, GetTableFromDB(SQLQuery), GetTableFromDB(SQLQuery), Convert.ToDouble(tbA2.Text), Convert.ToDouble(tbE2.Text));

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            for (int col = 0; col < dt.Columns.Count; col++)
            {
                string ColName = dt.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);
                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dt.Rows[row].ItemArray);
            }
        }

        private DataTable GetTableFromDB(string SQLQuery)
        {
            DataTable dt = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dt);
            return dt;
        }

        private void GetTableColumns()
        {
            string SQLQuery = "PRAGMA table_info(\"" + comboBox1.SelectedItem + "\");";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader read = command.ExecuteReader();
        }
        private void GetManufactures()
        {
            int kol = 0;
            string s1, s2;
            //comboBox1.Items.Clear();
            for (int row = 0; row < MarkGen.DTable.Rows.Count; row++)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    //s1 = (string)dTable.Rows[row].ItemArray[2];
                    s1 = MarkGen.DTable.Rows[row].ItemArray[2].ToString();
                    s2 = (string)comboBox1.Items[i];
                    if (String.Compare(s1, s2) == 0) kol++;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                GetTableNames();
                LoadParamm();
                comboBox1.Enabled = true;
                button2.Enabled = true;
                if (comboBox1.Items.Count != 0)
                    comboBox1.SelectedIndex = 0;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShowTable(SQL_AllTable("*"));
            GetTableColumns();
            GetManufactures();

            DrawShema();
            isLoadListMark = false;
        }

        private void DrawShema()
        {
            string SQLQuery = "select ShemaImg from Схема";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();
            MemoryStream ms = new MemoryStream((byte[])reader[0]);
            Image img = Image.FromStream(ms);
            pictureBox1.Image = img;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string SQLQuery = "select Max(Эпоха) from Данные ";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            string ms = command.ExecuteScalar().ToString();
            int maxCounter = Convert.ToInt32(ms);
            maxCounter++;
            string str = "\"Эпоха\",";
            string strValues = maxCounter + ",";
            for (int x = 1; x <= MarkGen.DTable.Columns.Count - 1; x++)
            {
                str += "\"" + x + "\",";
                strValues += CalkValues(x).ToString().Replace(",", ".") + ",";
            }
            strValues = strValues.Remove(strValues.Length - 1, 1);
            str = str.Remove(str.Length - 1, 1);
            SQLQuery = "insert into Данные (" + str + ") values(" + strValues + ")";
            command = new SQLiteCommand(SQLQuery, SQLiteConn);
            command.ExecuteNonQuery();

            ShowTable(SQL_AllTable("*"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                string SQLQuery = "delete from Данные where Эпоха = " + dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
                command.ExecuteNonQuery();
                ShowTable(SQL_AllTable("*"));
            }
        }

        private double CalkValues(int value)
        {
            double min = Convert.ToDouble(MarkGen.DTable.AsEnumerable().Min(x => x[value.ToString()]));
            double max = Convert.ToDouble(MarkGen.DTable.AsEnumerable().Max(x => x[value.ToString()]));
            Random ramdom = new Random();
            double newValue = min + (ramdom.NextDouble() * (max - min));
            return Math.Round(newValue, 4);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Вторая вкладка

        private void cbM_CheckedChanged(object sender, EventArgs e)
        {

            CheckBox cb = ((CheckBox)sender);
            switch (cb.Name)
            {
                case "cbMMinus":
                    if (cb.Checked)
                    {
                        MarkGen.ChangeValuesTable();
                        MarkGen.CalkM(MarkGen.DTableMinus);
                        MarkGen.CalkAlpha(MarkGen.DTableMinus);
                        Alpha();
                        DrawChartMMinus(MarkGen.ArrayM, MarkGen.ArrayAlpha);
                        ChartM3("M-", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridM();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series1"].Points.Clear();
                        chart3.Series["M-"].Points.Clear();
                    }
                    break;
                case "cbM":
                    if (cb.Checked)
                    {
                        MarkGen.CalkM(MarkGen.DTable);
                        MarkGen.CalkAlpha(MarkGen.DTable);
                        Alpha();
                        DrawChartM(MarkGen.ArrayM, MarkGen.ArrayAlpha);
                        ChartM3("M", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridM();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series2"].Points.Clear();
                        chart3.Series["M"].Points.Clear();
                    }
                    break;
                case "cbMPlus":
                    if (cb.Checked)
                    {
                        MarkGen.ChangeValuesTable();
                        MarkGen.CalkM(MarkGen.DTablePlus);
                        MarkGen.CalkAlpha(MarkGen.DTablePlus);
                        Alpha();
                        DrawChartMPlus(MarkGen.ArrayM, MarkGen.ArrayAlpha);
                        ChartM3("M+", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridM();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series3"].Points.Clear();
                        chart3.Series["M+"].Points.Clear();
                    }
                    break;
                case "cbMMinusProg":
                    if (cb.Checked)
                    {
                        MarkGen.ChangeValuesTable();
                        MarkGen.CalkM(MarkGen.DTableMinus);
                        MarkGen.CalkAlpha(MarkGen.DTableMinus);
                        Alpha();
                        MarkGen.CalkMProg();
                        MarkGen.CalkAlphaProg();
                        DrawChartMMinusProg(MarkGen.ArrayMProg, MarkGen.ArrayAlphaProg);
                        ChartMProg("M- прогноз", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridMProg();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series4"].Points.Clear();
                        chart3.Series["M- прогноз"].Points.Clear();
                    }
                    break;
                case "cbMProg":
                    if (cb.Checked)
                    {
                        MarkGen.CalkM(MarkGen.DTable);
                        MarkGen.CalkAlpha(MarkGen.DTable);
                        Alpha();
                        MarkGen.CalkMProg();
                        MarkGen.CalkAlphaProg();
                        DrawChartMProg(MarkGen.ArrayMProg, MarkGen.ArrayAlphaProg);
                        ChartMProg("M прогноз", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridMProg();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series5"].Points.Clear();
                        chart3.Series["M прогноз"].Points.Clear();
                    }
                    break;
                case "cbMPlusProg":
                    if (cb.Checked)
                    {
                        MarkGen.ChangeValuesTable();
                        MarkGen.CalkM(MarkGen.DTablePlus);
                        MarkGen.CalkAlpha(MarkGen.DTablePlus);
                        Alpha();
                        MarkGen.CalkMProg();
                        MarkGen.CalkAlphaProg();
                        DrawChartMPlusProg(MarkGen.ArrayMProg, MarkGen.ArrayAlphaProg);
                        ChartMProg("M+ прогноз", chart3, MarkGen.GetArrayEpoh(), MarkGen.ArrayM);
                        DrawGridMProg();
                        charto(chart1);
                        chartd(chart3);
                    }
                    else
                    {
                        chart1.Series["Series6"].Points.Clear();
                        chart3.Series["M+ прогноз"].Points.Clear();
                    }
                    break;
            }
        }

        private void Alpha()
        {
            for (int x = 0; x < MarkGen.ArrayMM.Length; x++)
            {
                if (comboBox1.SelectedIndex == 2)
                {
                    double i = MarkGen.ArrayAlpha[x] * Math.Pow(10, 6);
                    int res = Convert.ToInt32(i);
                    double result = res / Math.Pow(10, 7);
                    MarkGen.ArrayAlpha[x] = Convert.ToInt32(result);
                }
            }
        }

        private void charto(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            chart.ChartAreas[0].AxisX.Title = "M";
            chart.ChartAreas[0].AxisY.Title = "A";
        }

        private void chartd(System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            chart.ChartAreas[0].AxisX.Title = "Эпоха";
            chart.ChartAreas[0].AxisY.Title = "М";
        }

        private void FillCbSelectBlocks()
        {
            if (!isLoadListMark)
                ListMarks.Clear();
            cbSelectBlocks.Items.Clear();
            for (int x = 1; x <= CountBlocks; x++)
            {
                ItemNameValue item = new ItemNameValue(x, alphabet[x - 1].ToString());
                cbSelectBlocks.Items.Add(item);
                if (!isLoadListMark)
                    ListMarks.Add(new Marka(x - 1));
            }

            if (cbSelectBlocks.Items.Count != 0)
                cbSelectBlocks.SelectedIndex = 0;


        }


        private void DrawChartMMinus(double[] array1, double[] array2)
        {
            chart1.Series["Series1"].LegendText = "M-";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series1"].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series1"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        private void DrawChartM(double[] array1, double[] array2)
        {
            chart1.Series["Series2"].LegendText = "M";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series2"].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series2"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        private void DrawChartMPlus(double[] array1, double[] array2)
        {
            chart1.Series["Series3"].LegendText = "M+";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series3"].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series3"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        private void DrawChartMMinusProg(double[] array1, double[] array2)
        {
            chart1.Series["Series4"].LegendText = "M-";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series4"].Points.AddXY(array1[i], array2[i]);
                if (i == array1.Length - 1)
                    chart1.Series["Series4"].Points[i].MarkerColor = Color.Green;
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series4"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        private void DrawChartMProg(double[] array1, double[] array2)
        {
            chart1.Series["Series5"].LegendText = "M";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series5"].Points.AddXY(array1[i], array2[i]);
                if (i == array1.Length - 1)
                    chart1.Series["Series5"].Points[i].MarkerColor = Color.Yellow;
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series5"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        private void DrawChartMPlusProg(double[] array1, double[] array2)
        {
            chart1.Series["Series6"].LegendText = "M+";
            for (int i = 0; i < array1.Length; i++)
            {
                chart1.Series["Series6"].Points.AddXY(array1[i], array2[i]);
                if (i == array1.Length - 1)
                    chart1.Series["Series6"].Points[i].MarkerColor = Color.Black;
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart1.Series["Series6"].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
        }

        //private void ChartM(string nameSeries, System.Windows.Forms.DataVisualization.Charting.Chart chart, double[] array1, double[] array2)
        //{
        //   for (int i = 0; i < array1.Length; i++)
        // {
        //   chart.Series[nameSeries].Points.AddXY(array1[i], array2[i]);
        //    }
        //  for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
        //    {
        //      chart.Series[nameSeries].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
        //    }
        //  chart.ChartAreas[0].AxisY.Minimum = array2.Min();
        // chart.ChartAreas[0].AxisY.Maximum = array2.Max();
        //chart.ChartAreas[0].AxisX.Title = "Эпоха";
        //chart.ChartAreas[0].AxisY.Title = "M";
        //}

        private void ChartM3(string nameSeries, System.Windows.Forms.DataVisualization.Charting.Chart chart, double[] array1, double[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                chart.Series[nameSeries].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart.Series[nameSeries].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
            chart.ChartAreas[0].AxisY.Minimum = array2.Min() - 0.008;
            chart.ChartAreas[0].AxisY.Maximum = array2.Max() + 0.008;
        }

        private void ChartMProg(string nameSeries, System.Windows.Forms.DataVisualization.Charting.Chart chart, double[] array1, double[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                chart.Series[nameSeries].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart.Series[nameSeries].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
            chart.ChartAreas[0].AxisY.Minimum = array2.Min() - 0.008;
            chart.ChartAreas[0].AxisY.Maximum = array2.Max() + 0.008;
        }

        private void btChangeParamm_Click(object sender, EventArgs e)
        {
            SQLiteCommand command = new SQLiteCommand("update Схема set A = " + tbA2.Text.Replace(",", ".") + ", E = " + tbE2.Text.Replace(",", ".") + " where Id = 1", SQLiteConn);
            if (command.ExecuteNonQuery() == -1)
                MessageBox.Show("Параметры А и Е не сохранены. Попробуйте позже.");
            else
                MessageBox.Show("Параметры А и Е успешно сохранены");

            MarkGen.A = Convert.ToDouble(tbA2.Text);
            MarkGen.E = Convert.ToDouble(tbE2.Text);

            ClearFormThreetab1();
            DrawGridM();
        }

        private void ClearFormThreetab1()
        {
            foreach (Control ctrl in tabPage2.Controls.OfType<CheckBox>())
            {
                ((CheckBox)ctrl).Checked = false;
            }
        }

        private void DrawGridM()
        {
            dgvM.Rows.Clear();
            dgvMFault.Rows.Clear();
            MarkGen.CalkM(MarkGen.DTableMinus);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTableMinus);
            MarkGen.CalkAlphaProg();
            double[] arrMMinus = MarkGen.ArrayM;
            MarkGen.CalkM(MarkGen.DTable);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTable);
            MarkGen.CalkAlphaProg();
            double[] arrM = MarkGen.ArrayM;
            MarkGen.CalkM(MarkGen.DTablePlus);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTablePlus);
            MarkGen.CalkAlphaProg();
            double[] arrMPlus = MarkGen.ArrayM;
            double[] arrD = new double[MarkGen.ArrayM.Length];


            for (int x = 0; x < arrD.Length; x++)
                arrD[x] = arrMPlus[x] - arrMMinus[x];

            double[] arrR = new double[arrD.Length];
            for (int x = 0; x < arrR.Length; x++)
                arrR[x] = arrD[x] / 2;

            double[] arrL = new double[arrR.Length];
            for (int x = 1; x < arrL.Length; x++)
                arrL[x] = Math.Abs(arrM[0] - arrM[x]);

            dgvM.ColumnCount = 7;
            dgvM.Columns[0].HeaderText = "Эпоха";
            dgvM.Columns[1].HeaderText = "M-";
            dgvM.Columns[2].HeaderText = "M";
            dgvM.Columns[3].HeaderText = "M+";
            dgvM.Columns[4].HeaderText = "D";
            dgvM.Columns[5].HeaderText = "R";
            dgvM.Columns[6].HeaderText = "L";

            dgvMFault.ColumnCount = 2;
            dgvMFault.Columns[0].HeaderText = "Эпоха";
            dgvMFault.Columns[1].HeaderText = "Состояние";

            for (int x = 0; x < arrMMinus.Length; x++)
            {
                string[] arrRowM = new string[7];
                arrRowM[0] = MarkGen.DTable.Rows[x][0].ToString();
                arrRowM[1] = arrMMinus[x].ToString();
                arrRowM[2] = arrM[x].ToString();
                arrRowM[3] = arrMPlus[x].ToString();
                arrRowM[4] = arrD[x].ToString();
                arrRowM[5] = arrR[x].ToString();
                arrRowM[6] = arrL[x].ToString();
                dgvM.Rows.Add(arrRowM);


                string[] arrRowState = new string[2];
                if (x < MarkGen.DTable.Rows.Count)
                    arrRowState[0] = MarkGen.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrL[x] < arrR[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrL[x] > arrR[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrL[x] == arrR[x])
                    arrRowState[1] = "Предаварийное";

                dgvMFault.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.Gold;
                }
            }

        }

        private void DrawGridMProg()
        {
            dgvMFault.Rows.Clear();
            dgvMP.Rows.Clear();
            MarkGen.CalkM(MarkGen.DTableMinus);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTableMinus);
            MarkGen.CalkAlphaProg();

            double[] arrMMinusProg = MarkGen.ArrayMProg;
            double[] arrAlphaMinusProg = MarkGen.ArrayAlphaProg;
            MarkGen.CalkM(MarkGen.DTable);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTable);
            MarkGen.CalkAlphaProg();

            double[] arrMProg = MarkGen.ArrayMProg;
            double[] arrAlphaProg = MarkGen.ArrayAlphaProg;
            MarkGen.CalkM(MarkGen.DTablePlus);
            MarkGen.CalkMProg();
            MarkGen.CalkAlpha(MarkGen.DTablePlus);
            MarkGen.CalkAlphaProg();

            double[] arrMPlusProg = MarkGen.ArrayMProg;
            double[] arrAlphaPlusProg = MarkGen.ArrayAlphaProg;

            double[] arrDP = new double[MarkGen.ArrayMProg.Length];


            for (int x = 0; x < arrDP.Length; x++)
                arrDP[x] = arrMPlusProg[x] - arrMMinusProg[x];

            double[] arrRP = new double[arrDP.Length];
            for (int x = 0; x < arrRP.Length; x++)
                arrRP[x] = arrDP[x] / 2;

            double[] arrLP = new double[arrRP.Length];
            for (int x = 1; x < arrLP.Length; x++)
                arrLP[x] = Math.Abs(arrMProg[0] - arrMProg[x]);

            dgvMFault.ColumnCount = 2;
            dgvMFault.Columns[0].HeaderText = "Эпоха";
            dgvMFault.Columns[1].HeaderText = "Состояние";

            dgvMP.ColumnCount = 7;
            dgvMP.Columns[0].HeaderText = "Эпоха";
            dgvMP.Columns[1].HeaderText = "M-";
            dgvMP.Columns[2].HeaderText = "M";
            dgvMP.Columns[3].HeaderText = "M+";
            dgvMP.Columns[4].HeaderText = "AL-";
            dgvMP.Columns[5].HeaderText = "AL";
            dgvMP.Columns[6].HeaderText = "AL+";

            for (int x = 0; x < arrMProg.Length; x++)
            {

                string[] arrRowState = new string[2];
                if (x < MarkGen.DTable.Rows.Count)
                    arrRowState[0] = MarkGen.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrLP[x] < arrRP[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrLP[x] > arrRP[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrLP[x] == arrRP[x])
                    arrRowState[1] = "Предаварийное";

                dgvMFault.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dgvMFault[1, x].Style.BackColor = Color.Gold;
                }

                string[] arrRowMP = new string[7];
                if (x < MarkGen.DTable.Rows.Count)
                    arrRowMP[0] = MarkGen.DTable.Rows[x][0].ToString();
                else
                    arrRowMP[0] = "Прогноз";
                arrRowMP[1] = arrMMinusProg[x].ToString();
                arrRowMP[2] = arrMProg[x].ToString();
                arrRowMP[3] = arrMPlusProg[x].ToString();
                arrRowMP[4] = arrAlphaMinusProg[x].ToString();
                arrRowMP[5] = arrAlphaProg[x].ToString();
                arrRowMP[6] = arrAlphaPlusProg[x].ToString();
                dgvMP.Rows.Add(arrRowMP);


            }
        }




        #endregion

        #region Третья вкладка

        private void ClearFormThree()
        {
            foreach (Control ctrl in tabPage3.Controls.OfType<CheckBox>())
            {
                ((CheckBox)ctrl).Checked = false;
            }
        }

        private void cbSelectBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearFormThree();
            DrawListBox(lbRight, ListMarks[cbSelectBlocks.SelectedIndex].DTable);
            DrawTableBlock();
            bt1Add.Enabled = ValidationCountPointsAdd(lbRight);
            bt1Del.Enabled = ValidationCountPointsDel(lbRight);
            cbSelectBlocks.Enabled = true;
        }

        private void btSavePointToObj_Click(object sender, EventArgs e)
        {
            cbSelectBlocks.Enabled = true;
            string query = SQL_AllTable(GetStringArrColumns(lbRight));
            ListMarks[cbSelectBlocks.SelectedIndex] = new Marka(cbSelectBlocks.SelectedIndex, GetTableFromDB(query), GetTableFromDB(query), GetTableFromDB(query), Convert.ToDouble(tbA2.Text), Convert.ToDouble(tbE2.Text));
            DrawTableBlock();

            //пример, как брать объект
            // Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];

        }

        private string GetStringArrColumns(ListBox lb)
        {
            string columns = "Эпоха,";
            for (int x = 0; x < lb.Items.Count; x++)
            {
                ItemNameValue item = ((ItemNameValue)lb.Items[x]);
                columns += "[" + item.Id + "],";
            }
            columns = columns.Remove(columns.Length - 1, 1);
            return columns;
        }

        private void DrawTableBlock()
        {
            string query = SQL_AllTable(GetStringArrColumns(lbRight));
            DataTable dt = GetTableFromDB(query);

            dgvBlock.DataSource = dt;


            //dgvBlock.Rows.Clear();
            //dgvBlock.ColumnCount = dt.Columns.Count;


            //for (int x = 0; x < dt.Rows.Count; x++)
            //{
            //    string[] str = new string[dt.Columns.Count];
            //    for (int x = 0; x < dt.Rows.Count; x++)
            //}
        }

        private bool ValidationCountPointsAdd(ListBox lb)
        {
            cbSelectBlocks.Enabled = true;
            btSavePointToObj.Enabled = true;

            if (lb.Items.Count < 3)
                btSavePointToObj.Enabled = false;

            if (lb.Items.Count + 1 > (MarkGen.DTable.Columns.Count - 1) / cbSelectBlocks.Items.Count)
                return false;

            List<Marka> listNoEmpty = ListMarks.FindAll(x => x.DTable != null);
            if (listNoEmpty.Count != 0)
            {
                if (listNoEmpty.Count == 1 && cbSelectBlocks.SelectedIndex == listNoEmpty[0].Index)
                {
                }
                else
                {
                    if (lb.Items.Count + 1 > listNoEmpty[0].DTable.Columns.Count - 1)
                        return false;
                    if (lb.Items.Count != listNoEmpty[0].DTable.Columns.Count - 1)
                        btSavePointToObj.Enabled = false;
                }
            }



            return true;
        }

        private bool ValidationCountPointsDel(ListBox lb)
        {
            btSavePointToObj.Enabled = true;
            cbSelectBlocks.Enabled = false;



            if (lb.Items.Count < 3)
                btSavePointToObj.Enabled = false;

            if (lb.Items.Count == 0)
                return false;

            List<Marka> listNoEmpty = ListMarks.FindAll(x => x.DTable != null);
            if (listNoEmpty.Count != 0)
            {
                if (listNoEmpty.Count == 1 && cbSelectBlocks.SelectedIndex == listNoEmpty[0].Index)
                {
                }
                else
                {
                    if (lb.Items.Count != listNoEmpty[0].DTable.Columns.Count - 1)
                        btSavePointToObj.Enabled = false;
                }
            }

            //if (lb.Items.Count + 1 > (MarkGen.DTable.Columns.Count - 1) / cbSelectBlocks.Items.Count)
            //    return false;
            return true;
        }

        private void btRandomPoints_Click(object sender, EventArgs e)
        {
            RandomItems();
            btSavePointToObj.Enabled = false;
        }

        public void DrawListBox(ListBox lb, DataTable dt, bool isLoadListMarkLocal = false)
        {
            if (isLoadListMarkLocal)
            {
                return;
            }
            lb.Items.Clear();

            if (dt == null)
                return;

            for (int x = 1; x < dt.Columns.Count; x++)
            {
                ItemNameValue item = new ItemNameValue(Convert.ToInt32(dt.Columns[x].ColumnName), "Точка " + dt.Columns[x].ColumnName.ToString());
                lb.Items.Add(item);
            }

        }

        public void RandomItems()
        {
            ListBox[] arrLB = new ListBox[CountBlocks];


            DrawListBox(lbLeft, MarkGen.DTable);
            int maxPointBlock = (MarkGen.DTable.Columns.Count - 1) / CountBlocks;
            while (lbLeft.Items.Count > 1)
            {
                for (int x = 0; x < CountBlocks; x++)
                {
                    if (arrLB[x] == null)
                        arrLB[x] = new ListBox();
                    Random rnd = new Random();
                    int index = rnd.Next(0, lbLeft.Items.Count);
                    var item = lbLeft.Items[index];
                    arrLB[x].Items.Add(item);
                    lbLeft.Items.RemoveAt(index);
                }

            }

            for (int x = 0; x < CountBlocks; x++)
            {
                string query = SQL_AllTable(GetStringArrColumns(arrLB[x]));
                ListMarks[x] = new Marka(x, GetTableFromDB(query), GetTableFromDB(query), GetTableFromDB(query), Convert.ToDouble(tbA2.Text), Convert.ToDouble(tbE2.Text));
            }

            cbSelectBlocks_SelectedIndexChanged(cbSelectBlocks.SelectedItem, null);

            //DrawListBox(lbLeft, MarkGen.DTable);
            //bool flag = false;
            //while (lbLeft.Items.Count > 1)
            //{
            //    Random rnd = new Random();
            //    int index = rnd.Next(0, lbLeft.Items.Count);
            //    if (flag)
            //    {
            //        var item = lbLeft.Items[index];
            //        lbRight.Items.Add(item);
            //        lbLeft.Items.RemoveAt(index);
            //    }
            //    else
            //    {
            //        var item2 = lbLeft.Items[index];
            //        lbRight2.Items.Add(item2);
            //        lbLeft.Items.RemoveAt(index);
            //    }
            //    flag = !flag;
            //    Thread.Sleep(200);
            //    Application.DoEvents();
            //}
        }

        private void CbM_raschet(object sender, EventArgs e)
        {

            CheckBox cb = ((CheckBox)sender);
            switch (cb.Name)
            {
                case "cbminus":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTableMinus);
                        currentMark.CalkAlpha(currentMark.DTableMinus);
                        DrawChartMM("M-", chart4, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M-", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMM();
                        charto(chart4);
                        chartd(chart5);


                    }
                    else
                    {
                        chart4.Series["M-"].Points.Clear();
                        chart5.Series["M-"].Points.Clear();
                    }
                    break;
                case "cbmm":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.CalkM(currentMark.DTable);
                        currentMark.CalkAlpha(currentMark.DTable);
                        DrawChartMM("M", chart4, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMM();
                        charto(chart4);
                        chartd(chart5);
                    }
                    else
                    {
                        chart4.Series["M"].Points.Clear();
                        chart5.Series["M"].Points.Clear();
                    }
                    break;
                case "cbplus":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTablePlus);
                        currentMark.CalkAlpha(currentMark.DTablePlus);
                        DrawChartMM("M+", chart4, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M+", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMM();
                        charto(chart4);
                        chartd(chart5);
                    }
                    else
                    {
                        chart4.Series["M+"].Points.Clear();
                        chart5.Series["M+"].Points.Clear();
                    }
                    break;
                case "cbminusp":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTableMinus);
                        currentMark.CalkAlpha(currentMark.DTableMinus);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("M- прогноз", chart4, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("М- прогноз", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab2();
                        charto(chart4);
                        chartd(chart5);
                    }
                    else
                    {
                        chart4.Series["M- прогноз"].Points.Clear();
                        chart5.Series["М- прогноз"].Points.Clear();
                    }
                    break;
                case "cbmmp":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.CalkM(currentMark.DTable);
                        currentMark.CalkAlpha(currentMark.DTable);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("M прогноз", chart4, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("М прогноз", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab2();
                        charto(chart4);
                        chartd(chart5);
                    }
                    else
                    {
                        chart4.Series["M прогноз"].Points.Clear();
                        chart5.Series["М прогноз"].Points.Clear();
                    }
                    break;
                case "cbplisp":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTablePlus);
                        currentMark.CalkAlpha(currentMark.DTablePlus);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("M+ прогноз", chart4, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("M+ прогноз", chart5, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab2();
                        charto(chart4);
                        chartd(chart5);
                    }
                    else
                    {
                        chart4.Series["M+ прогноз"].Points.Clear();
                        chart5.Series["M+ прогноз"].Points.Clear();
                    }
                    break;
            }
        }

        private void DrawChartMM(string nameSeries, System.Windows.Forms.DataVisualization.Charting.Chart chart, double[] array1, double[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                chart.Series[nameSeries].Points.AddXY(array1[i], array2[i]);
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart.Series[nameSeries].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }
            chart5.ChartAreas[0].AxisY.Minimum = array2.Min() - 0.008;
            chart5.ChartAreas[0].AxisY.Maximum = array2.Max() + 0.008;

            chart6.ChartAreas[0].AxisY.Minimum = array2.Min() - 0.008;
            chart6.ChartAreas[0].AxisY.Maximum = array2.Max() + 0.008;
        }

        private void DrawChartMMProg(string nameSeries, System.Windows.Forms.DataVisualization.Charting.Chart chart, double[] array1, double[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                chart.Series[nameSeries].Points.AddXY(array1[i], array2[i]);
                if (i == array1.Length - 1)
                    chart.Series[nameSeries].Points[i].MarkerColor = Color.Yellow;
            }
            for (int i = 0; i < MarkGen.DTable.Rows.Count; i++)
            {
                chart.Series[nameSeries].Points[i].Label = MarkGen.DTable.Rows[i][0].ToString();
            }

        }

        private void DrawGridMM()
        {
            dgvvz.Rows.Clear();
            dgvv.Rows.Clear();
            Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
            currentMark.CalkM(currentMark.DTableMinus);

            currentMark.CalkAlpha(currentMark.DTableMinus);

            double[] arrMMinus = currentMark.ArrayM;
            currentMark.CalkM(currentMark.DTable);

            currentMark.CalkAlpha(currentMark.DTable);

            double[] arrM = currentMark.ArrayM;
            currentMark.CalkM(currentMark.DTablePlus);

            currentMark.CalkAlpha(currentMark.DTablePlus);

            double[] arrMPlus = currentMark.ArrayM;
            double[] arrD = new double[currentMark.ArrayM.Length];


            for (int x = 0; x < arrD.Length; x++)
                arrD[x] = arrMPlus[x] - arrMMinus[x];

            double[] arrR = new double[arrD.Length];
            for (int x = 0; x < arrR.Length; x++)
                arrR[x] = arrD[x] / 2;

            double[] arrL = new double[arrR.Length];
            for (int x = 1; x < arrL.Length; x++)
                arrL[x] = Math.Abs(arrM[0] - arrM[x]);

            dgvvz.ColumnCount = 7;
            dgvvz.Columns[0].HeaderText = "Эпоха";
            dgvvz.Columns[1].HeaderText = "M-";
            dgvvz.Columns[2].HeaderText = "M";
            dgvvz.Columns[3].HeaderText = "M+";
            dgvvz.Columns[4].HeaderText = "D";
            dgvvz.Columns[5].HeaderText = "R";
            dgvvz.Columns[6].HeaderText = "L";

            dgvv.ColumnCount = 2;
            dgvv.Columns[0].HeaderText = "Эпоха";
            dgvv.Columns[1].HeaderText = "Состояние";

            for (int x = 0; x < arrMMinus.Length; x++)
            {
                string[] arrRowM = new string[7];
                arrRowM[0] = currentMark.DTable.Rows[x][0].ToString();
                arrRowM[1] = arrMMinus[x].ToString();
                arrRowM[2] = arrM[x].ToString();
                arrRowM[3] = arrMPlus[x].ToString();
                arrRowM[4] = arrD[x].ToString();
                arrRowM[5] = arrR[x].ToString();
                arrRowM[6] = arrL[x].ToString();
                dgvvz.Rows.Add(arrRowM);


                string[] arrRowState = new string[2];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowState[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrL[x] < arrR[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrL[x] > arrR[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrL[x] == arrR[x])
                    arrRowState[1] = "Предаварийное";

                dgvv.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dgvv[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dgvv[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dgvv[1, x].Style.BackColor = Color.Gold;
                }
            }

        }

        private void DrawGridMProgTab2()
        {
            dgvv.Rows.Clear();
            dataGridView4.Rows.Clear();
            Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
            currentMark.CalkM(currentMark.DTableMinus);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTableMinus);
            currentMark.CalkAlphaProg();

            double[] arrMMinusProg = currentMark.ArrayMProg;
            double[] arrAlphaMinusProg = currentMark.ArrayAlphaProg;
            currentMark.CalkM(currentMark.DTable);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTable);
            currentMark.CalkAlphaProg();

            double[] arrMProg = currentMark.ArrayMProg;
            double[] arrAlphaProg = currentMark.ArrayAlphaProg;
            currentMark.CalkM(currentMark.DTablePlus);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTablePlus);
            currentMark.CalkAlphaProg();

            double[] arrMPlusProg = currentMark.ArrayMProg;
            double[] arrAlphaPlusProg = currentMark.ArrayAlphaProg;

            double[] arrDP = new double[currentMark.ArrayMProg.Length];


            for (int x = 0; x < arrDP.Length; x++)
                arrDP[x] = arrMPlusProg[x] - arrMMinusProg[x];

            double[] arrRP = new double[arrDP.Length];
            for (int x = 0; x < arrRP.Length; x++)
                arrRP[x] = arrDP[x] / 2;

            double[] arrLP = new double[arrRP.Length];
            for (int x = 1; x < arrLP.Length; x++)
                arrLP[x] = Math.Abs(arrMProg[0] - arrMProg[x]);

            dgvv.ColumnCount = 2;
            dgvv.Columns[0].HeaderText = "Эпоха";
            dgvv.Columns[1].HeaderText = "Состояние";

            dataGridView4.ColumnCount = 7;
            dataGridView4.Columns[0].HeaderText = "Эпоха";
            dataGridView4.Columns[1].HeaderText = "M-";
            dataGridView4.Columns[2].HeaderText = "M";
            dataGridView4.Columns[3].HeaderText = "M+";
            dataGridView4.Columns[4].HeaderText = "AL-";
            dataGridView4.Columns[5].HeaderText = "AL";
            dataGridView4.Columns[6].HeaderText = "AL+";

            for (int x = 0; x < arrMProg.Length; x++)
            {

                string[] arrRowState = new string[2];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowState[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrLP[x] < arrRP[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrLP[x] > arrRP[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrLP[x] == arrRP[x])
                    arrRowState[1] = "Предаварийное";

                dgvv.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dgvv[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dgvv[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dgvv[1, x].Style.BackColor = Color.Gold;
                }

                string[] arrRowMP = new string[7];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowMP[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowMP[0] = "Прогноз";
                arrRowMP[1] = arrMMinusProg[x].ToString();
                arrRowMP[2] = arrMProg[x].ToString();
                arrRowMP[3] = arrMPlusProg[x].ToString();
                arrRowMP[4] = arrAlphaMinusProg[x].ToString();
                arrRowMP[5] = arrAlphaProg[x].ToString();
                arrRowMP[6] = arrAlphaPlusProg[x].ToString();
                dataGridView4.Rows.Add(arrRowMP);


            }
        }



        #endregion

        #region Четвертая вкладка

        private bool ValidationCountPointsAddTab3(ListBox lb)
        {
            cbSubBlocksTab3.Enabled = true;
            btSaveTab3.Enabled = true;

            if (lb.Items.Count < 3)
                btSaveTab3.Enabled = false;

            List<Marka> listNoEmpty = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks.FindAll(x => x.DTable != null);
            if (listNoEmpty.Count != 0)
            {
                if (listNoEmpty.Count == 1 && cbSubBlocksTab3.SelectedIndex == listNoEmpty[0].Index)
                {
                }
                else
                {
                    if (lb.Items.Count + 1 > listNoEmpty[0].DTable.Columns.Count - 1)
                        return false;
                    if (lb.Items.Count != listNoEmpty[0].DTable.Columns.Count - 1)
                        btSaveTab3.Enabled = false;
                }
            }



            return true;
        }

        private bool ValidationCountPointsDelTab3(ListBox lb)
        {
            btSaveTab3.Enabled = true;
            cbSubBlocksTab3.Enabled = false;

            if (lb.Items.Count < 3)
                btSaveTab3.Enabled = false;

            if (lb.Items.Count == 0)
                return false;

            List<Marka> listNoEmpty = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks.FindAll(x => x.DTable != null);
            if (listNoEmpty.Count != 0)
            {
                if (listNoEmpty.Count == 1 && cbSubBlocksTab3.SelectedIndex == listNoEmpty[0].Index)
                {
                }
                else
                {
                    if (lb.Items.Count != listNoEmpty[0].DTable.Columns.Count - 1)
                        btSaveTab3.Enabled = false;
                }
            }

            //if (lb.Items.Count + 1 > (MarkGen.DTable.Columns.Count - 1) / cbSelectBlocks.Items.Count)
            //    return false;
            return true;
        }

        private void CbM_raschetTab3(object sender, EventArgs e)
        {

            CheckBox cb = ((CheckBox)sender);
            switch (cb.Name)
            {
                case "checkBox6":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTableMinus);
                        currentMark.CalkAlpha(currentMark.DTableMinus);
                        DrawChartMM("M-", chart2, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M-", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMMTab3();
                        charto(chart2);
                        chartd(chart6);


                    }
                    else
                    {
                        chart2.Series["M-"].Points.Clear();
                        chart6.Series["M-"].Points.Clear();
                    }
                    break;
                case "checkBox5":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.CalkM(currentMark.DTable);
                        currentMark.CalkAlpha(currentMark.DTable);
                        DrawChartMM("M", chart2, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMMTab3();
                        charto(chart2);
                        chartd(chart6);
                    }
                    else
                    {
                        chart2.Series["M"].Points.Clear();
                        chart6.Series["M"].Points.Clear();
                    }
                    break;
                case "checkBox4":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTablePlus);
                        currentMark.CalkAlpha(currentMark.DTablePlus);
                        DrawChartMM("M+", chart2, currentMark.ArrayM, currentMark.ArrayAlpha);
                        ChartM3("M+", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMMTab3();
                        charto(chart2);
                        chartd(chart6);
                    }
                    else
                    {
                        chart2.Series["M+"].Points.Clear();
                        chart6.Series["M+"].Points.Clear();
                    }
                    break;
                case "checkBox3":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTableMinus);
                        currentMark.CalkAlpha(currentMark.DTableMinus);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("М- прогноз", chart2, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("М- прогноз", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab3();
                        charto(chart2);
                        chartd(chart6);
                    }
                    else
                    {
                        chart2.Series["М- прогноз"].Points.Clear();
                        chart6.Series["М- прогноз"].Points.Clear();
                    }
                    break;
                case "checkBox2":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.CalkM(currentMark.DTable);
                        currentMark.CalkAlpha(currentMark.DTable);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("М прогноз", chart2, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("М прогноз", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab3();
                        charto(chart2);
                        chartd(chart6);
                    }
                    else
                    {
                        chart2.Series["М прогноз"].Points.Clear();
                        chart6.Series["М прогноз"].Points.Clear();
                    }
                    break;
                case "checkBox1":
                    if (cb.Checked)
                    {
                        Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
                        currentMark.ChangeValuesTable();
                        currentMark.CalkM(currentMark.DTablePlus);
                        currentMark.CalkAlpha(currentMark.DTablePlus);
                        currentMark.CalkMProg();
                        currentMark.CalkAlphaProg();
                        DrawChartMMProg("M+ прогноз", chart2, currentMark.ArrayMProg, currentMark.ArrayAlphaProg);
                        DrawChartMM("M+ прогноз", chart6, MarkGen.GetArrayEpoh(), currentMark.ArrayM);
                        DrawGridMProgTab3();
                        charto(chart2);
                        chartd(chart6);
                    }
                    else
                    {
                        chart2.Series["M+ прогноз"].Points.Clear();
                        chart6.Series["M+ прогноз"].Points.Clear();
                    }
                    break;
            }
        }

        private void DrawGridMMTab3()
        {
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            Marka currentMark = ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex];
            currentMark.CalkM(currentMark.DTableMinus);
            currentMark.CalkAlpha(currentMark.DTableMinus);
            double[] arrMMinus = currentMark.ArrayM;
            currentMark.CalkM(currentMark.DTable);
            currentMark.CalkAlpha(currentMark.DTable);
            double[] arrM = currentMark.ArrayM;
            currentMark.CalkM(currentMark.DTablePlus);
            currentMark.CalkAlpha(currentMark.DTablePlus);
            double[] arrMPlus = currentMark.ArrayM;
            double[] arrD = new double[currentMark.ArrayM.Length];

            for (int x = 0; x < arrD.Length; x++)
                arrD[x] = arrMPlus[x] - arrMMinus[x];

            double[] arrR = new double[arrD.Length];
            for (int x = 0; x < arrR.Length; x++)
                arrR[x] = arrD[x] / 2;

            double[] arrL = new double[arrR.Length];
            for (int x = 1; x < arrL.Length; x++)
                arrL[x] = Math.Abs(arrM[0] - arrM[x]);

            dataGridView2.ColumnCount = 7;
            dataGridView2.Columns[0].HeaderText = "Эпоха";
            dataGridView2.Columns[1].HeaderText = "M-";
            dataGridView2.Columns[2].HeaderText = "M";
            dataGridView2.Columns[3].HeaderText = "M+";
            dataGridView2.Columns[4].HeaderText = "D";
            dataGridView2.Columns[5].HeaderText = "R";
            dataGridView2.Columns[6].HeaderText = "L";

            dataGridView3.ColumnCount = 2;
            dataGridView3.Columns[0].HeaderText = "Эпоха";
            dataGridView3.Columns[1].HeaderText = "Состояние";

            for (int x = 0; x < arrMMinus.Length; x++)
            {
                string[] arrRowM = new string[7];
                arrRowM[0] = currentMark.DTable.Rows[x][0].ToString();
                arrRowM[1] = arrMMinus[x].ToString();
                arrRowM[2] = arrM[x].ToString();
                arrRowM[3] = arrMPlus[x].ToString();
                arrRowM[4] = arrD[x].ToString();
                arrRowM[5] = arrR[x].ToString();
                arrRowM[6] = arrL[x].ToString();
                dataGridView2.Rows.Add(arrRowM);


                string[] arrRowState = new string[2];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowState[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrL[x] < arrR[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrL[x] > arrR[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrL[x] == arrR[x])
                    arrRowState[1] = "Предаварийное";

                dataGridView3.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.Gold;
                }
            }

        }

        private void DrawGridMProgTab3()
        {
            dataGridView3.Rows.Clear();
            dataGridView5.Rows.Clear();
            Marka currentMark = ListMarks[cbSelectBlocks.SelectedIndex];
            currentMark.CalkM(currentMark.DTableMinus);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTableMinus);
            currentMark.CalkAlphaProg();

            double[] arrMMinusProg = currentMark.ArrayMProg;
            double[] arrAlphaMinusProg = currentMark.ArrayAlphaProg;
            currentMark.CalkM(currentMark.DTable);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTable);
            currentMark.CalkAlphaProg();

            double[] arrMProg = currentMark.ArrayMProg;
            double[] arrAlphaProg = currentMark.ArrayAlphaProg;
            currentMark.CalkM(currentMark.DTablePlus);
            currentMark.CalkMProg();
            currentMark.CalkAlpha(currentMark.DTablePlus);
            currentMark.CalkAlphaProg();

            double[] arrMPlusProg = currentMark.ArrayMProg;
            double[] arrAlphaPlusProg = currentMark.ArrayAlphaProg;

            double[] arrDP = new double[currentMark.ArrayMProg.Length];


            for (int x = 0; x < arrDP.Length; x++)
                arrDP[x] = arrMPlusProg[x] - arrMMinusProg[x];

            double[] arrRP = new double[arrDP.Length];
            for (int x = 0; x < arrRP.Length; x++)
                arrRP[x] = arrDP[x] / 2;

            double[] arrLP = new double[arrRP.Length];
            for (int x = 1; x < arrLP.Length; x++)
                arrLP[x] = Math.Abs(arrMProg[0] - arrMProg[x]);

            dataGridView3.ColumnCount = 2;
            dataGridView3.Columns[0].HeaderText = "Эпоха";
            dataGridView3.Columns[1].HeaderText = "Состояние";

            dataGridView5.ColumnCount = 7;
            dataGridView5.Columns[0].HeaderText = "Эпоха";
            dataGridView5.Columns[1].HeaderText = "M-";
            dataGridView5.Columns[2].HeaderText = "M";
            dataGridView5.Columns[3].HeaderText = "M+";
            dataGridView5.Columns[4].HeaderText = "AL-";
            dataGridView5.Columns[5].HeaderText = "AL";
            dataGridView5.Columns[6].HeaderText = "AL+";

            for (int x = 0; x < arrMProg.Length; x++)
            {

                string[] arrRowState = new string[2];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowState[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowState[0] = "Прогноз";
                if (arrLP[x] < arrRP[x])
                {
                    arrRowState[1] = "Стабильное";
                }
                else if (arrLP[x] > arrRP[x])
                    arrRowState[1] = "Не стабильное";
                else if (arrLP[x] == arrRP[x])
                    arrRowState[1] = "Предаварийное";

                dataGridView3.Rows.Add(arrRowState);

                if (arrRowState[1] == "Стабильное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.PaleGreen;
                }

                if (arrRowState[1] == "Не стабильное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.OrangeRed;
                }

                if (arrRowState[1] == "Предаварийное")
                {
                    dataGridView3[1, x].Style.BackColor = Color.Gold;
                }

                string[] arrRowMP = new string[7];
                if (x < currentMark.DTable.Rows.Count)
                    arrRowMP[0] = currentMark.DTable.Rows[x][0].ToString();
                else
                    arrRowMP[0] = "Прогноз";
                arrRowMP[1] = arrMMinusProg[x].ToString();
                arrRowMP[2] = arrMProg[x].ToString();
                arrRowMP[3] = arrMPlusProg[x].ToString();
                arrRowMP[4] = arrAlphaMinusProg[x].ToString();
                arrRowMP[5] = arrAlphaProg[x].ToString();
                arrRowMP[6] = arrAlphaPlusProg[x].ToString();
                dataGridView5.Rows.Add(arrRowMP);


            }
        }
        private void cbBlocksTab3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemNameValue item = ((ItemNameValue)cbBlocksTab3.SelectedItem);

            DrawListBox(lbLeftTab3, ListMarks[item.Id].DTable);

            CalcAndChangeCountBlocks(ListMarks[item.Id].DTable.Columns.Count - 1);

            DrawDgvTab3();

            cbSubBlocksTab3_SelectedIndexChanged(cbSubBlocksTab3, new EventArgs());

            DeleteItemsListBox(lbLeftTab3);
            ClearFormThreetab();
        }

        private void ClearFormThreetab()
        {
            foreach (Control ctrl in tabPage7.Controls.OfType<CheckBox>())
            {
                ((CheckBox)ctrl).Checked = false;
            }
        }

        private void cbSubBlocksTab3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks.Count != 0)
                DrawListBox(lbRigtTab3, ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex].DTable);
            DrawTableBlockTab3(lbRigtTab3);

            btAddTab3.Enabled = ValidationCountPointsAddTab3(lbRigtTab3);
            btDelTab3.Enabled = ValidationCountPointsDelTab3(lbRigtTab3);
            cbSubBlocksTab3.Enabled = true;
            ClearFormThreetab();
        }

        private void nudCountSunBlocka_ValueChanged(object sender, EventArgs e)
        {
            ItemNameValue item = ((ItemNameValue)cbBlocksTab3.SelectedItem);
            DrawListBox(lbLeftTab3, ListMarks[item.Id].DTable);



            DrawCbSubBlocks();

            ReCreateSubLists();

            DrawListBox(lbRigtTab3, ListMarks[item.Id].listSubMarks[cbSubBlocksTab3.SelectedIndex].DTable);

            DeleteItemsListBox(lbLeftTab3);
        }

        public void DeleteItemsListBox(ListBox lb, bool isLoadListMarkLocal = false)
        {
            if (isLoadListMarkLocal)
            {
                return;
            }

            ItemNameValue item = ((ItemNameValue)cbBlocksTab3.SelectedItem);
            for (int x = 0; x < ListMarks[item.Id].listSubMarks.Count; x++)
            {
                if (ListMarks[item.Id].listSubMarks[x].DTable == null)
                    continue;
                for (int y = 1; y < ListMarks[item.Id].listSubMarks[x].DTable.Columns.Count; y++)
                {
                    for (int indexLb = 0; indexLb < lb.Items.Count; indexLb++)
                    {
                        ItemNameValue itemCurrent = ((ItemNameValue)lb.Items[indexLb]);
                        int indexColumns = Convert.ToInt32(ListMarks[item.Id].listSubMarks[x].DTable.Columns[y].ColumnName);
                        if (itemCurrent.Id == indexColumns)
                            lb.Items.Remove(itemCurrent);
                    }
                    //ItemNameValue item = new ItemNameValue(Convert.ToInt32(dt.Columns[x].ColumnName), "Точка " + dt.Columns[x].ColumnName.ToString());
                    //lb.Items.Add(item);
                }
            }



        }

        private void btSaveTab3_Click(object sender, EventArgs e)
        {
            cbSubBlocksTab3.Enabled = true;
            string query = SQL_AllTable(GetStringArrColumns(lbRigtTab3));
            ListMarks[cbBlocksTab3.SelectedIndex].listSubMarks[cbSubBlocksTab3.SelectedIndex] = new Marka(cbBlocksTab3.SelectedIndex, GetTableFromDB(query), GetTableFromDB(query), GetTableFromDB(query), Convert.ToDouble(tbA2.Text), Convert.ToDouble(tbE2.Text));
            DrawTableBlockTab3(lbRigtTab3);
        }

        private void FillCbSelectBlocksTab3()
        {
            cbBlocksTab3.Items.Clear();
            //char[] arr = Enumerable.Range(0, 32).Select((x, i) => (char)("A" + i)).ToArray();
            //char[] arr = Enumerable.Range(0, 32).Select((x, i) => (char)("A" + i)).ToArray();


            for (int x = 0; x < CountBlocks; x++)
            {
                //cbBlocksTab3.Items.Add(new ItemNameValue("Блок" + arr[x], x));
                ItemNameValue item = new ItemNameValue(x, alphabet[x].ToString());
                cbBlocksTab3.Items.Add(item);
            }

            if (cbBlocksTab3.Items.Count != 0)
                cbBlocksTab3.SelectedIndex = 0;


        }

        private void DrawCbSubBlocks()
        {
            cbSubBlocksTab3.Items.Clear();
            for (int x = 0; x < nudCountSunBlocka.Value; x++)
            {
                ItemNameValue item = new ItemNameValue(x, (x + 1).ToString());
                cbSubBlocksTab3.Items.Add(item);
            }

            if (cbSubBlocksTab3.Items.Count != 0)
                cbSubBlocksTab3.SelectedIndex = 0;
        }

        private void ReCreateSubLists()
        {
            List<Marka> listTemp = ListMarks.FindAll(x => x.listSubMarks.Count != nudCountSunBlocka.Value);
            if (listTemp.Count > 0)
            {
                for (int x = 0; x < CountBlocks; x++)
                {
                    ListMarks[x].listSubMarks.Clear();
                    for (int subBlock = 0; subBlock < nudCountSunBlocka.Value; subBlock++)
                    {
                        ListMarks[x].listSubMarks.Add(new Marka(subBlock));
                    }
                }
            }

        }

        private void CalcAndChangeCountBlocks(int countPoints)
        {
            nudCountSunBlocka.Maximum = countPoints / 3;
        }

        private void DrawTableBlockTab3(ListBox lb)
        {
            string query = SQL_AllTable(GetStringArrColumns(lb));
            DataTable dt = GetTableFromDB(query);

            dgvPointsSelectBlock.DataSource = dt;
        }

        private void DrawDgvTab3()
        {
            int counter = 0;
            int counterShift = 2;
            DataTable dtRef = new DataTable();
            for (int row = 0; row < ListMarks[cbBlocksTab3.SelectedIndex].DTable.Rows.Count; row++)
            {
                DataRow dr = dtRef.NewRow();
                dtRef.Rows.Add(dr);
                for (int col = 2; col < ListMarks[cbBlocksTab3.SelectedIndex].DTable.Columns.Count; col++)
                {
                    if (row == 0)
                    {
                        DataColumn dc = new DataColumn(ListMarks[cbBlocksTab3.SelectedIndex].DTable.Columns[counterShift - 1].ColumnName + "-" + ListMarks[cbBlocksTab3.SelectedIndex].DTable.Columns[col].ColumnName, typeof(double));
                        dtRef.Columns.Add(dc);
                    }
                    double res = Convert.ToDouble(ListMarks[cbBlocksTab3.SelectedIndex].DTable.Rows[row][counterShift - 1]) - Convert.ToDouble(ListMarks[cbBlocksTab3.SelectedIndex].DTable.Rows[row][col]);

                    dtRef.Rows[row][counter] = Math.Abs(res);

                    if (col + 1 == ListMarks[cbBlocksTab3.SelectedIndex].DTable.Columns.Count && counterShift + 1 < ListMarks[cbBlocksTab3.SelectedIndex].DTable.Columns.Count)
                    {
                        counterShift++;
                        col = counterShift - 1;
                    }
                    counter++;
                }

                counter = 0;
                counterShift = 2;
            }
            dgvReferenseTab3.DataSource = dtRef;

            DataTable dtStability = new DataTable();
            for (int col = 0; col < dtRef.Columns.Count; col++)
            {
                DataColumn dc = new DataColumn(dtRef.Columns[col].ColumnName, typeof(double));
                dtStability.Columns.Add(dc);
                for (int row = 0; row < dtRef.Rows.Count; row++)
                {
                    if (col == 0)
                    {
                        DataRow dr = dtStability.NewRow();
                        dtStability.Rows.Add(dr);
                    }
                    double res = Convert.ToDouble(dtRef.Rows[0][col]) - Convert.ToDouble(dtRef.Rows[row][col]);
                    dtStability.Rows[row][col] = res;

                }
            }

            double e = Convert.ToDouble(textBox1.Text);
            dgvStabilityTab3.ColumnCount = dtStability.Columns.Count;
            dgvStabilityTab3.RowCount = dtStability.Rows.Count;
           

            for (int col = 0; col < dtRef.Columns.Count; col++)
            {
                for (int row = 0; row < dtRef.Rows.Count; row++)
                {
                    if (Convert.ToDouble(dtStability.Rows[row][col]) < e)
                    {
                        dgvStabilityTab3.Rows[row].Cells[col].Value = "Стабильная";
                        dgvStabilityTab3.Rows[row].Cells[col].Style.BackColor = Color.PaleGreen;
                    }
                    else if (Convert.ToDouble(dtStability.Rows[row][col]) > e)
                    {
                        dgvStabilityTab3.Rows[row].Cells[col].Value = "Не стабильная";
                        dgvStabilityTab3.Rows[row].Cells[col].Style.BackColor = Color.OrangeRed;
                    }
                    else
                    {
                        dgvStabilityTab3.Rows[row].Cells[col].Value = "Пред аварийная";
                        dgvStabilityTab3.Rows[row].Cells[col].Style.BackColor = Color.Gold;
                    }
                }
            }
        }


        #endregion

        #region Пятая вкладка

        private void btApplyFive_Click(object sender, EventArgs e)
        {
            LoadDataFive();
            DrawDGVFive();
            DrawChartFive();
        }

        public void LoadDataFive()
        {
            string query = SQL_AllTable(GetStringArrColumns(lbRightFive));
            MarkFive = new Marka(0, GetTableFromDB(query), GetTableFromDB(query), GetTableFromDB(query), Convert.ToDouble(tbA2.Text), Convert.ToDouble(tbE2.Text));
        }

        public void DrawDGVFive()
        {
            dgvFive.DataSource = MarkFive.DTable;
        }

        public void DrawChartFive()
        {
            double[] arrBorder = new double[(MarkFive.DTable.Columns.Count - 1) * MarkFive.DTable.Rows.Count];
            int counter = 0;
            chartFive.Series.Clear();
            for (int x = 0; x < MarkFive.DTable.Columns.Count - 1; x++)
            {
                Series currentSeries = new Series("Точка " + MarkFive.DTable.Columns[x + 1].ColumnName);
                currentSeries.ChartType = SeriesChartType.Spline;
                for (int y = 0; y < MarkFive.DTable.Rows.Count; y++)
                {
                    currentSeries.Points.AddXY(Convert.ToDouble(MarkFive.DTable.Rows[y][0]), Convert.ToDouble(MarkFive.DTable.Rows[y][x + 1]));
                    arrBorder[counter] = Convert.ToDouble(MarkFive.DTable.Rows[y][x + 1]);
                    counter++;
                }
                chartFive.Series.Add(currentSeries);
            }
            chartFive.ChartAreas[0].AxisY.Minimum = arrBorder.Min() - 0.008;
            chartFive.ChartAreas[0].AxisY.Maximum = arrBorder.Max() + 0.008;
        }




        #endregion

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chart5_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(dgvv[1, dgvv.Rows.Count - 1].Value) == "Не стабильное")
            {
                MessageBox.Show("Вам следует перейти на третий уровень. Прогнозное состояние объекта не стабильное.");
            }
            else
                MessageBox.Show("Переход на третий уровень не требуется. Прогнозное состояние объекта стабильное.");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(dataGridView3[1, dataGridView3.Rows.Count - 1].Value) == "Не стабильное")
            {
                MessageBox.Show("Вам следует перейти на четвертый уровень. Прогнозное состояние объекта не стабильное.");
            }
            else
                MessageBox.Show("Переход на четвертый уровень не требуется. Прогнозное состояние объекта стабильное.");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tabPage2.Parent = tabControl1;
            this.tabControl1.SelectedIndex = 1;

            ClearFormThreetab1();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabPage3.Parent = tabControl1;
            this.tabControl1.SelectedIndex = 2;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabPage4.Parent = tabControl1;
            this.tabControl1.SelectedIndex = 3;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabPage5.Parent = tabControl1;
            this.tabControl1.SelectedIndex = 4;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DrawDgvTab3();
        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }
    }
    }
