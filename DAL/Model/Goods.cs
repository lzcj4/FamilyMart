
namespace DAL.Model
{
    public enum GoodsType
    {
        /// <summary>
        /// 一般商品： OC包含果汁36
        /// </summary>
        NormalGoods = 0,

        /// <summary>
        /// 带一、三配商品：饭团一配10/9/1三配6/6/0
        /// </summary>
        DeliveryGoods = 1,
        /// <summary>
        /// 物流箱：  物流箱进/出/在店0/0/10
        /// </summary>
        Box=2
    }

    public class Goods
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GoodsType Type { get; set; }
    }
}
