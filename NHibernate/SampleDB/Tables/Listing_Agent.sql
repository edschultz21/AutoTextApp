CREATE TABLE [dbo].[Listing_Agent] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [LID]  INT           NULL,
    [AID]  INT           NULL,
    [OID]  INT           NULL,
    [Side] VARCHAR (100) NULL,
    CONSTRAINT [PK_Listing_Agent] PRIMARY KEY CLUSTERED ([Id] ASC)
);

