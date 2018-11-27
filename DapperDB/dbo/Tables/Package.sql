CREATE TABLE [dbo].[Packages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [CreationTime] DATETIME NOT NULL, 
    [UserId] INT NOT NULL, 

    CONSTRAINT [FK_Packages_Users] FOREIGN KEY (UserId) REFERENCES [Users]([Id]) 
)