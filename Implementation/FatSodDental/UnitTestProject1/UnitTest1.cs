using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FatSod.DataContext.Repositories;
using FatSod.Supply.Entities;
using Moq;
using FatSod.Supply.Abstracts;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {

        ////test if account exist
        //[TestMethod]
        //public void TestIfAccountExist()
        //{
        //    //Arrange
        //    AccountRepository acctRep = new AccountRepository();
        //    //Act
        //    bool isAcctExit = acctRep.CheckIfAcctExit(10141000);

        //    //Assert
        //    Assert.IsTrue(isAcctExit, "Failed:Account does not exit");
            
        //}

        ////tester l'affichage d'un nouveau compte
        //[TestMethod]
        //public void TestAfficheNewAccount()
        //{
        //    //Arrange
        //    AccountRepository acctRep = new AccountRepository();
        //    //Act
        //    int newAcctExit = acctRep.AfficheAccountNumber(1);

        //    //Assert
        //    Assert.AreEqual (311110003, newAcctExit);
        //    //Assert.IsNull(newAcctExit,"erreur execution affiche compte");

        //}

        ////tester la creation d'un nouveau compte
        //[TestMethod]
        //public void TestCreateNewAccount()
        //{
        //    //Arrange
        //    AccountRepository acctRep = new AccountRepository();
        //    //Act
        //    Account newAcct = acctRep.GenerateAccountNumber(1,"test hpp",true);

        //    //Assert
        //    //Assert.AreEqual(311110003, newAcctExit);
        //    //Assert.IsNotNull(newAcct, "erreur execution affiche compte");

        //    Assert.IsInstanceOfType(newAcct, typeof(Account));

        //}
        ////tester la creation d'un nvo cpte a partir du section account
        //[TestMethod]
        //public void TestCreateNewAcctWithSectionAcct()
        //{
        //    // arrange
        //    Mock<IAccount> mock = new Mock<IAccount>();
        //    mock.Setup(m => m.GenerateAccountNumber(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns<Account>(total => total);

        //    var target = new AccountRepository(mock.Object);

        //    // act
        //    var result = target.GenerateAccountNumber(1,"toto",true);
        //    // assert
        //    Assert.IsInstanceOfType(result, typeof(Account));

        //}
        [TestMethod]
        public void doitRetourner1SiNombreEst1 ()
        {
            Assert.AreEqual( "1", new FizzBuzz().generernombre(1,1));
        }
        [TestMethod]
        public void doitRetourner2SiNombreEst2()
        {
            Assert.AreEqual("2", new FizzBuzz().generernombre(2,2));
        }
        [TestMethod]
        public void doitRetournerFizzSiNombreEstx3()
        {
            Assert.AreEqual("Fizz", new FizzBuzz().generernombre(3,3));
        }
        [TestMethod]
        public void doitRetournerBuzzSiNombreEstx5()
        {
            Assert.AreEqual("Buzz", new FizzBuzz().generernombre(5,5));
        }
        [TestMethod]
        public void doitRetournerFizzBuzzSiNombreEstx15()
        {
            Assert.AreEqual("FizzBuzz", new FizzBuzz().generernombre(30,30));
        }
        [TestMethod]
        public void doitRetournerFizzBuzzSiNombreEstxfinal()
        {
            Assert.AreEqual("12Fizz4BuzzFizz78FizzBuzz11Fizz1314FizzBuzz16", new FizzBuzz().generernombre(1, 16));
        }
        [TestMethod]
        public void AfficheretournePrimaryDiagonal()
        {
            
            int[,] tab =
            {
                {3,4,5 },
                {4,5,6 },
                {3,4,5 }
            };
            int[] returntab = { 3, 5, 5 };
           Assert.AreEqual(returntab, new FizzBuzz().retournePrimaryDiagonal(tab));
            //Assert.IsInstanceOfType(returntab, typeof(int[]));
        }
        [TestMethod]
        public void AfficheretourneSecondaryDiagonal()
        {

            int[,] tab =
            {
                {3,4,5 },
                {4,5,6 },
                {3,4,5 }
            };
            int[] returntab = { 5, 5, 3 };
            Assert.AreEqual(returntab, new FizzBuzz().retourneSecondaryDiagonal(tab));
            //Assert.IsInstanceOfType(returntab, typeof(int[]));
        }

        [TestMethod]
        public void AffichediagonalDifference()
        {

            int[,] tab =
            {
                {11,2,4 },
                {4,5,6 },
                {10,8,-12 }
            };
            int returntab = 15;
            Assert.AreEqual(returntab, new FizzBuzz().diagonalDifference(tab));
            //Assert.IsInstanceOfType(returntab, typeof(int[]));
        }

        [TestMethod]
        public void AffichetestReturnPositif()
        {

            int[] tab =
                {-4, 3, -9, 0, 4, 1 };
            double returntab = 0.500000;
            Assert.AreEqual(returntab, new FizzBuzz().returnpositivenumber(tab));
        }
    }
}
