using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Model
{
    public class DialyReport
    {
        public DialyReport()
        {
            Details = new List<GoodsRecord>();
        }

        public int Id { get; set; }
        public DateTime SaleDate { get; set; }

        public string SaleDateStr
        {
            get { return SaleDate.ToString("yyyy-MM-dd"); }
        }

        /// <summary>
        /// 日销
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 来客数
        /// </summary>
        public int Customer { get; set; }
        /// <summary>
        /// 报废
        /// </summary>
        public double Waste { get; set; }

        /// <summary>
        /// 兼职
        /// </summary>
        public double ParttimeEmployee { get; set; }
        /// <summary>
        /// 正职
        /// </summary>
        public double Employee { get; set; }

        /// <summary>
        /// 包材
        /// </summary>
        public double PackingMaterialAmount { get; set; }
        /// <summary>
        /// 易耗品
        /// </summary>
        public double ConsumeableAmount { get; set; }
        /// <summary>
        /// 电表度数
        /// </summary>
        public double ElectrictCharge { get; set; }

        /// <summary>
        /// 神秘客户问题
        /// </summary>
        public string Problem { get; set; }

        public IList<GoodsRecord> Details { get; private set; }
    }
}
