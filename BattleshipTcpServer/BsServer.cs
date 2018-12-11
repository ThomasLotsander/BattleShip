using BattleShip;
using BattleShip.Managers;
using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleshipTcpServer
{
    public class BsServer
    {
        TcpListener listener;
        bool GreetingIsValid;

        private void StartListen(int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Starts listening on port: {port}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Misslyckades att öppna socket. Troligtvis upptagen.");
                Environment.Exit(1);
            }
        }

        public void Listen(int port, string playerName, GameManager targetGrid, GameManager oceanGrid)
        {
            //Console.WriteLine("Välkommen till servern");
            //Console.WriteLine("Ange port att lyssna på:");
            //var port = int.Parse(Console.ReadLine());

            StartListen(port);

            while (true)
            {
                Console.WriteLine("Väntar på att någon ska ansluta sig...");

                using (var client = listener.AcceptTcpClient())
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    Console.WriteLine($"Klienten har anslutit : {client.Client.RemoteEndPoint}");
                    writer.WriteLine(StatusCode.GetStatusCode(210));

                    while (client.Connected)
                    {

                        var receivedCommand = reader.ReadLine().ToUpper();
                        Console.WriteLine($"Mottaget: {receivedCommand}");

                        if (receivedCommand.Equals("QUIT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine("BYE BYE");
                            break;
                        }

                        if (receivedCommand.Contains("HELO"))
                        {
                            GreetingIsValid = true;
                            writer.WriteLine("220 " + playerName);
                        }

                        if (GreetingIsValid)
                        {
                            if (receivedCommand.Equals("START"))
                            {
                                Random rnd = new Random();
                                int random = rnd.Next(221, 223);

                                writer.WriteLine(StatusCode.GetStatusCode(random));
                                Console.WriteLine(StatusCode.GetStatusCode(random));

                                if (random == 222)
                                {
                                    var sendCommand = Console.ReadLine();
                                    writer.WriteLine(sendCommand);
                                }

                            }

                            if (receivedCommand.Contains("FIRE", StringComparison.InvariantCultureIgnoreCase))
                            {

                                var shot = oceanGrid.TrimShot(receivedCommand);
                                writer.WriteLine(oceanGrid.Fire(shot[0], shot[1]));

                                oceanGrid.DrawBoard();

                                var sendCommand = Console.ReadLine();
                                writer.WriteLine(sendCommand);
                            }

                        }
                        else
                        {
                            writer.WriteLine($"UNKNOWNED COMMAND: {receivedCommand}");
                        }

                    }
                }

            }

        }
    }
}
