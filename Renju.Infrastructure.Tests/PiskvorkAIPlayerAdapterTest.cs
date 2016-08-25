namespace Renju.Infrastructure.Tests
{
    using System.Diagnostics;
    using System.Threading;
    using AI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;
    using Protocols.Piskvork;

    [TestClass]
    public class PiskvorkAIPlayerAdapterTest
    {
        [TestMethod]
        public void StartingGameTest()
        {
            using (var adaptor = new PiskvorkAIPlayerAdapter(@"..\..\..\bin\Debug\piskvork\pbrain-yixin16_64.exe"))
            {
                var evt = new AutoResetEvent(false);
                adaptor.Says += (sender, e) =>
                {
                    Trace.WriteLine(e.Message);
                };
                adaptor.Dropping += (sender, e) =>
                {
                    Trace.WriteLine(e.Message.AsString());
                    evt.Set();
                };
                adaptor.Initialize(15);
                adaptor.OpponentDrops(new PieceDrop(7, 7, Side.Black));
                evt.WaitOne(15000);
                adaptor.End();
            }
        }
    }
}
