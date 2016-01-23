using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 工作日记
{
    public class WorkLogData
    {
        // 日期
        private DateTime dtWorkLog;
         
        public DateTime DtWorkLog
        {
            get { return dtWorkLog; }  
            set { dtWorkLog = value; }
        }
        // 计划
        private string strPlan;

        public string StrPlan
        {
            get { return strPlan; }
            set { strPlan = value; }
        }
        // 详细信息
        private string strDetail;

        public string StrDetail
        {
            get { return strDetail; }
            set { strDetail = value; }
        }
        // 是否完成
        private bool isDone;

        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }
        // 相关人员
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // 备注
        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
    }
}
