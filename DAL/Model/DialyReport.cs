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

        public bool IsWeekend
        {
            get
            {
                return this.SaleDate.DayOfWeek == DayOfWeek.Saturday || this.SaleDate.DayOfWeek == DayOfWeek.Sunday;
            }
        }

        public string SaleDateStr
        {
            get { return string.Format("{0} {1}", SaleDate.ToString("yyyy-MM-dd"), GetDayOfWeek(this.SaleDate)); }
        }

        private string GetDayOfWeek(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "周五";
                case DayOfWeek.Monday:
                    return "周一";
                case DayOfWeek.Saturday:
                    return "周六";
                case DayOfWeek.Sunday:
                    return "周日";
                case DayOfWeek.Thursday:
                    return "周四";
                case DayOfWeek.Tuesday:
                    return "周二";
                case DayOfWeek.Wednesday:
                    return "周三";
                default:
                    return "周一";
            }
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

        /// <summary>
        /// 天气
        /// </summary>
        public string Weather { get; set; }
        public IList<GoodsRecord> Details { get; private set; }
    }
}
