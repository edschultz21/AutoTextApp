INSERT [dbo].[Agent] ([AID], [AgentId], [Name]) VALUES (1, N'ANDREI', N'Andrei')
GO
INSERT [dbo].[Agent] ([AID], [AgentId], [Name]) VALUES (2, N'BOB', N'Bob Smith')
GO
INSERT [dbo].[Listing] ([LID], [ListingNum], [ListPrice]) VALUES (1, N'L1', 100000)
GO
INSERT [dbo].[Listing] ([LID], [ListingNum], [ListPrice]) VALUES (2, N'L2', 200000)
GO
INSERT [dbo].[Listing] ([LID], [ListingNum], [ListPrice]) VALUES (3, N'L3', 300000)
GO
SET IDENTITY_INSERT [dbo].[Listing_Agent] ON 

GO
INSERT [dbo].[Listing_Agent] ([Id], [LID], [AID], [OID], [Side]) VALUES (1, 1, 1, NULL, N'SELL')
GO
INSERT [dbo].[Listing_Agent] ([Id], [LID], [AID], [OID], [Side]) VALUES (2, 1, 2, NULL, N'SELL')
GO
INSERT [dbo].[Listing_Agent] ([Id], [LID], [AID], [OID], [Side]) VALUES (3, 2, 1, NULL, N'BUY')
GO
INSERT [dbo].[Listing_Agent] ([Id], [LID], [AID], [OID], [Side]) VALUES (4, 3, 1, NULL, N'SELL')
GO
INSERT [dbo].[Listing_Agent] ([Id], [LID], [AID], [OID], [Side]) VALUES (5, 1, 2, NULL, N'BUY')
GO
SET IDENTITY_INSERT [dbo].[Listing_Agent] OFF
GO