Use db37262

Select * From AspNetUsers
Select * From Connections

--Alter Table AspNetUsers
--Alter Column DateOfBirth Date



SELECT DATABASEPROPERTYEX(DB_NAME(), 'Collation');
--Alter Table AspNetUsers
--Alter Column Introduction nvarchar(Max)

--Drop table Photos
--Drop table Messages
--Drop table Likes
--Drop table Groups
--Drop table Connections
--Drop table AspNetUserTokens
--Drop table AspNetRoles
--Drop table AspNetUsers
--Drop table AspNetUserRoles
--Drop table AspNetUserLogins
--Drop table AspNetUserClaims
--Drop table AspNetRoleClaims

-- Source - https://stackoverflow.com/a
-- Posted by Gabriel GM, modified by community. See post 'Timeline' for change history
-- Retrieved 2026-01-11, License - CC BY-SA 4.0

--DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR

--SET @Cursor = CURSOR FAST_FORWARD FOR
--SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_SCHEMA + '].[' +  tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + '];'
--FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
--LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME

--OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

--WHILE (@@FETCH_STATUS = 0)
--BEGIN
--Exec sp_executesql @Sql
--FETCH NEXT FROM @Cursor INTO @Sql
--END

--CLOSE @Cursor DEALLOCATE @Cursor
--GO

--EXEC sp_MSforeachtable 'DROP TABLE ?'
--GO
