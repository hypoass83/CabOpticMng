using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using FatSod.Supply.Entities;
using FatSod.Supply.Abstracts;
using FatSod.DataContext.Repositories;
using Moq;

namespace FatSodDental.Tests
{
    [TestFixture]
    //[TestClass]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            Assert.That(true, Is.True);
        }

        //[Test]
        [TestCase("PROD", "SV CR39 WHITE Collectif Account", 2)]
        public void TestCreateNewAcctWithSectionAcct(string AccSectionCode, string collectivAcclable, int finalcoll)
        {
            // arrange
            Mock<ICollectifAccount> mock = new Mock<ICollectifAccount>(MockBehavior.Strict);
            mock.Setup(m => m.getCompteCollectifID(AccSectionCode, collectivAcclable)).Returns(finalcoll);

            var target = new AccountRepository(mock.Object);

            // act
            var result = target.GenerateAccountNumber(finalcoll, collectivAcclable + " Account", true);
            // assert
            //Assert.IsInstanceOfType(result, typeof(Account));
            Assert.That(result.AccountNumber, Is.EqualTo(311110003));

        }

        [TestCase("PROD", "SV CR39 WHITE Collectif Account")]
        public void TestgetCompteCollectifID(string AccSectionCode, string collectivAcclable)
        {
            // arrange
            //Mock<ICollectifAccount> mock = new Mock<ICollectifAccount>(MockBehavior.Strict);
            //mock.Setup(m => m.getCompteCollectifID(AccSectionCode, collectivAcclable)).Returns(finalcoll);

            var target = new CollectifAccountRepository();

            // act
            var result = target.getCompteCollectifID(AccSectionCode, collectivAcclable);
            // assert
            //Assert.IsInstanceOfType(result, typeof(Account));
            Assert.That(result, Is.EqualTo(1));

        }
    }
}
