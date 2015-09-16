
namespace DAL.Model
{
    /// <summary>
    /// 一配，三配数据
    /// </summary>
    public class GoodsRecord
    {
        public int Id { get; set; }

        public double FirstIn { get; set; }
        public double FirstSale { get; set; }
        public double FirstWaste { get; set; }

        public double ThirdIn { get; set; }
        public double ThirdSale { get; set; }
        public double ThirdWaste { get; set; }

        internal int Goods_Id { get { return this.Goods.Id; } }
        internal int DialyReport_Id { get { return this.DialyReport.Id; } }

        public Goods Goods { get; set; }
        public DialyReport DialyReport { get; set; }
    }
}
