CREATE TABLE [dbo].[Zip] (
    [Zip_ID]       INT           IDENTITY (1, 1) NOT NULL,
    [Zip]          VARCHAR (200) NULL,
    [ZipGroup1]    VARCHAR (200) NULL,
    [ZipSubGroup1] VARCHAR (200) NULL,
    CONSTRAINT [PK_Zip] PRIMARY KEY CLUSTERED ([Zip_ID] ASC)
);

