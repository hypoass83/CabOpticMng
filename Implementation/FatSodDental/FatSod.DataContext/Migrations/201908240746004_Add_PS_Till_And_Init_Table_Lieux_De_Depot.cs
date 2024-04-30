namespace FatSod.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PS_Till_And_Init_Table_Lieux_De_Depot : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO dbo.LieuxdeDepotBorderoes(LieuxdeDepotBorderoName) VALUES ('DIRECTION GENERALE')");
            Sql(@"ALTER FUNCTION [dbo].[GenerateReportOperationPeriodeOfDayTill]
                (
                @To DateTime,
                @At DateTime,
                @tillID Int=NULL
                )
                RETURNS @TableGenerateReportOperationPeriodeOfDayTill TABLE 
                (
	                --PK NUMERIC (18,0) Identity(1,1) Primary Key,
	                ReportOperationPeriodeOfDayID Int Identity(1,1) Primary Key,
	                OperationDate Datetime,
	                BeginDate Datetime,
	                EndDate Datetime,
	                InputAmount FLOAT,
	                OutPutAmount Float,
	                Intervenant Varchar(500),
	                Solde Float,
	                Operation Char(50),
	                TransactionNumber Char(30),
	                Description varchar(500),
	                OpeningCashAmount Float,
	                ClosingCashAmount Float,
	                PaymentMethod Varchar(50),
	                CashRegisterName Varchar(250),
	                CashHandCloseDay Float
                )
                As
                Begin
	                Declare @dateop DATEtIME,@rowtill numeric,@COUNTSO NUMERIC,@BudgetAllocatedID Numeric,@TOTALRETURNAMT FLOAT,@vartillID Int,
	                @OperationDate Datetime,@BeginDate Datetime,@EndDate Datetime,@InputAmount FLOAT,@OutPutAmount Float,@Intervenant Varchar(500),
	                @Solde Float,@Operation Char(50),@TransactionNumber Char(30),@Description varchar(500),@CustomerReturnID Numeric,@NonPaidCashID numeric,
	                @OpeningCashAmount Float,@ClosingCashAmount Float,@PaymentMethod Varchar(50),@CashRegisterName Varchar(250),@CashHandCloseDay Float
	                DECLARE @SaleID NUMERIC,@SliceDate DATETIME,@SliceAmount FLOAT,@SaleReceiptNumber VARCHAR(255),@PersonName VARCHAR(255)
	                DECLARE @ReductionAmount FLOAT,@TotalPriceHT FLOAT,@TotalPriceTTC FLOAT,@TVAAmount FLOAT,@SaleDate DATE,@SaleFullInformation VARCHAR(255)

	                IF @tillID IS NULL OR @tillID=0
	                BEGIN
		                DECLARE Tills_Cursor CURSOR FOR  
		                SELECT ID  FROM dbo.Tills order by ID
	                END
	                ELSE
	                BEGIN
		                DECLARE Tills_Cursor CURSOR FOR  
		                SELECT ID  FROM dbo.Tills WHERE ID=@tillID order by ID
	                END

	                OPEN Tills_Cursor;  
	                FETCH NEXT FROM Tills_Cursor INTO @vartillID; 
	                WHILE @@FETCH_STATUS = 0  
	                BEGIN
		                SET @dateop=@To
		                WHILE (@dateop <= @At)
		                BEGIN
		                select @PaymentMethod=null,@CashRegisterName =null
		                select @PaymentMethod=p.Code,@CashRegisterName=p.Name+' '+p.Code from dbo.Tills t inner join dbo.PaymentMethods p on t.ID=p.ID where t.ID=@vartillID
		
		                select @OpeningCashAmount=0,@ClosingCashAmount=0,@CashHandCloseDay =0
		                select @OpeningCashAmount=TillDayOpenPrice,@ClosingCashAmount=TillDayClosingPrice,@CashHandCloseDay=TillDayCashHand from dbo.TillDays
		                where TillID=@vartillID and TillDayDate=@dateop
		                set @rowtill=@@ROWCOUNT
		                if @OpeningCashAmount is null set @OpeningCashAmount=0
		                if @ClosingCashAmount is null set @ClosingCashAmount=0
		                if @CashHandCloseDay is null set @CashHandCloseDay=0

		                if (@rowtill>0)
		                Begin
			                -- //1-reglement par caisse des ventes
			                SELECT @SaleID=NULL,@SliceDate=NULL,@SliceAmount=0
			                DECLARE ventes_Cursor CURSOR FOR  
			                SELECT CSL.SaleID, SliceDate,SliceAmount   
			                FROM dbo.CustomerSlices CSL INNER JOIN dbo.Slices SL
			                ON CSL.SliceID=SL.SliceID /*INNER JOIN dbo.Sales SA
			                ON SA.SaleID=CSL.SaleID */
			                WHERE SL.SliceDate =@dateop AND SL.PaymentMethodID=@vartillID AND CSL.isDeposit=0 --AND SA.IsPaid=1
			                OPEN ventes_Cursor;  
			                FETCH NEXT FROM ventes_Cursor INTO @SaleID,@SliceDate,@SliceAmount;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN
				                --//recuperation de la vente concerne
				                SELECT @SaleReceiptNumber=NULL,@PersonName=NULL,@SaleDate=NULL
				                SELECT @SaleReceiptNumber=SA.SaleReceiptNumber,@PersonName=sa.CustomerName,@SaleDate=SA.SaleDate FROM dbo.Sales SA INNER JOIN
				                dbo.GlobalPeople C ON SA.CustomerID=C.GlobalPersonID WHERE SA.SaleID=@SaleID
				                --//verif if special order
				                SET @COUNTSO=0
				
				                IF @COUNTSO=0 SET @Operation='SALE'
				                ELSE SET @Operation='SPECIAL LENS ORDER'

				                SELECT @ReductionAmount=NULL,@TotalPriceHT=NULL,@TotalPriceTTC=NULL,@TVAAmount=NULL
				                SELECT @ReductionAmount=ReductionAmount,@TotalPriceHT=TotalPriceHT,@TotalPriceTTC=TotalPriceTTC,@TVAAmount=TVAAmount 
				                FROM DBO.ApplyExtraPriceSale(@SALEID);
				                IF @ReductionAmount IS NULL SET @ReductionAmount=0
				                IF @TotalPriceHT IS NULL SET @TotalPriceHT=0
				                IF @TotalPriceTTC IS NULL SET @TotalPriceTTC=0
				                IF @TVAAmount IS NULL SET @TVAAmount=0

				                SET @SaleFullInformation=NULL
				                SET @SaleFullInformation=@SaleReceiptNumber+' '+CONVERT(VARCHAR,@SaleDate,103)+' '+CONVERT(VARCHAR,@TotalPriceTTC)
				
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SliceDate,@To,@At,@SliceAmount,0,@PersonName,@TotalPriceTTC,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM ventes_Cursor INTO @SaleID,@SliceDate,@SliceAmount;  
			                END;  
			                CLOSE ventes_Cursor;  
			                DEALLOCATE ventes_Cursor; 
			                --//2-tous les depots par la caisse ----depot d'epargne par caisse
			                SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			                DECLARE Deposit_Cursor CURSOR FOR  
			                SELECT AllDepositDate,Amount,AllDepositReference,AllDepositReason,Name  
			                FROM dbo.AllDeposits dp INNER JOIN dbo.GlobalPeople c on dp.CustomerID=c.GlobalPersonID  WHERE dp.AllDepositDate =@dateop AND dp.PaymentMethodID=@vartillID
			                OPEN Deposit_Cursor;  
			                FETCH NEXT FROM Deposit_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN 
				
				                SET @Operation='DEPOSIT'
	
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SaleDate,@To,@At,@SliceAmount,0,@PersonName,@SliceAmount,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM Deposit_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			                END;  
			                CLOSE Deposit_Cursor;  
			                DEALLOCATE Deposit_Cursor; 

			                --3--//SURPLUS DE CAISSE PAR AJUSTEMENT
			                SELECT @SaleDate=NULL,@SliceAmount=0,@SaleFullInformation=NULL,	@PersonName=NULL
			                DECLARE Till_Cursor CURSOR FOR  
			                SELECT TillAdjustDate,PhysicalPrice - ComputerPrice,Justification,Justification  
			                FROM dbo.TillAdjusts ta WHERE ta.TillAdjustDate =@dateop AND ta.TillID=@vartillID
			                OPEN Till_Cursor; 
			                FETCH NEXT FROM Till_Cursor INTO @SaleDate,@SliceAmount,@SaleFullInformation,@PersonName;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN 
				
				                IF @SliceAmount > 0 
				                BEGIN
					                SET @Operation='OVERAGE'
					                SET @SaleReceiptNumber='OVE'+CONVERT(VARCHAR,@vartillID)
					                SET @InputAmount=@SliceAmount
					                SET @OutPutAmount=0
					                SET @SoldE=-1*@SliceAmount
				                END
				                ELSE
				                BEGIN
					                SET @Operation='TELLER ADJUST'
					                SET @SaleReceiptNumber='TADJ'+CONVERT(VARCHAR,@vartillID)
					                SET @InputAmount=0
					                SET @OutPutAmount=-1*@SliceAmount
					                SET @SoldE=-1*@SliceAmount
				                END
	
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SaleDate,@To,@At,@InputAmount,@OutPutAmount,@PersonName,@Solde,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM Till_Cursor INTO @SaleDate,@SliceAmount,@SaleFullInformation,@PersonName;  
			                END;  
			                CLOSE Till_Cursor;  
			                DEALLOCATE Till_Cursor; 
			                --//OUTPUTS OPERATIONS HISTORIQUE
			                --4--//recupartion des consommation du budget
			                SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			                DECLARE Budget_Cursor CURSOR FOR  
			                SELECT PaymentDate,VoucherAmount,Reference,BudgetAllocatedID,BeneficiaryName+' / '+Justification  
			                FROM dbo.BudgetConsumptions bc WHERE bc.PaymentDate =@dateop AND bc.PaymentMethodID=@vartillID 
			                OPEN Budget_Cursor;  
			                FETCH NEXT FROM Budget_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@BudgetAllocatedID,@PersonName;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN 
				
				                SET @Operation='BUDGET CONSUMPTION'
				                select @SaleFullInformation=bl.BudgetLineLabel from dbo.BudgetLines bl inner join dbo.BudgetAllocateds ba
				                on ba.BudgetLineID=bl.BudgetLineID where ba.BudgetAllocatedID=@BudgetAllocatedID
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM Budget_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@BudgetAllocatedID,@PersonName;  
			                END;  
			                CLOSE Budget_Cursor;  
			                DEALLOCATE Budget_Cursor; 
			                --5--//recuperation des sorties vers banque
			                SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			                DECLARE Tranfbank_Cursor CURSOR FOR  
			                SELECT treope.OperationDate,treope.OperationAmount,treope.OperationRef,treope.Justification,treope.Justification  
			                FROM dbo.TreasuryOperations treope WHERE treope.OperationDate =@dateop AND treope.TillID=@vartillID and treope.OperationType='TransfertToBank'
			                OPEN Tranfbank_Cursor;  
			                FETCH NEXT FROM Tranfbank_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN 
				
				                SET @Operation='BANK TRANSFER'
	
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM Tranfbank_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			                END;  
			                CLOSE Tranfbank_Cursor;  
			                DEALLOCATE Tranfbank_Cursor;
			                --6--//recuperation des retours sur ventes
			                DECLARE @TotalUnpaid Float
			                SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL,@CustomerReturnID=null
			                DECLARE returnSlice_Cursor CURSOR FOR  
			                SELECT sl.SliceDate,sl.SliceAmount,rs.CustomerReturnID  
			                FROM dbo.CustomerReturnSlices rs INNER JOIN dbo.Slices sl on rs.SliceID=sl.SliceID 
			                WHERE sl.SliceDate =@dateop AND sl.PaymentMethodID=@vartillID 
			                OPEN returnSlice_Cursor;  
			                FETCH NEXT FROM returnSlice_Cursor INTO @SaleDate,@SliceAmount,@CustomerReturnID;  
			                WHILE @@FETCH_STATUS = 0  
			                BEGIN 
				                --print 'test return'
				                --//recuperation de la vente concerne
				                select @SaleID=0
				                select @SaleID=SaleID from dbo.CustomerReturnSlices crs inner join dbo.CustomerReturns cr on crs.CustomerReturnID=cr.CustomerReturnID
				                where crs.CustomerReturnID=@CustomerReturnID

				                SELECT @SaleFullInformation=NULL,@PersonName=NULL,@SaleReceiptNumber=null
				                SELECT @SaleFullInformation=C.Name,@PersonName=C.Name,@SaleReceiptNumber=sa.SaleReceiptNumber FROM dbo.Sales SA INNER JOIN
				                dbo.GlobalPeople C ON SA.CustomerID=C.GlobalPersonID  WHERE SA.SaleID=@SaleID

				                SET @Operation='SALE RETURN'
	
				                --insertion resultat final
				                Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				                EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				                OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName,CashHandCloseDay)
				                Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				                @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName,@CashHandCloseDay)

				                FETCH NEXT FROM returnSlice_Cursor INTO @SaleDate,@SliceAmount,@CustomerReturnID;  
			                END;  
			                CLOSE returnSlice_Cursor;  
			                DEALLOCATE returnSlice_Cursor;

		                End
		

		                ----------------------------------------------------------------------------
		                SET @dateop = DATEADD (day , 1 , @dateop )
		
		                END
		                FETCH NEXT FROM Tills_Cursor INTO @vartillID; 
	                END
	                CLOSE Tills_Cursor;  
	                DEALLOCATE Tills_Cursor; 
	
	                RETURN;
                end;");
        }
        
        public override void Down()
        {
            Sql(@"Delete From dbo.LieuxdeDepotBorderoes");
            Sql(@"ALTER FUNCTION [dbo].[GenerateReportOperationPeriodeOfDayTill]
            (
            @To DateTime,
            @At DateTime,
            @tillID Int=NULL
            )
            RETURNS @TableGenerateReportOperationPeriodeOfDayTill TABLE 
            (
	            --PK NUMERIC (18,0) Identity(1,1) Primary Key,
	            ReportOperationPeriodeOfDayID Int Identity(1,1) Primary Key,
	            OperationDate Datetime,
	            BeginDate Datetime,
	            EndDate Datetime,
	            InputAmount FLOAT,
	            OutPutAmount Float,
	            Intervenant Varchar(500),
	            Solde Float,
	            Operation Char(50),
	            TransactionNumber Char(30),
	            Description varchar(500),
	            OpeningCashAmount Float,
	            ClosingCashAmount Float,
	            PaymentMethod Varchar(50),
	            CashRegisterName Varchar(250)
            )
            As
            Begin
	            Declare @dateop DATEtIME,@rowtill numeric,@COUNTSO NUMERIC,@BudgetAllocatedID Numeric,@vartillID Int,
	            @OperationDate Datetime,@BeginDate Datetime,@EndDate Datetime,@InputAmount FLOAT,@OutPutAmount Float,@Intervenant Varchar(500),
	            @Solde Float,@Operation Char(50),@TransactionNumber Char(30),@Description varchar(500),@CustomerReturnID Numeric,
	            @OpeningCashAmount Float,@ClosingCashAmount Float,@PaymentMethod Varchar(50),@CashRegisterName Varchar(250),@TOTVAL numeric
	            DECLARE @SaleID NUMERIC,@SliceDate DATETIME,@SliceAmount FLOAT,@SaleReceiptNumber VARCHAR(255),@PersonName VARCHAR(255)
	            DECLARE @ReductionAmount FLOAT,@TotalPriceHT FLOAT,@TotalPriceTTC FLOAT,@TVAAmount FLOAT,@SaleDate DATE,@SaleFullInformation VARCHAR(255)

	            SET @TOTVAL=0
	            IF @tillID IS NULL OR @tillID=0
	            BEGIN
		            DECLARE Tills_Cursor CURSOR FOR  
		            SELECT ID  FROM dbo.Tills ORDER BY ID
		
	            END
	            ELSE
	            BEGIN
		            DECLARE Tills_Cursor CURSOR FOR  
		            SELECT ID  FROM dbo.Tills WHERE ID=@tillID  ORDER BY ID
	            END
	
	            OPEN Tills_Cursor;  
	            FETCH NEXT FROM Tills_Cursor INTO @vartillID;
	            WHILE @@FETCH_STATUS = 0  
	            BEGIN 

		            SET @dateop=@To
		            WHILE (@dateop <= @At)
		            BEGIN
			            select @PaymentMethod=null,@CashRegisterName =null
			            select @PaymentMethod=p.Code,@CashRegisterName=p.Name+' '+p.Code from dbo.Tills t inner join dbo.PaymentMethods p on t.ID=p.ID where t.ID=@vartillID
		
			            select @OpeningCashAmount=0,@ClosingCashAmount=0
		
			            -- //1-reglement par caisse des ventes
			            SELECT @SaleID=NULL,@SliceDate=NULL,@SliceAmount=0
			            DECLARE ventes_Cursor CURSOR FOR  
			            SELECT SaleID, SliceDate,SliceAmount   
			            FROM dbo.CustomerSlices CSL INNER JOIN dbo.Slices SL
			            ON CSL.SliceID=SL.SliceID WHERE SL.SliceDate =@dateop AND SL.PaymentMethodID=@vartillID AND CSL.isDeposit=0
			            OPEN ventes_Cursor;  
			            FETCH NEXT FROM ventes_Cursor INTO @SaleID,@SliceDate,@SliceAmount;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				            --//recuperation de la vente concerne
				            SELECT @SaleReceiptNumber=NULL,@PersonName=NULL,@SaleDate=NULL
				            SELECT @SaleReceiptNumber=SA.SaleReceiptNumber,@PersonName=C.Name,@SaleDate=SA.SaleDate FROM dbo.Sales SA INNER JOIN
				            dbo.GlobalPeople C ON SA.CustomerID=C.GlobalPersonID WHERE SA.SaleID=@SaleID
				            --//verif if special order
				            SET @COUNTSO=0
				
				            IF @COUNTSO IS NULL SET @COUNTSO=0

				            IF @COUNTSO=0 SET @Operation='SALE'
				            ELSE SET @Operation='SPECIAL LENS ORDER'

				            SELECT @ReductionAmount=NULL,@TotalPriceHT=NULL,@TotalPriceTTC=NULL,@TVAAmount=NULL
				            SELECT @ReductionAmount=ReductionAmount,@TotalPriceHT=TotalPriceHT,@TotalPriceTTC=TotalPriceTTC,@TVAAmount=TVAAmount 
				            FROM DBO.ApplyExtraPriceSale(@SALEID);
				            IF @ReductionAmount IS NULL SET @ReductionAmount=0
				            IF @TotalPriceHT IS NULL SET @TotalPriceHT=0
				            IF @TotalPriceTTC IS NULL SET @TotalPriceTTC=0
				            IF @TVAAmount IS NULL SET @TVAAmount=0

				            SET @SaleFullInformation=NULL
				            SET @SaleFullInformation=@SaleReceiptNumber+' '+CONVERT(VARCHAR,@SaleDate,103)+' '+CONVERT(VARCHAR,@TotalPriceTTC)
				
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SliceDate,@To,@At,@SliceAmount,0,@PersonName,@TotalPriceTTC,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM ventes_Cursor INTO @SaleID,@SliceDate,@SliceAmount;  
			            END;  
			            CLOSE ventes_Cursor;  
			            DEALLOCATE ventes_Cursor; 
			            --//2-tous les depots par la caisse ----depot d'epargne par caisse
			            SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			            DECLARE Deposit_Cursor CURSOR FOR  
			            SELECT AllDepositDate,Amount,AllDepositReference,AllDepositReason,Representant  
			            FROM dbo.AllDeposits dp INNER JOIN dbo.GlobalPeople c on dp.CustomerID=c.GlobalPersonID  WHERE dp.AllDepositDate =@dateop AND dp.PaymentMethodID=@vartillID
			            OPEN Deposit_Cursor;  
			            FETCH NEXT FROM Deposit_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				
				            SET @Operation='DEPOSIT'
	
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SaleDate,@To,@At,@SliceAmount,0,@PersonName,@SliceAmount,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM Deposit_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			            END;  
			            CLOSE Deposit_Cursor;  
			            DEALLOCATE Deposit_Cursor; 
			            --3--//SURPLUS DE CAISSE PAR AJUSTEMENT
			            SELECT @SaleDate=NULL,@SliceAmount=0,@SaleFullInformation=NULL,	@PersonName=NULL
			            DECLARE Till_Cursor CURSOR FOR  
			            SELECT TillAdjustDate,PhysicalPrice - ComputerPrice,Justification,Justification  
			            FROM dbo.TillAdjusts ta WHERE ta.TillAdjustDate =@dateop AND ta.TillID=@vartillID
			            OPEN Till_Cursor; 
			            FETCH NEXT FROM Till_Cursor INTO @SaleDate,@SliceAmount,@SaleFullInformation,@PersonName;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				
				            IF @SliceAmount > 0 
				            BEGIN
					            SET @Operation='OVERAGE'
					            SET @SaleReceiptNumber='OVE'+CONVERT(VARCHAR,@vartillID)
					            SET @InputAmount=@SliceAmount
					            SET @OutPutAmount=0
					            SET @SoldE=-1*@SliceAmount
				            END
				            ELSE
				            BEGIN
					            SET @Operation='TELLER ADJUST'
					            SET @SaleReceiptNumber='TADJ'+CONVERT(VARCHAR,@vartillID)
					            SET @InputAmount=0
					            SET @OutPutAmount=-1*@SliceAmount
					            SET @SoldE=-1*@SliceAmount
				            END
	
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SaleDate,@To,@At,@InputAmount,@OutPutAmount,@PersonName,@Solde,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM Till_Cursor INTO @SaleDate,@SliceAmount,@SaleFullInformation,@PersonName;  
			            END;  
			            CLOSE Till_Cursor;  
			            DEALLOCATE Till_Cursor; 
			            --//OUTPUTS OPERATIONS HISTORIQUE
			            --4--//recupartion des consommation du budget
			            SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			            DECLARE Budget_Cursor CURSOR FOR  
			            SELECT PaymentDate,VoucherAmount,Reference,BudgetAllocatedID,BeneficiaryName+' / '+Justification  
			            FROM dbo.BudgetConsumptions bc WHERE bc.PaymentDate =@dateop AND bc.PaymentMethodID=@vartillID 
			            OPEN Budget_Cursor;  
			            FETCH NEXT FROM Budget_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@BudgetAllocatedID,@PersonName;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				
				            SET @Operation='BUDGET CONSUMPTION'
				            select @SaleFullInformation=bl.BudgetLineLabel from dbo.BudgetLines bl inner join dbo.BudgetAllocateds ba
				            on ba.BudgetLineID=bl.BudgetLineID where ba.BudgetAllocatedID=@BudgetAllocatedID
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM Budget_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@BudgetAllocatedID,@PersonName;  
			            END;  
			            CLOSE Budget_Cursor;  
			            DEALLOCATE Budget_Cursor; 
			            --5--//recuperation des sorties vers banque
			            SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL
			            DECLARE Tranfbank_Cursor CURSOR FOR  
			            SELECT treope.OperationDate,treope.OperationAmount,treope.OperationRef,treope.Justification,treope.Justification  
			            FROM dbo.TreasuryOperations treope WHERE treope.OperationDate =@dateop AND treope.TillID=@vartillID and treope.OperationType='TransfertToBank'
			            OPEN Tranfbank_Cursor;  
			            FETCH NEXT FROM Tranfbank_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				
				            SET @Operation='BANK TRANSFER'
	
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM Tranfbank_Cursor INTO @SaleDate,@SliceAmount,@SaleReceiptNumber,@SaleFullInformation,@PersonName;  
			            END;  
			            CLOSE Tranfbank_Cursor;  
			            DEALLOCATE Tranfbank_Cursor;
			            --6--//recuperation des retours sur ventes
			            SELECT @SaleDate=NULL,@SliceAmount=0,@SaleReceiptNumber=null,@SaleFullInformation=NULL,	@PersonName=NULL,@CustomerReturnID=null
			            DECLARE returnSlice_Cursor CURSOR FOR  
			            SELECT sl.SliceDate,sl.SliceAmount,rs.CustomerReturnID  
			            FROM dbo.CustomerReturnSlices rs INNER JOIN dbo.Slices sl on rs.SliceID=sl.SliceID 
			            WHERE sl.SliceDate =@dateop AND sl.PaymentMethodID=@vartillID 
			            OPEN returnSlice_Cursor;  
			            FETCH NEXT FROM returnSlice_Cursor INTO @SaleDate,@SliceAmount,@CustomerReturnID;  
			            WHILE @@FETCH_STATUS = 0  
			            BEGIN 
				
				            --//recuperation de la vente concerne
				            select @SaleID=0
				            select @SaleID=SaleID from dbo.CustomerReturnSlices crs inner join dbo.CustomerReturns cr on crs.CustomerReturnID=cr.CustomerReturnID
				            where crs.CustomerReturnID=@CustomerReturnID

				            SELECT @SaleFullInformation=NULL,@PersonName=NULL,@SaleReceiptNumber=null
				            SELECT @SaleFullInformation=C.Name,@PersonName=C.Name,@SaleReceiptNumber=sa.SaleReceiptNumber FROM dbo.Sales SA INNER JOIN
				            dbo.GlobalPeople C ON SA.CustomerID=C.GlobalPersonID  WHERE SA.SaleID=@SaleID

				            SET @Operation='SALE RETURN'
	
				            --insertion resultat final
				            Insert Into @TableGenerateReportOperationPeriodeOfDayTill(OperationDate,BeginDate,
				            EndDate,InputAmount,OutPutAmount,Intervenant,Solde,Operation,TransactionNumber,Description,
				            OpeningCashAmount,ClosingCashAmount,PaymentMethod,CashRegisterName)
				            Values (@SaleDate,@To,@At,0,@SliceAmount,@PersonName,@SliceAmount,@Operation,
				            @SaleReceiptNumber,@SaleFullInformation,@OpeningCashAmount,@ClosingCashAmount,@PaymentMethod,@CashRegisterName)

				            FETCH NEXT FROM returnSlice_Cursor INTO @SaleDate,@SliceAmount,@CustomerReturnID;  
			            END;  
			            CLOSE returnSlice_Cursor;  
			            DEALLOCATE returnSlice_Cursor;
		
		

		            ----------------------------------------------------------------------------
		            SET @dateop = DATEADD (day , 1 , @dateop )
		
		            END
		            FETCH NEXT FROM Tills_Cursor INTO @vartillID;  
	            END
	            CLOSE Tills_Cursor;  
	            DEALLOCATE Tills_Cursor; 
	
	            RETURN;
            end;");
        }
    }
}
