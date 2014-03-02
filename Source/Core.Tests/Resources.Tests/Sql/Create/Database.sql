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

create type Core.Name from nvarchar(100);
go

create type Core.Age from tinyint not null;
go

create type Core.LargeTableIds as table (
	Id int not null primary key
);
go

create type Core.SmallTableRows as table (
	Id int,
	Name Core.Name,
	Age Core.Age
);
go

create table Core.LargeTable (
	Id int not null identity(1,1)
		constraint PK_LargeTable primary key clustered,
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
	SomeGuid uniqueidentifier,
	Name Core.Name,
	Age Core.Age
);
go


create table Core.SmallTable (
	Id int not null identity(1,1)
		constraint PK_SmallTable primary key clustered,
	Name Core.Name,
	Age Core.Age
);
go

create procedure Core.GetAllLargeTableItems
as
begin
	select *
	from Core.LargeTable;
end
go

create procedure Core.GetAllLargeTableItemsById (
	@Id int
)
as
begin
	select *
	from Core.LargeTable
	where Id = @Id;
end
go

create procedure Core.GetAllLargeTableItemsByIds (
	@Ids Core.LargeTableIds readonly
)
as
begin
	select *
	from Core.LargeTable
	where Id in (
		select Id
		from @Ids
	);
end
go

create procedure Core.SaveSmallTableRows (
	@Rows as Core.SmallTableRows readonly
)
as
begin
	merge Core.SmallTable as target
	using (
		select * 
		from @Rows
	) as source
	on target.Id = source.Id
	when matched then 
		update set 
			target.Name = source.Name,
			target.Age = source.Age
	when not matched
	then insert (
		Name,
		Age
	) values (
		source.Name,
		source.Age
	);
end
go