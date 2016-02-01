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
        #region 读取文件+填充表格
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

                //PlanDate
                nIndex = str.IndexOf("--");
                strTmp = str.Substring(0, nIndex);
                str = str.Substring(nIndex + 2);
                WLD.PlanDate = Convert.ToDateTime(strTmp);


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
                dataGridView1.Rows[i].Cells[1].Value = lst[i].DtWorkLog.ToString("yyyy-MM-dd HH:mm:ss");
                dataGridView1.Rows[i].Cells[2].Value = lst[i].PlanDate.ToString("yyyy-MM-dd");//计划日期
                dataGridView1.Rows[i].Cells[3].Value = lst[i].StrPlan.ToString();
                dataGridView1.Rows[i].Cells[4].Value = lst[i].StrDetail.ToString();
                dataGridView1.Rows[i].Cells[5].Value = lst[i].IsDone.ToString();
                dataGridView1.Rows[i].Cells[6].Value = lst[i].Name.ToString();
                dataGridView1.Rows[i].Cells[7].Value = lst[i].Remark.ToString();
                if (lst[i].IsDone == false)
                {
                    SetColor_By_Row(Color.FromName("LightGreen"), i);
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
        #endregion

        #region 加载页面和表格添加行事件+日历

        private void Form1_Load(object sender, EventArgs e)
        {
            //日历
            CalendarColumn col = new CalendarColumn();
            col.HeaderText = "计划日期";
            //this.dataGridView1.Columns.Add(col);
            this.dataGridView1.Columns.Insert(2, col);
            // Read Data
            List<string> lst = ReadFile();
            // Insert dataGridView
            List<WorkLogData> dataList = ConvertList(lst);
            InsertToList(dataList);
            readTxt();
            this.WindowState = FormWindowState.Maximized;
            dataGridView1.RowsAdded += dataGridView1_RowsAdded;
            Remind();

            foreach (DataGridViewColumn  item in dataGridView1.Columns)
            {
                dataGridView1.Columns[item.Name].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }
        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                dataGridView1.Rows[i].Cells[0].ReadOnly = true;
                dataGridView1.Rows[i].Cells[1].ReadOnly = true;
            }
            dataGridView1.Rows[e.RowIndex - 1].Cells[0].Value = (dataGridView1.Rows.Count - 1).ToString();
            dataGridView1.Rows[e.RowIndex - 1].Cells[1].Value = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        }

        #endregion
      
        #region 保存
        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                //计划日期
                DateTime tmpDt1 = new DateTime();
                if (!DateTime.TryParse(dataGridView1.Rows[i].Cells[2].Value.ToString().Trim(), out tmpDt1))
                {
                    MessageBox.Show("第" + i.ToString() + "行的日期格式不正确!");
                    return false;
                }
                WLD.PlanDate = tmpDt1;

                // 计划
                if (dataGridView1.Rows[i].Cells[3].Value == null)
                {
                    WLD.StrPlan = " ";
                }
                else
                {
                    WLD.StrPlan = dataGridView1.Rows[i].Cells[3].Value.ToString().Trim();
                }

                // 详细信息
                if (dataGridView1.Rows[i].Cells[4].Value == null)
                {
                    WLD.StrDetail = " ";
                }
                else
                {
                    WLD.StrDetail = dataGridView1.Rows[i].Cells[4].Value.ToString().Trim();
                }

                // 完成
                if (dataGridView1.Rows[i].Cells[5].Value == null)
                {
                    WLD.IsDone = false;
                }
                else
                {
                    WLD.IsDone = dataGridView1.Rows[i].Cells[5].Value.ToString().Trim() == "True" ? true : false;
                }
                // 相关人员
                if (dataGridView1.Rows[i].Cells[6].Value == null)
                {
                    WLD.Name = " ";
                }
                else
                {
                    WLD.Name = dataGridView1.Rows[i].Cells[6].Value.ToString().Trim();
                }

                // 备注
                if (dataGridView1.Rows[i].Cells[7].Value == null)
                {
                    WLD.Remark = " ";
                }
                else
                {
                    WLD.Remark = dataGridView1.Rows[i].Cells[7].Value.ToString().Trim();
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
                str += lst[i].DtWorkLog.ToString("yyyy-MM-dd HH:mm:ss");
                str += "--";
                str += lst[i].PlanDate.ToString("yyyy-MM-dd HH:mm:ss");
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

        #endregion
        #region 提醒
        private void Remind()
        {
            for(int i=0;i<dataGridView1.Rows.Count -1;i++)
            {
               string str = "";
               str = DateTime.Now.ToString("yyyy-MM-dd");

                if (dataGridView1.Rows[i].Cells[2].Value.ToString() == str && dataGridView1.Rows[i].Cells[5].Value.ToString() == "False")
                {
                   MessageBox.Show("今天有计划任务，请关注", "日程提醒", MessageBoxButtons.OK);
                   SetColor_By_Row(Color.FromName("LightSkyBlue"), i);
                }
            }
        }
        #endregion
        #region 最小化到系统托盘+关闭时保存
        /// <summary>
        /// 最小化到系统托盘
        /// </summary>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
                notifyIcon1.Visible = true;  //托盘图标可见
                this.Visible = false;
            }
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;  //显示在系统任务栏
            this.Visible = true;
            this.WindowState = FormWindowState.Maximized;  //还原窗体
            notifyIcon1.Visible = false;  //托盘图标隐藏
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult Result = MessageBox.Show("是否退出并保存？\n（是：保存退出；否：不保存退出；取消：不退出；)", "提示", MessageBoxButtons.YesNoCancel);
            if (Result == DialogResult.Yes)
            {
                List<WorkLogData> lst = new List<WorkLogData>();
                if (saveTable(ref lst))
                {
                    saveTable_In_File(lst, "WorkLogData.txt");
                    saveTxt();
                    MessageBox.Show("保存成功");
                }
            }
            else if (Result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (Result == DialogResult.No)
            {

            }
        }

        #endregion

        #region 绑定日历(msdn上面的代码们，其实没怎么看懂- -）
        //这一大堆都是直接复制。。过来。。的。。。。。【这个世界真是……
        public class CalendarColumn : DataGridViewColumn
        {
            public CalendarColumn()
                : base(new CalendarCell())
            {
            }

            public override DataGridViewCell CellTemplate
            {
                get
                {
                    return base.CellTemplate;
                }
                set
                {
                    // Ensure that the cell used for the template is a CalendarCell.
                    if (value != null &&
                        !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
                    {
                        throw new InvalidCastException("Must be a CalendarCell");
                    }
                    base.CellTemplate = value;
                }
            }
        }

        public class CalendarCell : DataGridViewTextBoxCell
        {

            public CalendarCell()
                : base()
            {
                // Use the short date format.
                this.Style.Format = "g";
            }

            public override void InitializeEditingControl(int rowIndex, object
                initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                try
                {
                    // Set the value of the editing control to the current cell value.
                    base.InitializeEditingControl(rowIndex, initialFormattedValue,
                        dataGridViewCellStyle);
                    CalendarEditingControl ctl =
                        DataGridView.EditingControl as CalendarEditingControl;
                    // Use the default row value when Value property is null.
                    if (this.Value == null)
                    {
                        ctl.Value = (DateTime)this.DefaultNewRowValue;
                    }
                    else
                    {
                        //  ctl.Value = (DateTime)this.Value;
                        ctl.Value = DateTime.Parse(this.Value.ToString());
                    }
                }
                catch (Exception)
                {
                    throw;
                }
               
            }

            public override Type EditType
            {
                get
                {
                    // Return the type of the editing control that CalendarCell uses.
                    return typeof(CalendarEditingControl);
                }
            }

            public override Type ValueType
            {
                get
                {
                    // Return the type of the value that CalendarCell contains.

                    return typeof(DateTime);
                }
            }

            public override object DefaultNewRowValue
            {
                get
                {
                    // Use the current date and time as the default value.
                    return DateTime.Now;
                }
            }
        }

        class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
        {
            DataGridView dataGridView;
            private bool valueChanged = false;
            int rowIndex;

            public CalendarEditingControl()
            {
                this.Format = DateTimePickerFormat.Short;
            }

            // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
            // property.
            public object EditingControlFormattedValue
            {
                get
                {
                    //return this.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    return this.Value.ToShortDateString();
                }
                set
                {
                    if (value is String)
                    {
                        try
                        {
                            // This will throw an exception of the string is 
                            // null, empty, or not in the format of a date.
                            this.Value = DateTime.Parse((String)value);
                        }
                        catch
                        {
                            // In the case of an exception, just use the 
                            // default value so we're not left with a null
                            // value.
                            this.Value = DateTime.Now;
                        }
                    }
                }
            }

            // Implements the 
            // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
            public object GetEditingControlFormattedValue(
                DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            // Implements the 
            // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
            public void ApplyCellStyleToEditingControl(
                DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
                this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
                this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
            }

            // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
            // property.
            public int EditingControlRowIndex
            {
                get
                {
                    return rowIndex;
                }
                set
                {
                    rowIndex = value;
                }
            }

            // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
            // method.
            public bool EditingControlWantsInputKey(
                Keys key, bool dataGridViewWantsInputKey)
            {
                // Let the DateTimePicker handle the keys listed.
                switch (key & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
            // method.
            public void PrepareEditingControlForEdit(bool selectAll)
            {
                // No preparation needs to be done.
            }

            // Implements the IDataGridViewEditingControl
            // .RepositionEditingControlOnValueChange property.
            public bool RepositionEditingControlOnValueChange
            {
                get
                {
                    return false;
                }
            }

            // Implements the IDataGridViewEditingControl
            // .EditingControlDataGridView property.
            public DataGridView EditingControlDataGridView
            {
                get
                {
                    return dataGridView;
                }
                set
                {
                    dataGridView = value;
                }
            }

            // Implements the IDataGridViewEditingControl
            // .EditingControlValueChanged property.
            public bool EditingControlValueChanged
            {
                get
                {
                    return valueChanged;
                }
                set
                {
                    valueChanged = value;
                }
            }

            // Implements the IDataGridViewEditingControl
            // .EditingPanelCursor property.
            public Cursor EditingPanelCursor
            {
                get
                {
                    return base.Cursor;
                }
            }

            protected override void OnValueChanged(EventArgs eventargs)
            {
                // Notify the DataGridView that the contents of the cell
                // have changed.
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
            }
        }

        #endregion

        static SortOrder dgvSortOrder = SortOrder.Ascending;
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.Sort(new RowComparer(dgvSortOrder, e.ColumnIndex));
            dgvSortOrder = (dgvSortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
        }

        private bool ComprText(string strText, string strCompText)
        {
            return strText.IndexOf(strCompText) != -1;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string strText = txtSearch.Text.Trim();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                bool bViable = false;
                if (ComprText(dataGridView1.Rows[i].Cells[3].Value.ToString(), strText) || 
                    ComprText(dataGridView1.Rows[i].Cells[4].Value.ToString(), strText) ||
                    ComprText(dataGridView1.Rows[i].Cells[6].Value.ToString(), strText) || 
                    ComprText(dataGridView1.Rows[i].Cells[7].Value.ToString(), strText))
                {
                    bViable = true; 
                }
                dataGridView1.Rows[i].Visible = bViable;
            }
        }

       
    }

    class RowComparer : System.Collections.IComparer
    {
        private static int sortOrderModifier = 1;
        private int m_nColumnIndex = 0;

        public RowComparer(SortOrder sortOrder, int nColumnIndex)
        {
            m_nColumnIndex = nColumnIndex;
            sortOrderModifier = sortOrder == SortOrder.Descending ? -1 : 1;
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

            // Try to sort based on the Last Name column.
            int CompareResult = System.String.Compare(
                DataGridViewRow1.Cells[m_nColumnIndex].Value.ToString(),
                DataGridViewRow2.Cells[m_nColumnIndex].Value.ToString());

            // If the Last Names are equal, sort based on the First Name.
           /* if (CompareResult == 0)
            {
                CompareResult = System.String.Compare(
                    DataGridViewRow1.Cells[m_nColumnIndex].Value.ToString(),
                    DataGridViewRow2.Cells[m_nColumnIndex].Value.ToString());
            }*/
            return CompareResult * sortOrderModifier;
        }
    }
}