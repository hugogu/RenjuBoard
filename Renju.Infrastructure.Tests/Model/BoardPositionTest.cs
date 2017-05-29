namespace Renju.Infrastructure.Tests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Infrastructure.Model;
    using Infrastructure.Model.Extensions;

    [TestClass]
    public class BoardPositionTest
    {
        [TestMethod]
        public void AddPositionTest()
        {
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OnSameHorizontalLineTest()
        {
            var positionA = new BoardPosition(3, 0);
            var positionB = new BoardPosition(4, 0);

            Assert.IsTrue(positionA.IsOnLineWith(positionB));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OnSameVerticalLineTest()
        {
            var positionA = new BoardPosition(0, 1);
            var positionB = new BoardPosition(0, 2);

            Assert.IsTrue(positionA.IsOnLineWith(positionB));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void OnSameInclinedLineTest()
        {
            var positionA = new BoardPosition(0, 0);
            var positionB = new BoardPosition(10, 10);

            Assert.IsTrue(positionA.IsOnLineWith(positionB));
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void NotOnSameLineTest()
        {
            var positionA = new BoardPosition(0, 1);
            var positionB = new BoardPosition(2, 2);

            Assert.IsFalse(positionA.IsOnLineWith(positionB));
        }
    }
}
