drop table if exists tb_goods;
create table if not exists tb_goods
(
 Id integer primary key autoincrement ,
 Name varchar(100) not null unique,
 Type integer default 0
);

insert into tb_goods(Name,Type) values('OC包含果汁',0);
insert into tb_goods(Name,Type) values('中岛柜',0);
insert into tb_goods(Name,Type) values('关东煮',0);
insert into tb_goods(Name,Type) values('蒸包',0);

insert into tb_goods(Name,Type) values('盒饭',1);
insert into tb_goods(Name,Type) values('饭团',1);
insert into tb_goods(Name,Type) values('三明治',1);
insert into tb_goods(Name,Type) values('寿司',1);
insert into tb_goods(Name,Type) values('调理面',1);
insert into tb_goods(Name,Type) values('面包',1);

insert into tb_goods(Name,Type) values('集享卡',0);
insert into tb_goods(Name,Type) values('+2元得康师傅饮品',0);
insert into tb_goods(Name,Type) values('+5元维他椰子水',0);
insert into tb_goods(Name,Type) values('哈根达斯小纸杯',0);
insert into tb_goods(Name,Type) values('冰淇淋',0);
insert into tb_goods(Name,Type) values('咖茶',0);

insert into tb_goods(Name,Type) values('物流箱',2);

--new add goods
insert into tb_goods(Name,Type) values('+4.5元香蕉牛奶',0);
insert into tb_goods(Name,Type) values('+5元乐事薯片',0);
insert into tb_goods(Name,Type) values('+5元巧克力',0);
insert into tb_goods(Name,Type) values('+5上海记忆',0);
insert into tb_goods(Name,Type) values('岛柜',0);

drop table if exists tb_dialyreport;
create table if not exists tb_dialyreport
(
 Id integer primary key autoincrement ,
 SaleDate varchar(30) not null,
 Amount decimal,
 Customer integer,
 Waste decimal,
 ParttimeEmployee decimal,
 Employee decimal,
 PackingMaterialAmount decimal,
 ConsumeableAmount decimal,
 ElectrictCharge decimal,
 Problem varchar(200),
 Weather varchar(10)
);

drop table if exists tb_goodsrecord;
create table if not exists tb_goodsrecord
(
 Id integer primary key autoincrement ,
 FirstIn decimal,
 FirstSale decimal,
 FirstWaste decimal,
 ThirdIn decimal,
 ThirdSale decimal,
 ThirdWaste decimal,
 goods_id int,
 dialyreport_id int,
 foreign key(goods_id) references tb_goods(Id),
 foreign key(dialyreport_id) references tb_dialyreport(id)
);
