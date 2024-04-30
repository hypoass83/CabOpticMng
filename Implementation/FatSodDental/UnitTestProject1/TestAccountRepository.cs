using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Moq;
using FatSod.Supply.Abstracts;
using FatSod.Supply.Entities;
using FatSod.DataContext.Repositories;

namespace UnitTestProject1
{
    //[TestFixture]
    public class TestAccountRepository
    {
        [Test]
        public void TestHelloNunit()
        {
            Assert.That(true, Is.False);
        }

        [TestCase("PROD", "Stock of goods A", typeof(CollectifAccount))]
        public void TestCreateNewAcctWithSectionAcct(string AccSectionCode, string collectivAcclable, CollectifAccount finalcoll)
        {
            // arrange
            Mock<ICollectifAccount> mock = new Mock<ICollectifAccount>(MockBehavior.Strict);
            mock.Setup(m => m.GetCollectifAccount(AccSectionCode, collectivAcclable)).Returns(finalcoll);

            var target = new AccountRepository(mock.Object);

            // act
            var result = target.GenerateAccountNumber(finalcoll.CollectifAccountID, collectivAcclable + " Account", true);
            // assert
            //Assert.IsInstanceOfType(result, typeof(Account));
            Assert.That(result.AccountNumber, Is.EqualTo(311110003));

        }
    }
}
