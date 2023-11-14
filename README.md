# Perfume-shop

Online shop

# Run

1. Download project/git pull main
2. set default project as Domain,
3. start migration with `update-database` command typed in nuget console
4. In database run query:
   SET IDENTITY_INSERT dbo.ProductCategories ON;
   insert into dbo.ProductCategories(Id, Name) values (1, 'catA');
   insert into dbo.ProductCategories(Id, Name) values (2, 'catB');
   insert into dbo.ProductCategories(Id, Name) values (3, 'catC');
