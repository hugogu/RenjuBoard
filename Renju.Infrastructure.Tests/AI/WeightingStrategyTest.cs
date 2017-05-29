namespace Renju.Infrastructure.Tests.AI
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Infrastructure.AI.Weight;
    using System;

    [TestClass]
    public class WeightingStrategyTest
    {
        [TestMethod]
        public void TestWeightPatternGeneration()
        {
            var script = @"maxlength=11
++++.,+++.+,++.++:64
----.,---.-,--.--
+++._,+++_._
+++_.
+_---._
-_.---_
_.---_
++.__,++_._
+.___
+_.__
--._,--_.
-._
++__.
+__._
-_._";
            var stream = new MemoryStream(Encoding.Default.GetBytes(script));
            var strategy = new WeightingStrategy(stream);

            Assert.AreEqual(11, strategy.PatternLength);
            Assert.AreEqual(15, strategy.Rules.Count);
        }

        [TestMethod]
        public void WeightingRuleGenerator()
        {
            var patterns = WeightingRule.GeneratePatterns(".", 11).ToList();

            Assert.AreEqual(Convert.ToInt32(Math.Pow(3, 11)) / 2, patterns.Count);
        }
    }
}
