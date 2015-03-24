use [master];

if db_id('SqlFramework') is not null
begin
	alter database [SqlFramework] set single_user with rollback immediate;
	drop database [SqlFramework];
end