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
        int numberOfErrors = 0;

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
            StartListen(port);
            while (true)
            {
                Console.WriteLine("Väntar på att någon ska ansluta sig...");

                using (var client = listener.AcceptTcpClient())
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    // Skriver ut Protokollet när någon är ansluten
                    Console.WriteLine($"Klienten har anslutit : {client.Client.RemoteEndPoint}");
                    writer.WriteLine(StatusCode.GetStatusCode(210));

                    while (client.Connected)
                    {
                        var receivedCommand = reader.ReadLine().ToUpper();
                        bool isStatusCode = int.TryParse(receivedCommand.Split(' ')[0], out int receivedStatusCode);
                        Console.WriteLine($"Mottaget: {receivedCommand}");

                        if (receivedCommand.Equals("QUIT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine("BYE BYE");
                            break;
                        }

                        // Det kommer ett Error
                        else if (receivedStatusCode == 500 || receivedStatusCode == 501)
                        {
                            // Kan hantera kod, inget som behövs

                            Console.Write("Din tur igen -->");
                            writer.WriteLine(Console.ReadLine());
                        }

                        // Visa Hjälp
                        else if (receivedCommand.Equals("HELP"))
                        {
                            var helpMessage = ResponseManager.GetHelpMessages();
                            writer.WriteLine(ResponseManager.GetHelpMessages());

                        }

                        // Kollar om det är ett Hello eller Helo meddelande
                        else if (receivedCommand.Split(' ')[0].Equals("HELO", StringComparison.InvariantCultureIgnoreCase) || receivedCommand.Split(' ')[0].Equals("HELLO", StringComparison.InvariantCultureIgnoreCase))
                        {
                            GreetingIsValid = true;
                            writer.WriteLine("220 " + playerName);
                        }                       

                        // Klienten har sagt HELLO
                        else if (GreetingIsValid)
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
                            else if (receivedCommand.Split(' ')[0].Equals("FIRE", StringComparison.InvariantCultureIgnoreCase))
                            {

                                var response = oceanGrid.TrimShot(receivedCommand);
                                bool isError = CheckForError(response);
                                writer.WriteLine(response);

                                oceanGrid.DrawBoard();

                                if (!isError)
                                {
                                    var sendCommand = Console.ReadLine();
                                    writer.WriteLine(sendCommand);
                                }
                                
                            }
                            else
                            {

                                if (!isStatusCode)
                                {
                                    writer.WriteLine(StatusCode.GetStatusCode(500));
                                    numberOfErrors++;
                                }
                            }

                        }

                        else
                        {

                            // Om kommandot inte är en statuskod men inte fastnar i nån if-sats skicka ett 500 error
                            if (!isStatusCode)
                            {
                                writer.WriteLine(StatusCode.GetStatusCode(500));
                            }
                        }

                        if (numberOfErrors > 3)
                        {
                            Console.Clear();
                            writer.WriteLine(StatusCode.GetStatusCode(270));
                            break;
                        }

                    }
                }

            }

        }

        private bool CheckForError(string command)
        {
            if (command.Split(' ')[0] == "500" || command.Split(' ')[0] == "501")
            {
                numberOfErrors++;
                return true;
            }
            else
            {
                numberOfErrors = 0;
                return false;
            }
        }
    }
}
