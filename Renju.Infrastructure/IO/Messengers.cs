namespace Renju.Infrastructure.IO
{
    using System.Diagnostics;

    public static class Messengers
    {
        public static IMessenger<REQ, RES> FromProcess<REQ, RES>(Process process)
        {
            return new StreamMessenger<REQ, RES>(process.StandardOutput, process.StandardInput);
        }
    }
}
