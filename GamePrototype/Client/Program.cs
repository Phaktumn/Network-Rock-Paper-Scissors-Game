using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Client.Client_Side;
using Common.Network;

namespace Client
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            ClientGameController clientGameController = new ClientGameController();
            clientGameController.Start();
        }
    }
}
