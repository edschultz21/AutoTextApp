CREATE TABLE [dbo].[Agent] (
    [AID]     INT           NOT NULL,
    [AgentId] VARCHAR (100) NULL,
    [Name]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Agent] PRIMARY KEY CLUSTERED ([AID] ASC)
);

