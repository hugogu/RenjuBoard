namespace Renju.Infrastructure.Tests
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                adaptor.Says += (sender, e) => Trace.WriteLine(e.Message);
                adaptor.Dropping += (sender, e) =>
                {
                    Trace.WriteLine(e.Message.AsString());
                    evt.Set();
                };
                Task.Run(async () =>
                {
                    await adaptor.InitializeAsync(15).ConfigureAwait(false);
                    await adaptor.OpponentDropsAsync(new PieceDrop(7, 7, Side.Black)).ConfigureAwait(false);
                    evt.WaitOne(15000);
                    await adaptor.EndAsync().ConfigureAwait(false);
                }).Wait();
            }
        }
    }
}
