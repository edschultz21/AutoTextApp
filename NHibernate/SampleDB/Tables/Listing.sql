CREATE TABLE [dbo].[Listing] (
    [LID]        INT           NOT NULL,
    [Zip_ID]     INT           NULL,
    [ListingNum] VARCHAR (100) NULL,
    [ListPrice]  INT           NULL,
    CONSTRAINT [PK_Listing] PRIMARY KEY CLUSTERED ([LID] ASC)
);



