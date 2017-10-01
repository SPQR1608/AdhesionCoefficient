using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AdhesionCoefficient
{
    public partial class Form1 : Form
    {
        SqlConnection conBD = new SqlConnection(@"data source=PC;initial catalog=AdhesionCoeff;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
        SqlCommand cmdBD = new SqlCommand();
        SqlDataReader readBD;
        
        string idLocoType = string.Empty;
        List<double> dMassSpeed = new List<double>();
        List<double> coeffMass = new List<double>();
        List<double> initCoeffMass = new List<double>();
        public Form1()
        {
            InitializeComponent();
        }

        private void DinamicChecked()
        {
            Speed.Location = new System.Drawing.Point(187, 33);
            Speed.Enabled = true;
            finalSpeed.Enabled = true;
            Step.Enabled = true;
            label5.Text = "Начальная скорость";
            label4.Location = new System.Drawing.Point(293, 36);
            VariablesGroupBox.Size = new System.Drawing.Size(366, 165);
            label9.Enabled = false;
            textBox1.Enabled = false;
            Speed.Clear();
            textBox1.Clear();
        }

        private void StaticChecked()
        {
            Speed.Location = new System.Drawing.Point(118, 33);
            Speed.Enabled = true;
            label4.Location = new System.Drawing.Point(224, 36);
            VariablesGroupBox.Size = new System.Drawing.Size(318, 82);
            label5.Text = "Cкорость";
            label9.Enabled = true;
            textBox1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string currentButton = string.Empty;       
           
                if (Static.Checked == true)
                {
                    if(locoType.Text == string.Empty)
                    {
                        MessageBox.Show("Выберите тип локомотива!", "Ошибка!");
                    }
                    else if(locoName.Text == string.Empty)
                    {
                        MessageBox.Show("Выберите название локомотива!", "Ошибка!");
                    }
                    else if(Speed.Text == string.Empty)
                    {
                        MessageBox.Show("Введите скорость!", "Ошибка!");
                    }
                    else if (Double.Parse(Speed.Text) < 0)
                    {
                        MessageBox.Show("Скорость не может быть отрицательной. Введите значение больше либо равное нулю!", "Ошибка!");
                    }
                    else
                    {
                        StaticCalc();
                    }                   
                }
                else if (Dinamic.Checked == true)
                {
                    if (locoType.Text == string.Empty)
                    {
                        MessageBox.Show("Выберите тип локомотива!", "Ошибка!");
                    }
                    else if (locoName.Text == string.Empty)
                    {
                        MessageBox.Show("Выберите название локомотива!", "Ошибка!");
                    }
                    else if (Speed.Text == string.Empty)
                    {
                        MessageBox.Show("Введите начальную скорость!", "Ошибка!");
                    }
                    else if (Double.Parse(Speed.Text) < 0)
                    {
                        MessageBox.Show("Скорость не может быть отрицательной. Введите значение больше нуля!", "Ошибка!");
                    }
                    else if (finalSpeed.Text == string.Empty)
                    {
                        MessageBox.Show("Введите конечную скорость!", "Ошибка!");
                    }
                    else if (Double.Parse(finalSpeed.Text) < 0)
                    {
                        MessageBox.Show("Скорость не может быть отрицательной. Введите значение больше нуля!", "Ошибка!");
                    }
                    else if (Step.Text == string.Empty)
                    {
                        MessageBox.Show("Введите шаг!", "Ошибка!");
                    }
                    else if (Double.Parse(Speed.Text) > Double.Parse(finalSpeed.Text))
                    {
                        MessageBox.Show("Начальная скорость должна быть меньше конечной!", "Ошибка!");
                    }
                    else
                    {
                        DinamicCalc();
                    }
                }
            }

        private void StaticCalc()
        {
            string loco_Type = string.Empty;
            string loco_Name = string.Empty;
            double speed = 0;
            double initialAdhCoeff = 0;
            double adhCoeff = 0;
            double percentCount = 0;
            //cmdBD.Connection = conBD;
            loco_Type = locoType.SelectedItem.ToString();
            loco_Name = locoName.SelectedItem.ToString();

            conBD.Open();
            cmdBD.CommandText = "select adh_initial_coeff from loco_adhesion_coeff where id_loco_type ='" + idLocoType + "'";
            readBD = cmdBD.ExecuteReader();

            if (readBD.HasRows)
            {
                while (readBD.Read())
                {
                    initialAdhCoeff = Convert.ToDouble(readBD[0].ToString());
                    break;
                }
            }
            conBD.Close();
            speed = Convert.ToDouble(Speed.Text);

            adhCoeff = SwitchList(loco_Type, loco_Name, initialAdhCoeff, speed);

            percentCount = chkRules();
            adhCoeff = (adhCoeff * percentCount) / 100;
            textBox1.Text = Convert.ToString(Math.Round(adhCoeff, 5));       
        }

        private void DinamicCalc()
        {
            string loco_Type = string.Empty;
            string loco_Name = string.Empty;
            double speed = 0;
            double fSpeed = 0;
            double initialAdhCoeff = 0;
            double step = 0;

            loco_Type = locoType.SelectedItem.ToString();
            loco_Name = locoName.SelectedItem.ToString();

            conBD.Open();
            cmdBD.CommandText = "select adh_initial_coeff from loco_adhesion_coeff where id_loco_type ='" + idLocoType + "'";
            readBD = cmdBD.ExecuteReader();

            if (readBD.HasRows)
            {
                while (readBD.Read())
                {
                    initialAdhCoeff = Convert.ToDouble(readBD[0].ToString());
                    break;
                }
            }
            conBD.Close();
            speed = Convert.ToDouble(Speed.Text);
            fSpeed = Convert.ToDouble(finalSpeed.Text);
            step = Convert.ToDouble(Step.Text);
            dinamicSwitchList(loco_Type, loco_Name, initialAdhCoeff, speed, fSpeed, step);
        }

        private double SwitchList(string loco_Type, string loco_Name, double initialAdhCoeff, double speed)
        {
            double adhCoeff = 0;
            switch (loco_Type)
            {
                case "Электровоз":
                    switch (loco_Name)
                    {
                        case "ВЛ10":
                        case "ВЛ11":
                        case "ВЛ82":
                            adhCoeff = FormulaForType1(initialAdhCoeff, speed);
                            break;
                        case "ВЛ22":
                        case "ВЛ23":
                        case "ВЛ8":
                            adhCoeff = FormulaForType2(initialAdhCoeff, speed);
                            break;
                        case "ВЛ60":
                        case "ВЛ80":
                            adhCoeff = FormulaForType3(initialAdhCoeff, speed);
                            break;
                    }
                    break;
                case "Тепловоз":
                    switch (loco_Name)
                    {
                        case "ТЭ10":
                        case "2ТЭ10Л":
                            adhCoeff = FormulaForType4(initialAdhCoeff, speed);
                            break;
                        case "ТЭ2":
                        case "ТЭ3":
                        case "М62":
                        case "2М62":
                        case "2ТЭ10В":
                        case "2ТЭ10М":
                        case "2ТЭ116":
                        case "3ТЭ10М":
                            adhCoeff = FormulaForType5(initialAdhCoeff, speed);
                            break;
                    }
                    break;
            }
            return adhCoeff;
        }

        private void dinamicSwitchList(string loco_Type, string loco_Name, double initialAdhCoeff, double speed, double fSpeed, double step)
        {
            switch (loco_Type)
            {
                case "Электровоз":
                    switch (loco_Name)
                    {
                        case "ВЛ10":
                        case "ВЛ11":
                        case "ВЛ82":
                            FormulaForType1D(initialAdhCoeff, speed, fSpeed, step);
                            break;
                        case "ВЛ22":
                        case "ВЛ23":
                        case "ВЛ8":
                            FormulaForType2D(initialAdhCoeff, speed, fSpeed, step);
                            break;
                        case "ВЛ60":
                        case "ВЛ80":
                            FormulaForType3D(initialAdhCoeff, speed, fSpeed, step);
                            break;
                    }
                    break;
                case "Тепловоз":
                    switch (loco_Name)
                    {
                        case "ТЭ10":
                        case "2ТЭ10Л":
                            FormulaForType4D(initialAdhCoeff, speed, fSpeed, step);
                            break;
                        case "ТЭ2":
                        case "ТЭ3":
                        case "М62":
                        case "2М62":
                        case "2ТЭ10В":
                        case "2ТЭ10М":
                        case "2ТЭ116":
                        case "3ТЭ10М":
                            FormulaForType5D(initialAdhCoeff, speed, fSpeed, step);
                            break;
                    }
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmdBD.Connection = conBD;

            conBD.Open();
            cmdBD.CommandText = "select type from loco_type";           
            readBD = cmdBD.ExecuteReader();

            if (readBD.HasRows)
            {
                while (readBD.Read())
                {                  
                    locoType.Items.Add(readBD[0].ToString());
                }
            }

            conBD.Close();

            //RadioButtons
            toolTip1.ToolTipIcon = ToolTipIcon.Warning;
           // toolTip1.ToolTipTitle = "Подсказка";
            toolTip1.IsBalloon = true;
            toolTip1.SetToolTip(Static, "Определение расчетного коэффициента сцепления только при одной скорости");
            toolTip1.SetToolTip(Dinamic, "Определение расчетногот коэффициента сцепления с вводом начальной и конечной скорости");
            toolTip1.SetToolTip(checkBox7, "Это условие вляет на расчетный коэффициент сцепления только при начальной скорости < 0.3");
            //Static.Checked = true;


            CheckBoxName();
        }

        private void locoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id_type = string.Empty;

            ComboBox Item = sender as ComboBox;
            if (Item.SelectedIndex != -1)
            {
                conBD.Open();
                cmdBD.CommandText = "select id_type from loco_type where type ='" + locoType.SelectedItem.ToString() + "'";

                readBD = cmdBD.ExecuteReader();
                if (readBD.HasRows)
                {                    
                   while (readBD.Read())
                    {
                        id_type = readBD[0].ToString();
                    }
                }                
                conBD.Close();

                idLocoType = id_type;

                locoName.Items.Clear();
                locoName.Text="";
                conBD.Open();
                cmdBD.CommandText = "select loco_name from loco_adhesion_coeff where id_loco_type ='" + id_type + "'";

                readBD = cmdBD.ExecuteReader();
                if (readBD.HasRows)
                {
                    while (readBD.Read())
                    {
                        locoName.Items.Add(readBD[0].ToString());
                    }
                }  
                conBD.Close();
                locoName.Enabled = true;
            }     
     

        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioBtn = (RadioButton)sender;

            string currentButton = radioBtn.Name;
            if (radioBtn.Checked == true)
            {
                switch (currentButton)
                {
                    case "Static":
                        StaticChecked();
                        break;
                    case "Dinamic":
                        DinamicChecked();
                        break;                
                }
            }
        }

        private void CheckBoxName()
        {
            int i = 1;

            conBD.Open();
            cmdBD.CommandText = "select [rule] from adhesion_rules";

            readBD = cmdBD.ExecuteReader();
            if (readBD.HasRows)
            {
                while (readBD.Read())
                {
                   (groupBox2.Controls["checkBox" + i.ToString()] as CheckBox).Text = readBD[0].ToString();
                   i++;
                }
            }
            conBD.Close();
        }

        private double chkRules()
        {
            double percentCount = 0;
            int i = 0;
            int j = 0;
            int UnChkNum = 0;
            int chkNum = 0;
            string[] chkBoxArray = new string[7];
            double[] percentOfValue = new double[7];

            for (i = 1; i < 8; i++)
            {
                if ((groupBox2.Controls["checkBox" + i.ToString()] as CheckBox).Checked)
                {
                    chkBoxArray[i - 1] = (groupBox2.Controls["checkBox" + i.ToString()] as CheckBox).Text;
                    chkNum++;
                }
                else
                {
                    UnChkNum++;
                }
            }

            if (UnChkNum == 7)
            {
                MessageBox.Show("Вычисление произойдет без дополнительных условий", "Внимание!");
            }
            else
            {
                conBD.Open();
                cmdBD.CommandText = "select precent_of_value, percent_sign, [rule] from adhesion_rules";
                readBD = cmdBD.ExecuteReader();

                if (readBD.HasRows)
                {
                    while (readBD.Read())
                    {
                        for (i = 0; i < 7; i++)
                        {
                            if (readBD[2].ToString() == chkBoxArray[i])
                            {
                                if (readBD[1].ToString() == "-")
                                {
                                    percentOfValue[j] = Convert.ToDouble(readBD[0].ToString()) * (-1);
                                }
                                else
                                {
                                    percentOfValue[j] = Convert.ToDouble(readBD[0].ToString());
                                }
                                percentCount += percentOfValue[j];
                                break;
                            }
                        }
                        j++;
                    }
                }
                conBD.Close();
            }
            percentCount += 100;
            return percentCount;
        }

        private double FormulaForType1(double adhCoeff0, double speed)
        {
            double adhCoeff = 0;
                 
                adhCoeff = adhCoeff0 * ((50 + 16.368 * speed - 0.0412 * speed) / (50 + 20 * speed));
                return adhCoeff;
        }

        private double FormulaForType2(double adhCoeff0, double speed)
        {
            double adhCoeff = 0;

            adhCoeff = adhCoeff0 * ((1 + 0.1515 * speed) / (1 + 0.2 * speed));

            return adhCoeff;
        }

        private double FormulaForType3(double adhCoeff0, double speed)
        {
            double adhCoeff = 0;     

            adhCoeff = adhCoeff0 * ((50 + 4.538 * speed - 0.01 * speed) / (50 + 6 * speed));

            return adhCoeff;
        }

        private double FormulaForType4(double adhCoeff0, double speed)
        {
            double adhCoeff = 0;;

            adhCoeff = adhCoeff0 * ((1 + 0.01789 * speed) / (1 + 0.04545 * speed));

            return adhCoeff;
        }

        private double FormulaForType5(double adhCoeff0, double speed)
        {
            double adhCoeff = 0;

            adhCoeff = adhCoeff0 * ((1 + 0.01431 * speed) / (1 + 0.03636 * speed));

            return adhCoeff;
        }

        private void FormulaForType1D(double adhCoeff0, double speed, double finalSpeed, double step)
        {
            double percentCount = chkRules();
            double dinamicPercent = 0;
            double dinamicSpeed = speed;
            double adhCoeff = 0;
            bool flag = false;

            dMassSpeed.Clear();
            coeffMass.Clear();
            initCoeffMass.Clear();
            //int intIter = IterNumber(speed, finalSpeed, step);

            if ((groupBox2.Controls["checkBox" + 7.ToString()] as CheckBox).Checked)
            {
                flag = true;
            }

            while (dinamicSpeed <= finalSpeed)
            {
                dinamicPercent = percentCount;
                if (dinamicSpeed <= 0.3 && flag == true)
                {
                    dinamicPercent -= 37;
                }
                adhCoeff = adhCoeff0 * ((50 + 16.368 * dinamicSpeed - 0.0412 * dinamicSpeed) / (50 + 20 * dinamicSpeed));
                initCoeffMass.Add(adhCoeff);

                adhCoeff = (adhCoeff * dinamicPercent) / 100;

                dMassSpeed.Add(dinamicSpeed);
                coeffMass.Add(adhCoeff);

                dinamicSpeed += step;
            }

            //double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
            //double[] coeffMass2 = coeffMass.ToArray<double>();
            //label3.Text = Convert.ToString(coeffMass2[50]) + ' ' + Convert.ToString(coeffMass2[1]);
            adhCoeffData();
            DrawGraph();
        }

        private void FormulaForType2D(double adhCoeff0, double speed, double finalSpeed, double step)
        {
            double percentCount = chkRules();
            double dinamicPercent = 0;
            double dinamicSpeed = speed;
            double adhCoeff = 0;
            bool flag = false;

            dMassSpeed.Clear();
            coeffMass.Clear();
            initCoeffMass.Clear();

            if ((groupBox2.Controls["checkBox" + 7.ToString()] as CheckBox).Checked)
            {
                flag = true;
            }

            while (dinamicSpeed <= finalSpeed)
            {
                dinamicPercent = percentCount;
                if (dinamicSpeed <= 0.3 && flag == true)
                {
                    dinamicPercent -= 37;
                }
                adhCoeff = adhCoeff0 * ((1 + 0.1515 * dinamicSpeed) / (1 + 0.2 * dinamicSpeed));
                initCoeffMass.Add(adhCoeff);

                adhCoeff = (adhCoeff * dinamicPercent) / 100;

                dMassSpeed.Add(dinamicSpeed);
                coeffMass.Add(adhCoeff);

                dinamicSpeed += step;
            }
            //double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
            //double[] coeffMass2 = coeffMass.ToArray<double>();
            //label3.Text = Convert.ToString(coeffMass2[50]) + ' ' + Convert.ToString(coeffMass2[1]);
            adhCoeffData();
            DrawGraph();
        }

        private void FormulaForType3D(double adhCoeff0, double speed, double finalSpeed, double step)
        {
            double percentCount = chkRules();
            double dinamicPercent = 0;
            double dinamicSpeed = speed;
            double adhCoeff = 0;
            bool flag = false;

            dMassSpeed.Clear();
            coeffMass.Clear();
            initCoeffMass.Clear();

            if ((groupBox2.Controls["checkBox" + 7.ToString()] as CheckBox).Checked)
            {
                flag = true;
            }

            while (dinamicSpeed <= finalSpeed)
            {
                dinamicPercent = percentCount;
                if (dinamicSpeed <= 0.3 && flag == true)
                {
                    dinamicPercent -= 37;
                }
                adhCoeff = adhCoeff0 * ((50 + 4.538 * dinamicSpeed - 0.01 * dinamicSpeed) / (50 + 6 * dinamicSpeed));
                initCoeffMass.Add(adhCoeff);

                adhCoeff = (adhCoeff * dinamicPercent) / 100;

                dMassSpeed.Add(dinamicSpeed);
                coeffMass.Add(adhCoeff);

                dinamicSpeed += step;
            }
            //double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
            //double[] coeffMass2 = coeffMass.ToArray<double>();
            //label3.Text = Convert.ToString(coeffMass2[50]) + ' ' + Convert.ToString(coeffMass2[1]);
            adhCoeffData();
            DrawGraph();
        }

        private void FormulaForType4D(double adhCoeff0, double speed, double finalSpeed, double step)
        {
            double percentCount = chkRules();
            double dinamicPercent = 0;
            double dinamicSpeed = speed;
            double adhCoeff = 0;
            bool flag = false;

            dMassSpeed.Clear();
            coeffMass.Clear();
            initCoeffMass.Clear();

            if ((groupBox2.Controls["checkBox" + 7.ToString()] as CheckBox).Checked)
            {
                flag = true;
            }

            while (dinamicSpeed <= finalSpeed)
            {
                dinamicPercent = percentCount;
                if (dinamicSpeed <= 0.3 && flag == true)
                {
                    dinamicPercent -= 37;
                }
                adhCoeff = adhCoeff0 * ((1 + 0.01789 * dinamicSpeed) / (1 + 0.04545 * dinamicSpeed));
                initCoeffMass.Add(adhCoeff);

                adhCoeff = (adhCoeff * dinamicPercent) / 100;

                dMassSpeed.Add(dinamicSpeed);
                coeffMass.Add(adhCoeff);

                dinamicSpeed += step;
            }
            //double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
           // double[] coeffMass2 = coeffMass.ToArray<double>();
            //label3.Text = Convert.ToString(coeffMass2[50]) + ' ' + Convert.ToString(coeffMass2[1]);
            adhCoeffData();
            DrawGraph();
        }

        private void FormulaForType5D(double adhCoeff0, double speed, double finalSpeed, double step)
        {
            double percentCount = chkRules();
            double dinamicPercent = 0;
            double dinamicSpeed = speed;
            double adhCoeff = 0;
            bool flag = false;

            dMassSpeed.Clear();
            coeffMass.Clear();
            initCoeffMass.Clear();

            if ((groupBox2.Controls["checkBox" + 7.ToString()] as CheckBox).Checked)
            {
                flag = true;
            }

            while (dinamicSpeed <= finalSpeed)
            {
                dinamicPercent = percentCount;
                if (dinamicSpeed <= 0.3 && flag == true)
                {
                    dinamicPercent -= 37;
                }
                adhCoeff = adhCoeff0 * ((1 + 0.01431 * dinamicSpeed) / (1 + 0.03636 * dinamicSpeed));
                initCoeffMass.Add(adhCoeff);

                adhCoeff = (adhCoeff * dinamicPercent) / 100;

                dMassSpeed.Add(dinamicSpeed);
                coeffMass.Add(adhCoeff);

                dinamicSpeed += step;
            }         
            adhCoeffData();
            DrawGraph();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox6.Enabled = false;
            }
            else
            {
                checkBox6.Enabled = true;
            }
            
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox5.Enabled = false;
            }
            else
            {
                checkBox5.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox8.Enabled = false;
            }
            else
            {
                checkBox8.Enabled = true;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
            }
            else
            {
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox2.Enabled = false;
                checkBox4.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
                checkBox4.Enabled = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            if (chkBox.Checked)
            {
                checkBox3.Enabled = false;
                checkBox2.Enabled = false;
            }
            else
            {
                checkBox3.Enabled = true;
                checkBox2.Enabled = true;
            }
        }

        private int IterNumber(double speed, double finalSpeed, double step)
        {
            double itr = 0;
            
            itr = Math.Ceiling((finalSpeed - speed) / step);

            return Convert.ToInt32(itr);
        }

        private void DrawGraph()
        {
            int lenght = 0;
            string icon = @"..\..\prog-rails_icon-icons.ico";
            Form form = new Form();
            form.Size = new System.Drawing.Size(1000, 650);
            form.Icon = new System.Drawing.Icon(icon);
            form.Text = "График зависимости расчетного коэффициента сцепления от скорости";
			form.Show();
            System.Windows.Forms.DataVisualization.Charting.Chart chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            //System.Windows.Forms.DataVisualization.Charting.Title title = new System.Windows.Forms.DataVisualization.Charting.Title();
			form.Controls.Add(chart);

			chart.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
		    chartArea.Name = "ChartArea";
			chart.ChartAreas.Add(chartArea);
			chart.Dock = System.Windows.Forms.DockStyle.Fill;
			legend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
			legend.Name = "Legend";
			chart.Legends.Add(legend);
			chart.Location = new System.Drawing.Point(0,0);
			chart.Name = "chart";

			series.ChartArea = "ChartArea";
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series.Legend = "Legend";
            series.Name = "Зависимость ψ от v";
            legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 16, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			chart.Series.Add(series);

            series1.ChartArea = "ChartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend";
            series1.Name = "Зависимость ψ от v без условий";
            legend.Font = new System.Drawing.Font("Microsoft Sans Serif", 16, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chart.Series.Add(series1);

			chart.Size = new System.Drawing.Size(1000, 650);
			chart.TabIndex = 1;
			chart.Text = "chart1";
			chart.Series.Clear();

			series.Color = Color.Red;
			series.IsVisibleInLegend = true;
			series.IsXValueIndexed = true;

            series1.Color = Color.Blue;
            series1.IsVisibleInLegend = true;
            series1.IsXValueIndexed = true;

            //title = chart.Titles.Add("");
            //title.Font = new System.Drawing.Font("Times New Roman", 20, FontStyle.Bold);
            //title.ForeColor = System.Drawing.Color.Black;

            chart.ChartAreas["ChartArea"].AxisX.Title = "V, км/ч";
            chart.ChartAreas["ChartArea"].AxisX.TitleFont = new Font("Times New Roman", 14, FontStyle.Bold);
            chart.ChartAreas["ChartArea"].AxisX.TitleForeColor = Color.Black;
            //chart.ChartAreas["ChartArea"].AxisX.IsLabelAutoFit;

            chart.ChartAreas["ChartArea"].AxisY.Title = "ψ";
            chart.ChartAreas["ChartArea"].AxisY.TitleFont = new Font("Times New Roman", 16, FontStyle.Bold);
            chart.ChartAreas["ChartArea"].AxisY.TitleForeColor = Color.Black;
				 
			chart.Series.Add(series);
            chart.Series.Add(series1);

            double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
            double[] coeffMass2 = coeffMass.ToArray<double>();
            double[] coeffMassInit = initCoeffMass.ToArray<double>();
            lenght = dMassSpeed2.Length;
				
			for (int i = 0; i < lenght; i++) {
                series.Points.AddXY(Math.Round(dMassSpeed2[i], 3), coeffMass2[i]);
                series1.Points.AddXY(Math.Round(dMassSpeed2[i], 3), coeffMassInit[i]);
			}
        }

        private void Speed_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Double.Parse(Speed.Text) > 0.3)
                {
                    checkBox7.Enabled = false;
                }
                else if (Double.Parse(Speed.Text) < 0.3)
                {
                    checkBox7.Enabled = true;
                }
            }
            catch
            {
                checkBox7.Enabled = true;
            }
        }

        private void adhCoeffData()
        {
            int lenght = 0;
            DataGridViewTextBoxColumn Column1 = new DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn Column2 = new DataGridViewTextBoxColumn();


            dataGridView1.Columns.Clear();
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		    dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[2] {
				Column1,
				Column2
			});          

            double[] dMassSpeed2 = dMassSpeed.ToArray<double>();
            double[] coeffMass2 = coeffMass.ToArray<double>();
            lenght = dMassSpeed2.Length;
           
            dataGridView1.Rows.Add(lenght);

            Column1.HeaderText = "V";					  
            Column1.MaxInputLength = 24;					  
            Column1.Name = "Column1";					 
            Column1.ReadOnly = true;
            Column1.Width = 60;

            Column2.HeaderText = "ψ";					  
            Column2.MaxInputLength = 24;					  
            Column2.Name = "Column2";					  
            Column2.ReadOnly = true;					  
            Column2.Width = 60;
            
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystroke;

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[i];

                foreach (DataGridViewCell cell in dataGridViewRow.Cells)
                {
                    string val = cell.Value as string;
                    if (string.IsNullOrEmpty(val))
                    {
                        if (!dataGridViewRow.IsNewRow)
                        {
                            dataGridView1.Rows.Remove(dataGridViewRow);
                            break;
                        }
                    }
                }
            }

            dataGridView1.Rows.Add(lenght);

            for (int i = 0; i < 2; i++){
                for (int j = 0; j < lenght; j++)
                {
					if (i == 0)
                        dataGridView1.Rows[j].Cells[i].Value = Convert.ToString(Math.Round(dMassSpeed2[j], 3));

					if (i == 1)
                        dataGridView1.Rows[j].Cells[i].Value = Convert.ToString(Math.Round(coeffMass2[j], 3));
				}
            }            
        }
    }
}
