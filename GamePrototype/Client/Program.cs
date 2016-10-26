using Client.Client_Side;

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
