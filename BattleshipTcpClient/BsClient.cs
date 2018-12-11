using BattleShip;
using BattleShip.Managers;
using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BattleshipTcpClient
{
    public class BsClient
    {
        public void Host(string host, int port, string playerName, GameManager targetGrid, GameManager oceanGrid)
        {
         
            using (var client = new TcpClient(host, port))
            using (var networkStream = client.GetStream())
            using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
            using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
            {

                while (client.Connected)
                {
                    var receivedCommand = reader.ReadLine().ToUpper();
                    Console.WriteLine($"Mottaget: {receivedCommand}");

                    if (receivedCommand.Equals(StatusCode.GetStatusCode(210)))
                    {
                        writer.WriteLine("HELO " + playerName);
                    }

                    else if (receivedCommand.Contains("220"))
                    {
                        Console.WriteLine("Skriv START för att starta spelet");
                        writer.WriteLine(Console.ReadLine());
                    }
                    else if (receivedCommand.Contains("221"))
                    {
                        Console.Write("> ");
                        var sendCommand = Console.ReadLine();
                        writer.WriteLine(sendCommand);

                    }
                    else if (receivedCommand.Contains("FIRE", StringComparison.InvariantCultureIgnoreCase))
                    {

                        var shot = oceanGrid.TrimShot(receivedCommand);
                        writer.WriteLine(oceanGrid.Fire(shot[0], shot[1]));
                        // Markera egan skott på Targetgrid

                        oceanGrid.DrawBoard();

                        Console.Write("> ");
                        var sendCommand = Console.ReadLine();
                        writer.WriteLine(sendCommand);

                        if (sendCommand == "QUIT")
                            Environment.Exit(1);

                        if (sendCommand.Contains("FIRE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var myShot = targetGrid.TrimShot(sendCommand);
                            targetGrid.MarkTargetGrid(myShot[0], myShot[1]);
                            targetGrid.DrawBoard();
                        }


                    }

                    if (!client.Connected) break;



                };

            }

        }
    }
}
