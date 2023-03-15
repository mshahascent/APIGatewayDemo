USE [AROHA]
GO

/****** Object:  Table [dbo].[users]    Script Date: 03-03-2023 12:37:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[users]') AND type in (N'U'))
DROP TABLE [dbo].[users]
GO

/****** Object:  Table [dbo].[users]    Script Date: 03-03-2023 12:37:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[users](
	[id] [int] identity(1, 1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](50) NOT NULL,
	[companyid] [nchar](10) NOT NULL,
	[firstname] [varchar](250) NOT NULL,
	[middlename] [varchar](50) NULL,
	[lastname] [varchar](250) NOT NULL,
	[email] [varchar](250) NOT NULL,
	[address] [varchar](250) NOT NULL,
	[gender] [varchar](20) NOT NULL,
	[dateofbirth] [date] NULL,
	[ssn] [varchar](50) NULL,
	[createdby] [int] NOT NULL,
	[created_date] [datetime]  NOT NULL,
	[updatedby] [int]  NULL,
	[updated_date] [datetime]  NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


