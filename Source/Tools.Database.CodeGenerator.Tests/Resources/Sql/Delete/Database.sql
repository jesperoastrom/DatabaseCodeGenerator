use [master];

if db_id('CodeGeneratorTests') is not null
begin
	alter database [CodeGeneratorTests] set single_user with rollback immediate;
	drop database [CodeGeneratorTests];
end