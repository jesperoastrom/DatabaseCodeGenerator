use [master];

if db_id('CodeGeneratorTests') is not null
begin
	alter database [CodeGeneratorTests] set single_user with rollback immediate;
	drop database [CodeGeneratorTests];
end
go

create database [CodeGeneratorTests];
alter database  [CodeGeneratorTests] set recovery simple;
alter database  [CodeGeneratorTests] set compatibility_level = 100;
go

use [CodeGeneratorTests];
go

create schema Core;
go

create type Core.TestTableIds as table (
	Id int not null primary key
);
go

create table Core.TestTable (
	Id int not null
		constraint PK_TestTable primary key clustered,
	SomeBit bit,
	SomeTinyInt tinyint,
	SomeSmallInt SmallInt,
	SomeInt int,
	SomeBigInt bigint,
	SomeDecimal decimal(9, 3),
	SomeMoney money,
	SomeSmallMoney smallmoney,
	SomeSmallDateTime smalldatetime,
	SomeDateTime datetime,
	SomeChar char(10),
	SomeNChar nchar(10),
	SomeVarChar varchar(10),
	SomeNVarChar nvarchar(10),
	SomeBinary binary(10),
	SomeVarBinary varbinary(10),
	SomeTimestamp timestamp,
	SomeGuid uniqueidentifier
);
go

create procedure Core.GetAllTestTableItems
as
begin
	select *
	from Core.TestTable;
end
go

create procedure Core.GetAllTestTableItemsById (
	@Id int
)
as
begin
	select *
	from Core.TestTable
	where Id = @Id;
end
go

create procedure Core.GetAllTestTableItemsByIds (
	@Ids Core.TestTableIds readonly
)
as
begin
	select *
	from Core.TestTable
	where Id in (
		select Id
		from @Ids
	);
end
go
