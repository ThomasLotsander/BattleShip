using BattleShip.Managers;
using BattleShip.Model;
using BattleshipTcpClient;
using BattleshipTcpServer;
using System;
using System.Collections.Generic;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            
            while (true)
            {
                ShipManager shipManager = new ShipManager();
                List<Ship> ships = shipManager.CreateShip();

                GameManager oceanGrid = new GameManager(ships);
                GameManager targetGrid = new GameManager();


                Console.Write("Spelarens namn: ");
                string name = Console.ReadLine();

                Console.WriteLine("Ange host:");
                var host = Console.ReadLine();
                Console.WriteLine("Ange port:");
                var port = int.Parse(Console.ReadLine());

                if (String.IsNullOrWhiteSpace(host))
                {
                    try
                    {
                        BsServer server = new BsServer();
                        server.Listen(port, name, targetGrid, oceanGrid);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    try
                    {
                        BsClient client = new BsClient();
                        client.Host(host, port, name, targetGrid, oceanGrid);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Fastnat i error");
                        Console.WriteLine(e.Message);
                    }
                }


            }

        }
    }
}
