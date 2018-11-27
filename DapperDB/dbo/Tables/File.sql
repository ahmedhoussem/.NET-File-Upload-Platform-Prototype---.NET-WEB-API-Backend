CREATE TABLE [dbo].[Files]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Filename] VARCHAR(MAX) NOT NULL, 
    [PackageId] INT NOT NULL,

	[GeneratedFilename] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_Files_Packages] FOREIGN KEY (PackageId) REFERENCES [Packages]([Id]) 
)