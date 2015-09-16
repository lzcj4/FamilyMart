select A.id as '序号',C.saledate as '销售日期',B.name as '品名', 
       A.FirstIn as '一配进',A.FirstSale as '一配销',A.FirstWaste as '一配废',
       A.ThirdIn as '三配进',A.ThirdSale as '三配销',A.ThirdWaste as '三配废'
from tb_goodsrecord as A,tb_goods as B,tb_dialyreport as C
where A.goods_id=B.id and A.dialyreport_id= C.id
and C.id=6