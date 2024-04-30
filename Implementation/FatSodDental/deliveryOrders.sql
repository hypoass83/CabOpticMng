delete from customerreturnlines
delete from salelines
delete from SaleReturnAccountOperations
delete from SaleAccountOperations


delete from customerslices
delete from SpecialOrderSlices
delete from TillSales
delete from BankSales
delete from SavingAccountSales
delete from sales
delete from CustomerReturns
delete from CustomerReturnSlices

update SpecialOrders 
set OrderStatut = 3;
select * from SpecialOrders