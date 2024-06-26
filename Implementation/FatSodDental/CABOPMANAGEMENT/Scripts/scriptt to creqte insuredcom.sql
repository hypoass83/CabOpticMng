USE [ValdozInventory]
GO
/****** Object:  Table [dbo].[InsuredCompanies]    Script Date: 9/2/2019 8:35:04 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsuredCompanies]') AND type in (N'U'))
DROP TABLE [dbo].[InsuredCompanies]
GO
/****** Object:  Table [dbo].[InsuredCompanies]    Script Date: 9/2/2019 8:35:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsuredCompanies]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InsuredCompanies](
	[InsuredCompanyID] [int] IDENTITY(1,1) NOT NULL,
	[InsuredCompanyCode] [nvarchar](100) NOT NULL,
	[InsuredCompanyLabel] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.InsuredCompanies] PRIMARY KEY CLUSTERED 
(
	[InsuredCompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[InsuredCompanies] ON 

GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (1, N'233396', N'233396')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (2, N'ACEFA', N'ACEFA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (3, N'ACMS', N'ACMS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (4, N'ACTION CONTRE LA FAIM', N'ACTION CONTRE LA FAIM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (5, N'ACTIVA ASSURANCES', N'ACTIVA ASSURANCES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (6, N'ADAF', N'ADAF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (7, N'ADAX  PETROLEUM', N'ADAX  PETROLEUM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (8, N'ADC  SA', N'ADC  SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (9, N'ADDAX', N'ADDAX')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (10, N'ADEF', N'ADEF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (11, N'ADVANS CAMEROOM', N'ADVANS CAMEROOM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (12, N'ADVAS', N'ADVAS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (13, N'AEC SARL', N'AEC SARL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (14, N'AER', N'AER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (15, N'AFD', N'AFD')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (16, N'AFFIRMATIVE ACTION', N'AFFIRMATIVE ACTION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (17, N'AFRICA FOOD', N'AFRICA FOOD')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (18, N'AFRICA SECURITY', N'AFRICA SECURITY')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (19, N'AFRICA SERVICE', N'AFRICA SERVICE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (20, N'AFRILAND FIRST BANK', N'AFRILAND FIRST BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (21, N'AGC ASSURANCES', N'AGC ASSURANCES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (22, N'AGGREKO', N'AGGREKO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (23, N'AGUF', N'AGUF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (24, N'AIR COTE D''IVOIRE', N'AIR COTE D''IVOIRE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (25, N'ALLIANZ CONSTRUCTION', N'ALLIANZ CONSTRUCTION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (26, N'ALLIANZ MARQUE', N'ALLIANZ MARQUE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (27, N'ALOIS FINANCE', N'ALOIS FINANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (28, N'AMADEUS', N'AMADEUS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (29, N'AMBASSADE DE FRANCE', N'AMBASSADE DE FRANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (30, N'ANTIC', N'ANTIC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (31, N'APEZ', N'APEZ')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (32, N'APN', N'APN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (33, N'AREA ASSURANCE CAMEROON', N'AREA ASSURANCE CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (34, N'ARMP', N'ARMP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (35, N'ART', N'ART')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (36, N'ASCOMA CAMEROON', N'ASCOMA CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (37, N'ASECNA', N'ASECNA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (38, N'ASSEMBLE NATIONALE DU CAMEROUN', N'ASSEMBLE NATIONALE DU CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (39, N'ASSURANCES GENERALES DU CAMEROUN', N'ASSURANCES GENERALES DU CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (40, N'ATC', N'ATC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (41, N'ATTIJARI SECURITIES', N'ATTIJARI SECURITIES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (42, N'AUDITEC FOIRIER CONSULTING S A', N'AUDITEC FOIRIER CONSULTING S A')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (43, N'AWF', N'AWF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (44, N'AXA ASSURANCE', N'AXA ASSURANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (45, N'BANK CAMEROUNAISE DES PME', N'BANK CAMEROUNAISE DES PME')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (46, N'BANQUE ATLANTIQUE', N'BANQUE ATLANTIQUE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (47, N'BANQUE CAMEROUNAISE DES PME', N'BANQUE CAMEROUNAISE DES PME')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (48, N'BAT', N'BAT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (49, N'BATI SERVICE', N'BATI SERVICE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (50, N'BEAC', N'BEAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (51, N'BELGIAN EMBASSY', N'BELGIAN EMBASSY')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (52, N'BENEFICIAL LIFE INSURANCE', N'BENEFICIAL LIFE INSURANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (53, N'BGFI BANK', N'BGFI BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (54, N'BMN', N'BMN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (55, N'BOCOM', N'BOCOM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (56, N'BOLLORE TRANSPORT ET L.C.', N'BOLLORE TRANSPORT ET L.C.')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (57, N'BRAND AND CONSUMERS', N'BRAND AND CONSUMERS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (58, N'BRITISH AMERICAN TOBACCO', N'BRITISH AMERICAN TOBACCO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (59, N'BUCREP', N'BUCREP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (60, N'CAISSE AUTONOME  D''AMORTISSEMENT DU CAMEROUN', N'CAISSE AUTONOME  D''AMORTISSEMENT DU CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (61, N'CAM WATER', N'CAM WATER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (62, N'CAMAIR CO ', N'CAMAIR CO ')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (63, N'CAMEL', N'CAMEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (64, N'CAMI TOYATA', N'CAMI TOYATA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (65, N'CAMINEX', N'CAMINEX')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (66, N'CAMNAFAW', N'CAMNAFAW')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (67, N'CAMPOST', N'CAMPOST')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (68, N'CAMRAIL', N'CAMRAIL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (69, N'CAMTEL', N'CAMTEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (70, N'CANAL PLUS', N'CANAL PLUS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (71, N'CANOPY', N'CANOPY')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (72, N'CARE AND HEATH PROGRAM', N'CARE AND HEATH PROGRAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (73, N'CARE CAMEROUN', N'CARE CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (74, N'CARPA', N'CARPA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (75, N'CASINO', N'CASINO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (76, N'CAT', N'CAT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (77, N'CBC BANK', N'CBC BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (78, N'CBM CAMEROON', N'CBM CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (79, N'CCA BANK', N'CCA BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (80, N'CCAA', N'CCAA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (81, N'CCC PLC', N'CCC PLC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (82, N'CCPC', N'CCPC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (83, N'CEC PROM', N'CEC PROM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (84, N'CECAW', N'CECAW')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (85, N'CEGELEC', N'CEGELEC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (86, N'CELLULE BAD/BM-MINTP', N'CELLULE BAD/BM-MINTP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (87, N'CENAME', N'CENAME')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (88, N'CENEEMA', N'CENEEMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (89, N'CENTRE PASTEUR DU CAMEROUN', N'CENTRE PASTEUR DU CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (90, N'CEPI SA', N'CEPI SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (91, N'CFA', N'CFA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (92, N'CFAO TECHNOLOGIES', N'CFAO TECHNOLOGIES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (93, N'CHANAS ASSURANCE', N'CHANAS ASSURANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (94, N'CHEMONICS INTERNATIONAL', N'CHEMONICS INTERNATIONAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (95, N'CHOCOCAM', N'CHOCOCAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (96, N'CIBLE RH', N'CIBLE RH')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (97, N'CIC CACAO', N'CIC CACAO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (98, N'CICAM', N'CICAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (99, N'CICR', N'CICR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (100, N'CIDR', N'CIDR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (101, N'CIMA INTERNATIONAL', N'CIMA INTERNATIONAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (102, N'CIMACAM', N'CIMACAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (103, N'CIMENCAM', N'CIMENCAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (104, N'CIRCB', N'CIRCB')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (105, N'CITRACEL', N'CITRACEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (106, N'CITY SPORT', N'CITY SPORT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (107, N'CNLS', N'CNLS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (108, N'CNPS', N'CNPS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (109, N'COLGATE PALMOLIVE', N'COLGATE PALMOLIVE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (110, N'COMPANIE FORESTIERE DE KRIBI', N'COMPANIE FORESTIERE DE KRIBI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (111, N'CONAC', N'CONAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (112, N'CONGREGATION DE LA MISSION', N'CONGREGATION DE LA MISSION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (113, N'CONTINENTAL-RE', N'CONTINENTAL-RE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (114, N'COTCO', N'COTCO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (115, N'COTRAVA', N'COTRAVA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (116, N'CREDIT FONCIER DU CAMEROUN', N'CREDIT FONCIER DU CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (117, N'CREMENCAM', N'CREMENCAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (118, N'CREOLINK', N'CREOLINK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (119, N'CRIMENCAM', N'CRIMENCAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (120, N'CROISSANT ROUGE', N'CROISSANT ROUGE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (121, N'CROIX ROUGE', N'CROIX ROUGE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (122, N'CROIX ROUGE FRANCAISE', N'CROIX ROUGE FRANCAISE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (123, N'CRS', N'CRS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (124, N'CRTV', N'CRTV')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (125, N'DAFI SCHOLARSHIP', N'DAFI SCHOLARSHIP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (126, N'DAL', N'DAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (127, N'DANG REMEMBER', N'DANG REMEMBER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (128, N'DANGOTE', N'DANGOTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (129, N'DANO COMMUNICATION', N'DANO COMMUNICATION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (130, N'DHL INTERNATIONAL', N'DHL INTERNATIONAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (131, N'DIASPORA SANTE', N'DIASPORA SANTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (132, N'DKT', N'DKT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (133, N'DM', N'DM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (134, N'DOHONE', N'DOHONE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (135, N'DOMAYO CAM', N'DOMAYO CAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (136, N'DOUALAIR', N'DOUALAIR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (137, N'DR NGOUFO', N'DR NGOUFO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (138, N'DTP TERRASSEMENT', N'DTP TERRASSEMENT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (139, N'ECART SERVICE', N'ECART SERVICE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (140, N'ECHO', N'ECHO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (141, N'ECOLE DE SCIENCE DE LA SANTE', N'ECOLE DE SCIENCE DE LA SANTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (142, N'ECOLE PRIVEE CATHOLIQUE', N'ECOLE PRIVEE CATHOLIQUE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (143, N'EDC', N'EDC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (144, N'EFAB CORPORATE', N'EFAB CORPORATE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (145, N'EGIS CAMEROON', N'EGIS CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (146, N'ELECAM', N'ELECAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (147, N'ELIS CAMEROON', N'ELIS CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (148, N'ELIZABETH GLASER PEDIATRIC AIDS FOUNDATION', N'ELIZABETH GLASER PEDIATRIC AIDS FOUNDATION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (149, N'EMPLOI SERVICE', N'EMPLOI SERVICE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (150, N'ENEO CAMEROON SA', N'ENEO CAMEROON SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (151, N'EPC', N'EPC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (152, N'EPCIY', N'EPCIY')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (153, N'EPRESS SERVICE AND PRINT', N'EPRESS SERVICE AND PRINT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (154, N'ERA CAMEROON', N'ERA CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (155, N'ERICSSON CAMEROUN', N'ERICSSON CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (156, N'ETS LAFORME', N'ETS LAFORME')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (157, N'EUROPE AFRIQUE INTERIM', N'EUROPE AFRIQUE INTERIM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (158, N'EUROTECH', N'EUROTECH')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (159, N'EXPRESS SERVICES AND PRINT', N'EXPRESS SERVICES AND PRINT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (160, N'FAIRMED', N'FAIRMED')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (161, N'FAYADORT SA', N'FAYADORT SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (162, N'FECAFOOT', N'FECAFOOT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (163, N'FEDEC', N'FEDEC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (164, N'FEICOM', N'FEICOM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (165, N'FERRERO CAMEROUN S.A', N'FERRERO CAMEROUN S.A')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (166, N'FICR CROISSANT ROUGE', N'FICR CROISSANT ROUGE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (167, N'FNE', N'FNE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (168, N'FODECC', N'FODECC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (169, N'FODER', N'FODER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (170, N'FOND ROUTIER', N'FOND ROUTIER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (171, N'FONDATION INTER PROGRESS', N'FONDATION INTER PROGRESS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (172, N'FOUSTEL DE COULANGES', N'FOUSTEL DE COULANGES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (173, N'FRERRO', N'FRERRO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (174, N'G H S S', N'G H S S')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (175, N'GECEFIC', N'GECEFIC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (176, N'GEFI', N'GEFI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (177, N'GENIE MILITAIRE', N'GENIE MILITAIRE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (178, N'GIMAC', N'GIMAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (179, N'GIZ', N'GIZ')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (180, N'GLOBAL HEALTH', N'GLOBAL HEALTH')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (181, N'GPAC', N'GPAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (182, N'GRAPHIC SYSTEM', N'GRAPHIC SYSTEM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (183, N'GROUPE KAM', N'GROUPE KAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (184, N'GRUPPO PICCINI SA', N'GRUPPO PICCINI SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (185, N'GULFIN', N'GULFIN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (186, N'HAAWAS MEDIA', N'HAAWAS MEDIA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (187, N'HALLIBULTON', N'HALLIBULTON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (188, N'HAVAS AFRICA', N'HAVAS AFRICA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (189, N'HELEN KELLER INTERNATIONAL', N'HELEN KELLER INTERNATIONAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (190, N'HENRI ET FRERES', N'HENRI ET FRERES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (191, N'HILTON HOTEL', N'HILTON HOTEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (192, N'HOTEL MONT FEBE', N'HOTEL MONT FEBE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (193, N'HSD HUMAN RESOURCE SOLUTIONS', N'HSD HUMAN RESOURCE SOLUTIONS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (194, N'HUAWEI', N'HUAWEI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (195, N'HUAWEITECHNOLONOGIES', N'HUAWEITECHNOLONOGIES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (196, N'HYDRACK', N'HYDRACK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (197, N'HYDROMINE CAMEROON LIMITED', N'HYDROMINE CAMEROON LIMITED')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (198, N'HYGRAC', N'HYGRAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (199, N'HYSACAM', N'HYSACAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (200, N'I N S', N'I N S')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (201, N'ICAP', N'ICAP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (202, N'ICRAFT', N'ICRAFT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (203, N'IED INVEST CAM', N'IED INVEST CAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (204, N'IFC', N'IFC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (205, N'IFORD', N'IFORD')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (206, N'IHS CAMEROUN', N'IHS CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (207, N'IITA CAMEROON', N'IITA CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (208, N'IMEX PHARMA', N'IMEX PHARMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (209, N'INC', N'INC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (210, N'INST. DE PHILO ST. J. MUKASSA', N'INST. DE PHILO ST. J. MUKASSA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (211, N'INTEK SARL', N'INTEK SARL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (212, N'INTEL', N'INTEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (213, N'INTELHRC', N'INTELHRC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (214, N'INTEX', N'INTEX')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (215, N'IPSJM', N'IPSJM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (216, N'ISSEA', N'ISSEA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (217, N'IUCN', N'IUCN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (218, N'JMJ AFRICA', N'JMJ AFRICA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (219, N'JRS', N'JRS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (220, N'KENYA AIRWAYS', N'KENYA AIRWAYS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (221, N'KOICA', N'KOICA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (222, N'KPDC', N'KPDC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (223, N'LABOREX', N'LABOREX')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (224, N'LANAVET', N'LANAVET')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (225, N'LDC CAMEROON', N'LDC CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (226, N'LFPC', N'LFPC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (227, N'LJD CONSULTING AND SERVICES LTD', N'LJD CONSULTING AND SERVICES LTD')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (228, N'LMT GROUP', N'LMT GROUP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (229, N'LOUIS BERGER', N'LOUIS BERGER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (230, N'LOXEA AVIS FLEET', N'LOXEA AVIS FLEET')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (231, N'MADZA', N'MADZA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (232, N'MAGZI', N'MAGZI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (233, N'MAN', N'MAN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (234, N'MAN SA', N'MAN SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (235, N'MARANATHA AU COEUR DU MONDE', N'MARANATHA AU COEUR DU MONDE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (236, N'MAZARS CAMEROON', N'MAZARS CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (237, N'MEDECINS CONSEIL ASCOMA', N'MEDECINS CONSEIL ASCOMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (238, N'MEDIA PLUS', N'MEDIA PLUS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (239, N'METABIOTA', N'METABIOTA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (240, N'MSF', N'MSF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (241, N'MTN CAMEROON', N'MTN CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (242, N'MUNEF-FNE', N'MUNEF-FNE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (243, N'MUPER CAMPOST', N'MUPER CAMPOST')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (244, N'MUPUF', N'MUPUF')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (245, N'N A', N'N A')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (246, N'NACHTIGAL ', N'NACHTIGAL ')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (247, N'NEPTUNE OIL', N'NEPTUNE OIL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (248, N'NESTLE', N'NESTLE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (249, N'NEW AGE', N'NEW AGE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (250, N'NEXTTEL', N'NEXTTEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (251, N'NFC BANK', N'NFC BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (252, N'NHPC', N'NHPC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (253, N'NILE DUTCH', N'NILE DUTCH')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (254, N'NORWAGIAN REFUGEE COUNCIL', N'NORWAGIAN REFUGEE COUNCIL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (255, N'NOVARTIS CAMEROUN', N'NOVARTIS CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (256, N'NSIA ASSURANCE', N'NSIA ASSURANCE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (257, N'NSIACAM', N'NSIACAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (258, N'OHADA', N'OHADA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (259, N'OLAMCAM', N'OLAMCAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (260, N'OMNIUM SERVICE', N'OMNIUM SERVICE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (261, N'ONCC', N'ONCC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (262, N'ONYX MANPOWER SERVICES CAMEROON SA', N'ONYX MANPOWER SERVICES CAMEROON SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (263, N'ORANGE CAMEROUN', N'ORANGE CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (264, N'P2M PHARMA', N'P2M PHARMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (265, N'PA3C', N'PA3C')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (266, N'PAEPSY', N'PAEPSY')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (267, N'PAEPYS', N'PAEPYS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (268, N'PALLISCO', N'PALLISCO')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (269, N'PAMOCCA', N'PAMOCCA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (270, N'PAN AFRICA', N'PAN AFRICA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (271, N'PARTENAIRE SANTE', N'PARTENAIRE SANTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (272, N'PARTICULIARE SANTE', N'PARTICULIARE SANTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (273, N'PASEM', N'PASEM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (274, N'PDST/CCAA', N'PDST/CCAA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (275, N'PDVIR', N'PDVIR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (276, N'PEA JEUNES', N'PEA JEUNES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (277, N'PERE MARISTE', N'PERE MARISTE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (278, N'PFIZER CAMEROUN', N'PFIZER CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (279, N'PHARMACIE DES CONGRES', N'PHARMACIE DES CONGRES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (280, N'PICTET CAM', N'PICTET CAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (281, N'PIGMA', N'PIGMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (282, N'PLAN CAMEROUN', N'PLAN CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (283, N'PLAN INTERNATIONAL', N'PLAN INTERNATIONAL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (284, N'PMUC', N'PMUC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (285, N'PNDP', N'PNDP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (286, N'PORT AUTONOME DE KRIBI', N'PORT AUTONOME DE KRIBI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (287, N'PPG CAMEROUN', N'PPG CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (288, N'PPSAC', N'PPSAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (289, N'PPSAC/OCEAC', N'PPSAC/OCEAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (290, N'PRECASEM', N'PRECASEM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (291, N'PRO ASSUR', N'PRO ASSUR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (292, N'PRODEL', N'PRODEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (293, N'PROJET CAB', N'PROJET CAB')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (294, N'PROJET MEMVELE', N'PROJET MEMVELE')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (295, N'PROJET SANAGA', N'PROJET SANAGA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (296, N'PSA', N'PSA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (297, N'PULCI', N'PULCI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (298, N'SABC', N'SABC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (299, N'SAHAM ASSURANCES', N'SAHAM ASSURANCES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (300, N'SAMAFILS', N'SAMAFILS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (301, N'SANDOZ WC AFRICA', N'SANDOZ WC AFRICA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (302, N'SANOFI', N'SANOFI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (303, N'SANOFILS', N'SANOFILS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (304, N'SANTE PARTICULIER', N'SANTE PARTICULIER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (305, N'SAPHIR', N'SAPHIR')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (306, N'SAPP', N'SAPP')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (307, N'SCB CAMEROUN', N'SCB CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (308, N'SCHLUMBERGER', N'SCHLUMBERGER')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (309, N'SCI HIBISCUS', N'SCI HIBISCUS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (310, N'SDMC SARL', N'SDMC SARL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (311, N'SEGELEC', N'SEGELEC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (312, N'SEPECAM', N'SEPECAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (313, N'SFID', N'SFID')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (314, N'SGC', N'SGC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (315, N'SGS.SA', N'SGS.SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (316, N'SIC CACAOS', N'SIC CACAOS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (317, N'SIGHT SAVERS', N'SIGHT SAVERS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (318, N'SIM', N'SIM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (319, N'SIMTECH-3D', N'SIMTECH-3D')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (320, N'SIRPACAM', N'SIRPACAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (321, N'SITRACEL', N'SITRACEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (322, N'SNI', N'SNI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (323, N'SNV CAMEROUN', N'SNV CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (324, N'SOCAPERIMA', N'SOCAPERIMA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (325, N'SOCATAM', N'SOCATAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (326, N'SOCIA', N'SOCIA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (327, N'SOCIETE IND. DE MBANG', N'SOCIETE IND. DE MBANG')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (328, N'SODICAM', N'SODICAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (329, N'SODICAM CITY SPORT', N'SODICAM CITY SPORT')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (330, N'SOLEVO CAM', N'SOLEVO CAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (331, N'SOMAC SA', N'SOMAC SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (332, N'SOMAFILS', N'SOMAFILS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (333, N'SONAFI', N'SONAFI')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (334, N'SONATREL', N'SONATREL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (335, N'SOPECAM', N'SOPECAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (336, N'SOQUICAM', N'SOQUICAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (337, N'SOTICAM', N'SOTICAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (338, N'SPM', N'SPM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (339, N'STANDARD CHARTERED BANK', N'STANDARD CHARTERED BANK')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (340, N'SUD CAMEROUN HEVEA SA', N'SUD CAMEROUN HEVEA SA')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (341, N'SUNU ASSURANCES', N'SUNU ASSURANCES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (342, N'TOTAL CAMEROON', N'TOTAL CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (343, N'TRACTAFRIC MOTORS CAMEROUN', N'TRACTAFRIC MOTORS CAMEROUN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (344, N'TRADIN CAM', N'TRADIN CAM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (345, N'TROPIK INDUSTRIES', N'TROPIK INDUSTRIES')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (346, N'UBA CAMEROON', N'UBA CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (347, N'UCAC', N'UCAC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (348, N'UCB', N'UCB')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (349, N'UDC', N'UDC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (350, N'UDM', N'UDM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (351, N'UICN', N'UICN')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (352, N'UNICS PLC', N'UNICS PLC')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (353, N'UNION BANK CAMEROON', N'UNION BANK CAMEROON')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (354, N'URANIUM', N'URANIUM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (355, N'VALDOZ', N'VALDOZ')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (356, N'VEO CANAL+', N'VEO CANAL+')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (357, N'VIETTEL', N'VIETTEL')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (358, N'VODACOM', N'VODACOM')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (359, N'WCS', N'WCS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (360, N'WEXFORD', N'WEXFORD')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (361, N'WOOD PRODUCTION', N'WOOD PRODUCTION')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (362, N'WORLD WILD FUND', N'WORLD WILD FUND')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (363, N'WWCS', N'WWCS')
GO
INSERT [dbo].[InsuredCompanies] ([InsuredCompanyID], [InsuredCompanyCode], [InsuredCompanyLabel]) VALUES (364, N'YUP', N'YUP')
GO
SET IDENTITY_INSERT [dbo].[InsuredCompanies] OFF
GO
