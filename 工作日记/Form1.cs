using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 工作日记
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        private List<string> ReadFile()
        {
            List<string> lst = new List<string>();

            using (StreamReader Sr = new StreamReader(System.Environment.CurrentDirectory + @"\WorkLogData.txt", Encoding.Unicode))
            {
                string str = string.Empty;
                do
                {
                    str = Sr.ReadLine();
                    if (str != null && str.Trim() != "")
                        lst.Add(str);
                } while (str != null);
                Sr.Close();
            }

            return lst;
        }
        /// <summary>
        /// 读取文本数据
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        private List<WorkLogData> ConvertList(List<string> lst)
        {
            int nIndex = -1;
            string strTmp = "";

            List<WorkLogData> RetLst = new List<WorkLogData>();
            for (int i = 0; i < lst.Count; i++)
            {
                string str = lst[i];
                WorkLogData WLD = new WorkLogData();

                // DateTime
                nIndex = str.IndexOf("--");
                if (nIndex == -1)
                {
                    MessageBox.Show("Error Text format!");
                    Application.Exit();
                    break;
                }

                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                WLD.DtWorkLog = Convert.ToDateTime(strTmp);

                // Plan
                nIndex = str.IndexOf("--");
                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                WLD.StrPlan = strTmp;

                // Detail 
                nIndex = str.IndexOf("--");
                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                WLD.StrDetail = strTmp;

                // IsDone
                nIndex = str.IndexOf("--");
                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                if (strTmp == "0")
                    WLD.IsDone = false;
                else
                    WLD.IsDone = true;

                // Name
                nIndex = str.IndexOf("--");
                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                WLD.Name = strTmp;

                //Remark
                WLD.Remark = str;
                RetLst.Add(WLD);
            }
            return RetLst;
        }
        /// <summary>
        /// 添加到表格
        /// </summary>
        /// <param name="lst"></param>
        private void InsertToList(List<WorkLogData> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                dataGridView1.Rows[i].Cells[1].Value = lst[i].DtWorkLog.ToString("g");
                dataGridView1.Rows[i].Cells[2].Value = lst[i].StrPlan.ToString();
                dataGridView1.Rows[i].Cells[3].Value = lst[i].StrDetail.ToString();
                dataGridView1.Rows[i].Cells[4].Value = lst[i].IsDone.ToString();
                dataGridView1.Rows[i].Cells[5].Value = lst[i].Name.ToString();
                dataGridView1.Rows[i].Cells[6].Value = lst[i].Remark.ToString();
                if (lst[i].IsDone == false)
                {
                    SetColor_By_Row(Color.FromName("DarkSeaGreen"), i);
                }
            }
        }
        /// <summary>
        /// 设置表格颜色
        /// </summary>
        /// <param name="Cor"></param>
        /// <param name="nRow"></param>
        private void SetColor_By_Row(Color Cor, int nRow)
        {
            DataGridViewCellStyle DGVCS = new System.Windows.Forms.DataGridViewCellStyle();
            DGVCS.BackColor = Cor;
            for (int k = 0; k < dataGridView1.Columns.Count; k++)
            {
                dataGridView1.Rows[nRow].Cells[k].Style = DGVCS;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.Columns[4].Width = 260;
            //dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Read Data
            List<string> lst = ReadFile();
            // Insert dataGridView
            List<WorkLogData> dataList = ConvertList(lst);
            InsertToList(dataList);
            readTxt();

            dataGridView1.RowsAdded += dataGridView1_RowsAdded;
        }
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex - 1].Cells[0].Value = (dataGridView1.Rows.Count - 1).ToString();
            dataGridView1.Rows[e.RowIndex - 1].Cells[1].Value = System.DateTime.Now.ToString("g");
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1.Rows[i].Cells[0].ReadOnly = true;
                dataGridView1.Rows[i].Cells[1].ReadOnly = true;
            }
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            List<WorkLogData> lst = new List<WorkLogData>();
            if (saveTable(ref lst))
            {
                saveTable_In_File(lst, "WorkLogData.txt");
                saveTxt();
                MessageBox.Show("保存成功");
            }
        }
        private void saveTxt()
        {
            StreamWriter Sw = new StreamWriter(System.Environment.CurrentDirectory + @"\Objectives.txt", false, Encoding.Unicode);
            Sw.Write(textBox2.Text.ToString());
            StreamWriter Sw1 = new StreamWriter(System.Environment.CurrentDirectory + @"\ImportantMatters.txt", false, Encoding.Unicode);
            Sw1.Write(textBox1.Text.ToString());
            Sw.Close();
            Sw1.Close();
        }
        private void readTxt()
        {
            StreamReader Sd = new StreamReader(System.Environment.CurrentDirectory + @"\Objectives.txt", Encoding.Unicode);
            textBox2.Text = Sd.ReadToEnd();
            StreamReader Sd1 = new StreamReader(System.Environment.CurrentDirectory + @"\ImportantMatters.txt", Encoding.Unicode);
            textBox1.Text = Sd1.ReadToEnd();
            Sd.Close();
            Sd1.Close();
        }
        private bool saveTable(ref List<WorkLogData> lst)
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                WorkLogData WLD = new WorkLogData();

                // 日期
                DateTime tmpDt = new DateTime();
                if (!DateTime.TryParse(dataGridView1.Rows[i].Cells[1].Value.ToString().Trim(), out tmpDt))
                {
                    MessageBox.Show("第" + i.ToString() + "行的日期格式不正确!");
                    return false;
                }
                WLD.DtWorkLog = tmpDt;

                // 计划
                if (dataGridView1.Rows[i].Cells[2].Value == null)
                {
                    WLD.StrPlan = " ";
                }
                else
                {
                    WLD.StrPlan = dataGridView1.Rows[i].Cells[2].Value.ToString().Trim();
                }

                // 详细信息
                if (dataGridView1.Rows[i].Cells[3].Value == null)
                {
                    WLD.StrDetail = " ";
                }
                else
                {
                    WLD.StrDetail = dataGridView1.Rows[i].Cells[3].Value.ToString().Trim();
                }

                // 完成
                if (dataGridView1.Rows[i].Cells[4].Value == null)
                {
                    WLD.IsDone = false;
                }
                else
                {
                    WLD.IsDone = dataGridView1.Rows[i].Cells[4].Value.ToString().Trim() == "True" ? true : false;
                }
                // 相关人员
                if (dataGridView1.Rows[i].Cells[5].Value == null)
                {
                    WLD.Name = " ";
                }
                else
                {
                    WLD.Name = dataGridView1.Rows[i].Cells[5].Value.ToString().Trim();
                }

                // 备注
                if (dataGridView1.Rows[i].Cells[6].Value == null)
                {
                    WLD.Remark = " ";
                }
                else
                {
                    WLD.Remark = dataGridView1.Rows[i].Cells[6].Value.ToString().Trim();
                }
                lst.Add(WLD);
            }
            return true;

        }
        private void saveTable_In_File(List<WorkLogData> lst, string strFileName)
        {
            string str = string.Empty;
            for (int i = 0; i < lst.Count; i++)
            {
                str += lst[i].DtWorkLog.ToString("g");
                str += "--";
                str += lst[i].StrPlan;
                str += "--";
                str += lst[i].StrDetail;
                str += "--";
                str += lst[i].IsDone ? "1" : "0";
                str += "--";
                str += lst[i].Name;
                str += "--";
                str += lst[i].Remark;
                str += "\r\n";
            }

            using (StreamWriter SW = new StreamWriter(System.Environment.CurrentDirectory + @"\" + strFileName, false, Encoding.Unicode))
            {
                SW.Write(str);
                SW.Close();
            }
        }
        private void MiniWindow()
        {
            //if(this.WindowState == )
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}