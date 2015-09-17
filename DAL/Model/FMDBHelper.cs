using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Common;
using System.Data.SQLite;

namespace DAL.Model
{
    public class FMDBHelper : IDisposable
    {
        private static FMDBHelper instance;
        public static FMDBHelper Instance
        {
            get
            {
                instance = instance ?? new FMDBHelper();
                return instance;
            }
        }

        DBHelper dbHelper;

        public FMDBHelper()
        {
            dbHelper = new DBHelper();
        }

        private const string sqlSelect = "select * from ";
        private string sqlSelectGoods = sqlSelect + " tb_goods";
        private string sqlSelectGoodsRecord = sqlSelect + " tb_goodsrecord";
        private string sqlSelectDialyReport = sqlSelect + " tb_dialyreport";

        private string sqlOrderbyAsc = "  order by id asc";

        #region Query

        public IEnumerable<Goods> GetGoods()
        {
            return GetGoods(sqlSelectGoods + sqlOrderbyAsc);
        }
        private Goods GetGoods(SQLiteDataReader reader)
        {
            Goods item = new Goods();
            item.Id = Convert.ToInt32(reader["Id"]);
            item.Name = reader["Name"].ToString();
            item.Type = (GoodsType)Convert.ToInt32(reader["Id"]);
            return item;
        }

        public IEnumerable<Goods> GetGoods(string sql)
        {
            using (SQLiteDataReader reader = dbHelper.Query(sql))
            {
                while (reader.Read())
                {
                    Goods item = GetGoods(reader);
                    yield return item;
                }
            }
        }

        public IEnumerable<GoodsRecord> GetGoodsRecord()
        {
            string sql = @"select A.* ,from tb_goodsrecord as A, tb_goods as B, tb_dialyreport as C
                         where A.goods_id=B.id and A.dialyreport_id= C.id ";
            //return GetGoodsRecord(sql);
            return GetGoodsRecord(sqlSelectGoodsRecord + sqlOrderbyAsc);

        }

        public IEnumerable<GoodsRecord> GetGoodsRecord(string sql)
        {
            using (SQLiteDataReader reader = dbHelper.Query(sql))
            {
                while (reader.Read())
                {
                    GoodsRecord item = new GoodsRecord();
                    item.Id = Convert.ToInt32(reader["Id"]);

                    item.FirstIn = Convert.ToDouble(reader["FirstIn"]);
                    item.FirstSale = Convert.ToDouble(reader["FirstSale"]);
                    item.FirstWaste = Convert.ToDouble(reader["FirstWaste"]);

                    item.ThirdIn = Convert.ToDouble(reader["ThirdIn"]);
                    item.ThirdSale = Convert.ToDouble(reader["ThirdSale"]);
                    item.ThirdWaste = Convert.ToDouble(reader["ThirdWaste"]);

                    //item.Goods_Id = Convert.ToInt32(reader["Goods_Id"]);
                    //item.DialyReport_Id = Convert.ToInt32(reader["DialyReport_Id"]);

                    int goods_id = Convert.ToInt32(reader["Goods_Id"]);
                    int report_id = Convert.ToInt32(reader["DialyReport_Id"]);

                    item.Goods = GetGoods(string.Format("{0} where Id={1}", sqlSelectGoods, goods_id)).FirstOrDefault();
                    item.DialyReport = GetDialyReportOnly(string.Format("{0} where Id={1}", sqlSelectDialyReport, report_id)).FirstOrDefault();
                    yield return item;
                }
            }
        }

        public IEnumerable<DialyReport> GetDialyReport()
        {
            return GetDialyReport(sqlSelectDialyReport + "  order by SaleDate asc");
        }

        private DialyReport GetDialyReport(SQLiteDataReader reader)
        {
            DialyReport item = new DialyReport();
            item.Id = Convert.ToInt32(reader["Id"]);
            item.SaleDate = Convert.ToDateTime(reader["SaleDate"]);
            item.Amount = Convert.ToDouble(reader["Amount"]);
            item.Customer = Convert.ToInt32(reader["Customer"]);
            item.Waste = Convert.ToDouble(reader["Waste"]);
            item.ParttimeEmployee = Convert.ToDouble(reader["ParttimeEmployee"]);
            item.Employee = Convert.ToDouble(reader["Employee"]);
            item.PackingMaterialAmount = Convert.ToDouble(reader["PackingMaterialAmount"]);
            item.ConsumeableAmount = Convert.ToDouble(reader["ConsumeableAmount"]);
            item.ElectrictCharge = Convert.ToDouble(reader["ElectrictCharge"]);
            item.Problem = Convert.ToString(reader["Problem"]);
            return item;
        }
        public IEnumerable<DialyReport> GetDialyReportOnly(string sql)
        {
            using (SQLiteDataReader reader = dbHelper.Query(sql))
            {
                while (reader.Read())
                {
                    DialyReport item = GetDialyReport(reader);
                    yield return item;
                }
            }
        }

        public IEnumerable<DialyReport> GetDialyReport(string sql)
        {
            using (SQLiteDataReader reader = dbHelper.Query(sql))
            {
                while (reader.Read())
                {
                    DialyReport item = GetDialyReport(reader);
                    var list = GetGoodsRecord(string.Format("{0} where dialyreport_id={1} {2}", sqlSelectGoodsRecord, item.Id, sqlOrderbyAsc));
                    foreach (var tempItem in list)
                    {
                        item.Details.Add(tempItem);
                    }
                    yield return item;
                }
            }
        }

        #endregion

        #region Insert

        public bool InsertDialyReport(DialyReport item)
        {
            string insertFormat = @" insert into tb_dialyreport( SaleDate,Amount,Customer,Waste,ParttimeEmployee,Employee,
                                                                 PackingMaterialAmount,ConsumeableAmount,ElectrictCharge,Problem) 
                                 values('{0}',{1},{2},{3},{4},{5},{6},{7},{8},'{9}')";
            string sql = string.Format(insertFormat, item.SaleDate.ToString("yyyy-MM-dd"), item.Amount, item.Customer, item.Waste, item.ParttimeEmployee,
                                                    item.Employee, item.PackingMaterialAmount, item.ConsumeableAmount,
                                                    item.ElectrictCharge, item.Problem);
            int repId = dbHelper.Insert(sql);
            item.Id = repId;
            StringBuilder sb = new StringBuilder();
            foreach (GoodsRecord tempItem in item.Details)
            {
                sb.AppendLine(GetGoodsRecordSql(tempItem));
            }
            int subResult = dbHelper.Insert(sb.ToString());
            return repId > 0 && subResult > 0;
        }

        private string GetGoodsRecordSql(GoodsRecord item)
        {
            string insertFormat = @" insert into tb_goodsrecord( FirstIn,FirstSale,FirstWaste,ThirdIn,ThirdSale,ThirdWaste,
                                                                 goods_id,dialyreport_id) 
                                 values({0},{1},{2},{3},{4},{5},{6},{7});";
            string sql = string.Format(insertFormat, item.FirstIn, item.FirstSale, item.FirstWaste, item.ThirdIn, item.ThirdSale,
                                                     item.ThirdWaste, item.Goods_Id, item.DialyReport_Id);
            return sql;
        }

        public bool InsertGoodsRecord(GoodsRecord item)
        {
            string sql = GetGoodsRecordSql(item);
            int result = dbHelper.Insert(sql);
            return result > 0;
        }

        public bool InsertGoods(Goods item)
        {
            string insertFormat = @" insert into tb_goods( Name,Type) 
                                 values('{0}',{1})";
            string sql = string.Format(insertFormat, item.Name, (int)item.Type);
            int result = dbHelper.Insert(sql);
            return result > 0;
        }

        #endregion


        #region Update



        #endregion

        #region Delete



        #endregion

        #region IDisposable Members

        protected void Disposing(bool isDisposed)
        {
            if (dbHelper != null)
            {
                dbHelper.Dispose();
            }
        }

        public void Dispose()
        {
            Disposing(true);
        }

        #endregion
    }
}
