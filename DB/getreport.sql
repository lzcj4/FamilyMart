select id as '序号', saledate  as '销售日期',amount as '金额',customer as '客户',
       waste as '损耗',parttimeemployee as '兼职',consumeableamount  as '易耗品',
       Electrictcharge  as '电费',problem  as '问题'from tb_dialyreport
order by saledate
