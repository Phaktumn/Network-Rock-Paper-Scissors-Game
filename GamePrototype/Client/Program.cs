using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientController clientController = new ClientController();
            clientController.Connect("127.0.0.1", 5000);
            clientController.StartClient();
        }
    }
}
