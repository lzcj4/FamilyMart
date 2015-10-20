using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Common;
using DAL.Model;
using System.Text.RegularExpressions;

namespace DAL
{
    #region

    //9月13日  
    //余杭文一西路店日商7630
    //来客数476
    //报废134.

    //OC包含果汁36
    //中岛柜43
    //关东煮22
    //蒸包2.5

    //盒饭一配进/销/废70/69/0三配25/25/0
    //饭团一配10/9/1三配6/6/0
    //三明治一配15/15/0三配6/7/0
    //寿司一配14/13/1三配0/0/0
    //调理面一配36/34/1三配6/6/0
    //面包98/49/5

    //集享卡4 
    //+2元得康师傅饮品4，
    //+5元维他椰子水0，
    //哈根达斯小纸杯6，
    //冰淇淋13，
    //咖茶10，

    //兼职8，
    //正职20，

    //物流箱进/出/在店0/0/10

    //包材金额0
    //消耗品金额0
    //电表度数523
    //神秘客问题:排面差.地上纸屑

    #endregion

    public class DialyReportParser
    {
        static IDictionary<string, Goods> goodsCache = new Dictionary<string, Goods>();
        static DialyReportParser()
        {
            LoadGoods();
        }

        public static void LoadGoods()
        {
            goodsCache.Clear();
            var list = FMDBHelper.Instance.GetGoods();
            foreach (var item in list)
            {
                goodsCache.Add(item.Name, item);
            }
        }

        static StringBuilder sbInfo = new StringBuilder();

        private static string GetGoodName(string itemValue)
        {
            if (itemValue.IsNullOrEmpty())
            {
                throw new ArgumentNullException("当前解析报表数据不能为空");
            }

            string result = string.Empty;
            foreach (var item in goodsCache.Keys)
            {
                if (itemValue.Contains(item))
                {
                    result = item;
                    break;
                }
            }
            if (result.IsNullOrEmpty())
            {
                string ex = string.Format("当前货品名称不符:{0}", itemValue);
                sbInfo.AppendLine(ex);
                throw new InvalidOperationException(ex);
            }
            return result;
        }

        public static DialyReport Parse(string str, string weather)
        {
            if (str.IsNullOrEmpty())
            {
                throw new ArgumentNullException("当前解析报表数据不能为空");
            }

            sbInfo.Clear();
            string[] parts = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.IsNullOrEmpty())
            {
                throw new InvalidOperationException("当前解释日商数据非27项，请正确格式化");
            }
            bool hasHargendas = parts[17].Contains("哈根达斯");
            if (hasHargendas)
            {
                if (parts.Count() != 27)
                {
                    sbInfo.AppendLine("当前数据有缺少，非 27 个标准项");
                    Logger.WriteLine("当前数据有缺少，非 27 个标准项");
                }
            }
            else
            {
                if (parts.Count() != 26)
                {
                    sbInfo.AppendLine("当前数据有缺少，非 26 个标准项");
                    Logger.WriteLine("当前数据有缺少，非 26 个标准项");
                }
            }

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            DialyReport dialyRep = new DialyReport();
            dialyRep.SaleDate = GetDate(parts[0]);
            dialyRep.Amount = GetDoubleValue("余杭文一西路店日商", parts[1]);
            dialyRep.Customer = (int)GetDoubleValue("来客数", parts[2]);
            dialyRep.Waste = GetDoubleValue("报废", parts[3]);

            AddNormalRecord(GetGoodName(parts[4]), parts[4], dialyRep);
            AddNormalRecord(GetGoodName(parts[5]), parts[5], dialyRep);
            AddNormalRecord(GetGoodName(parts[6]), parts[6], dialyRep);
            AddNormalRecord(GetGoodName(parts[7]), parts[7], dialyRep);

            AddDeliveryRecord(GetGoodName(parts[8]), parts[8], dialyRep);
            AddDeliveryRecord(GetGoodName(parts[9]), parts[9], dialyRep);
            AddDeliveryRecord(GetGoodName(parts[10]), parts[10], dialyRep);
            AddDeliveryRecord(GetGoodName(parts[11]), parts[11], dialyRep);
            AddDeliveryRecord(GetGoodName(parts[12]), parts[12], dialyRep);
            AddDeliveryRecord(GetGoodName(parts[13]), parts[13], dialyRep);

            AddNormalRecord(GetGoodName(parts[14]), parts[14], dialyRep);
            AddNormalRecord(GetGoodName(parts[15]), parts[15], dialyRep);
            AddNormalRecord(GetGoodName(parts[16]), parts[16], dialyRep);

            int itemIndex = 17;
            if (!hasHargendas)
            {
                //AddNormalRecord("哈根达斯小纸杯", "哈根达斯小纸杯0", dialyRep);
                itemIndex = 16;
            }
            else
            {
                AddNormalRecord(GetGoodName(parts[itemIndex]), parts[itemIndex], dialyRep);
            }

            AddNormalRecord(GetGoodName(parts[itemIndex + 1]), parts[itemIndex + 1], dialyRep);
            AddNormalRecord(GetGoodName(parts[itemIndex + 2]), parts[itemIndex + 2], dialyRep);

            dialyRep.ParttimeEmployee = GetDoubleValue("兼职", parts[itemIndex + 3]);
            dialyRep.Employee = GetDoubleValue("正职", parts[itemIndex + 4]);

            GetBoxRecord("物流箱", parts[itemIndex + 5], dialyRep);

            dialyRep.PackingMaterialAmount = GetDoubleValue("包材金额", parts[itemIndex + 6]);
            dialyRep.ConsumeableAmount = GetDoubleValue("消耗品金额", parts[itemIndex + 7]);
            dialyRep.ElectrictCharge = GetDoubleValue("电表度数", parts[itemIndex + 8]);
            if (itemIndex + 9 < parts.Length)
            {
                dialyRep.Problem = GetStrValue("神秘客问题", parts[itemIndex + 9]);
            }

            dialyRep.Weather = weather;
            Logger.Error(sbInfo.ToString());
            return dialyRep;
        }
        public static string GetLatestError()
        {
            return sbInfo.ToString();
        }

        private static DateTime GetDate(string str)
        {
            string[] parts = str.Split(new string[] { "年", "月", "日" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.IsNullOrEmpty() || !(parts.Length == 2 || parts.Length == 3))
            {
                sbInfo.AppendLine(string.Format("无效日期:{0}", str));
                throw new InvalidOperationException("无效日期");
            }

            string dateStr = string.Empty;
            if (parts.Length == 3)
            {
                dateStr = string.Format("{0:D4}-{1:D2}-{2:D2}", Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
            }
            else if (parts.Length == 2)
            {
                dateStr = string.Format("{0:D4}-{1:D2}-{2:D2}", DateTime.Now.Year, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
            }

            DateTime result = DateTime.Now;
            if (!DateTime.TryParse(dateStr, out result))
            {
                sbInfo.AppendLine(string.Format("无效日期:{0}", dateStr));
                throw new InvalidCastException("非有效数值");
            }
            return result;
        }

        private static string Replace(string str, string prefix, string replaceValue = "")
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            return str.Replace(prefix, replaceValue);
        }

        private static string FilterInvalidValues(string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            string[] invalidValues = { ".", ",", ";", "，", "。", "；" };
            string result = str;
            foreach (var item in invalidValues)
            {
                if (result.EndsWith(item))
                {
                    result = result.Substring(0, str.Length - 1);
                }
            }
            return result;
        }

        private static double GetDoubleValue(string prefix, string str)
        {
            if (str.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("待解析数据为空:{0}", prefix));
                throw new ArgumentNullException("待解析数据为空");
            }

            string content = Replace(str, prefix);
            // amount = FilterInvalidValues(amount);

            Match match = Regex.Match(content, @"\d+\.\d+");
            string amount = string.Empty;
            if (!match.Success)
            {
                match = Regex.Match(content, @"\d+");
            }
            if (match.Success)
            {
                amount = match.Value;
            }
            double result = 0;
            if (!Double.TryParse(amount, out result))
            {
                sbInfo.AppendLine(string.Format("非有效数值:{0}", str));
                throw new InvalidCastException(string.Format("非有效数值:{0}", str));
            }
            return result;
        }

        private static string GetStrValue(string prefix, string str)
        {
            if (str.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("待解析数据为空:{0}", prefix));
                throw new ArgumentNullException("待解析数据为空");
            }
            string result = Replace(str, prefix).Replace(':', ' ').Replace(',', ' ')
                                                .Replace('，', ' ').Replace('。', ' ')
                                                .Trim(new char[] { ',', ':', '.' });
            return result;
        }

        private static MatchCollection GetMatches(string strValue)
        {
            MatchCollection matchValues = Regex.Matches(strValue, @"\d+/\d+/\d+");
            return matchValues;
        }

        private static void AddNormalRecord(string prefix, string str, DialyReport report)
        {
            if (str.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("待解析数据为空:{0}", prefix));
                throw new InvalidCastException(string.Format("待解析数据为空:{0}", prefix));
            }

            if (!goodsCache.ContainsKey(prefix))
            {
                sbInfo.AppendLine(string.Format("解释前缀无效:{0}", prefix));
                return;
            }

            string strValue = GetStrValue(prefix, str);

            if (strValue.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("解释前缀无效:{0}", prefix));
                throw new ArgumentNullException(string.Format("当前:{0} 解析失败", str));
            }

            GoodsRecord result = new GoodsRecord();
            result.Goods = goodsCache[prefix];
            result.DialyReport = report;
            double itemValue = GetDoubleValue(prefix, str);
            result.FirstSale = itemValue;
            report.Details.Add(result);
        }

        private static void AddDeliveryRecord(string prefix, string str, DialyReport report)
        {
            if (str.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("待解析数据为空:{0}", prefix));
                throw new InvalidCastException(string.Format("待解析数据为空:{0}", prefix));
            }

            if (!goodsCache.ContainsKey(prefix))
            {
                sbInfo.AppendLine(string.Format("解释前缀无效:{0}", prefix));
                return;
            }

            string strValue = GetStrValue(prefix, str);
            MatchCollection matchValues = GetMatches(strValue);
            int matchTimes = matchValues.Count;
            if (matchTimes == 0)
            {
                sbInfo.AppendLine(string.Format("当前:{0} 解析失败", str));
                throw new ArgumentNullException(string.Format("当前:{0} 解析失败", str));
            }

            if (matchTimes != 2)
            {
                sbInfo.AppendLine(string.Format("当前:{0} 解析可能有问题", str));
            }

            GoodsRecord result = new GoodsRecord();
            result.Goods = goodsCache[prefix];
            result.DialyReport = report;
            Tuple<double, double, double> firstGroup = GetTuple(matchValues[0].Groups[0].Value);
            result.FirstIn = firstGroup.Item1;
            result.FirstSale = firstGroup.Item2;
            result.FirstWaste = firstGroup.Item3;

            if (matchTimes == 2)
            {
                Tuple<double, double, double> thirdGroup = GetTuple(matchValues[1].Groups[0].Value);
                result.ThirdIn = thirdGroup.Item1;
                result.ThirdSale = thirdGroup.Item2;
                result.ThirdWaste = thirdGroup.Item3;
            }
            report.Details.Add(result);
        }

        private static void GetBoxRecord(string prefix, string str, DialyReport report)
        {
            if (str.IsNullOrEmpty())
            {
                sbInfo.AppendLine(string.Format("待解析数据为空:{0}", prefix));
                throw new InvalidCastException(string.Format("待解析数据为空:{0}", prefix));
            }

            if (!goodsCache.ContainsKey(prefix))
            {
                sbInfo.AppendLine(string.Format("解释前缀无效:{0}", prefix));
                return;
            }

            string strValue = GetStrValue(prefix, str);
            MatchCollection matchValues = GetMatches(strValue);
            if (matchValues.Count != 1)
            {
                sbInfo.AppendLine(string.Format("当前:{0} 解析失败", str));
                throw new ArgumentNullException(string.Format("当前:{0} 解析失败", str));
            }

            GoodsRecord result = new GoodsRecord();
            result.Goods = goodsCache[prefix];
            result.DialyReport = report;
            Tuple<double, double, double> firstGroup = GetTuple(matchValues[0].Groups[0].Value);
            result.FirstIn = firstGroup.Item1;
            result.FirstSale = firstGroup.Item2;
            result.FirstWaste = firstGroup.Item3;

            report.Details.Add(result);
        }

        private static Tuple<double, double, double> GetTuple(string str)
        {
            if (str.IsNullOrEmpty())
            {
                throw new ArgumentNullException("待解析数据为空");
            }
            string[] parts = str.Split(new char[] { '/' });
            if (parts.IsNullOrEmpty() || parts.Length != 3)
            {
                sbInfo.AppendLine(string.Format("当前:{0} 解析失败", str));
                throw new ArgumentNullException(string.Format("当前:{0} 解析失败", str));
            }

            double fiValue = double.Parse(parts[0]);
            double seValue = double.Parse(parts[1]);
            double thValue = double.Parse(parts[2]);
            return new Tuple<double, double, double>(fiValue, seValue, thValue);
        }
    }
}
