using BattleShip;
using BattleShip.Managers;
using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BattleshipTcpClient
{
    public class BsClient
    {
        int numberOfErrors = 0;
        bool serverAwnserIsOk = false;
        public void Host(string host, int port, string playerName, GameManager targetGrid, GameManager oceanGrid)
        {

            using (var client = new TcpClient(host, port))
            using (var networkStream = client.GetStream())
            using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
            using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
            {
                while (client.Connected)
                {
                    if (!client.Connected) break;

                    string receivedCommand = reader.ReadLineAsync().Result.ToUpper();
                    bool isStatusCode = int.TryParse(receivedCommand.Split(' ')[0], out int receivedStatusCode);
                    Console.WriteLine($"Mottaget: {receivedCommand}");

                    // Välkomstmeddelande
                    if (receivedStatusCode == 210)
                    {
                        serverAwnserIsOk = true;
                        writer.WriteLine("HELO " + playerName);
                    }



                    // Om servern inte har svart med ett välkomstmeddlenade ska förbindelsen brytas
                    else if (serverAwnserIsOk == false)
                    {
                        writer.WriteLine(StatusCode.GetStatusCode(501));
                        Environment.Exit(1);
                    }

                    // Om det kommer en 500 status-kod
                    else if (receivedStatusCode == 500 || receivedStatusCode == 501)
                    {
                        Console.Write("Din tur igen -->");
                        writer.WriteLine(Console.ReadLine());
                    }

                    // Klienten har vunnit
                    else if (receivedStatusCode == 260)
                    {
                        writer.WriteLine("QUIT");
                    }

                    else if (receivedStatusCode == 270)
                        break;

                    // Det är ett HELLO Meddelande 
                    else if (receivedStatusCode == 220)
                    {
                        var sendMessage = "";
                        do
                        {
                            Console.WriteLine("Skriv START för att starta spelet");
                            sendMessage = Console.ReadLine().ToUpper();
                            if (sendMessage == "QUIT")
                                Environment.Exit(1);

                        } while (sendMessage != "START");
                        writer.WriteLine(sendMessage);
                    }

                    // Clienten börjar
                    else if (receivedStatusCode == 221)
                    {
                        Console.Write("--> ");
                        var sendCommand = Console.ReadLine();
                        writer.WriteLine(sendCommand);

                    }

                    // Servern försöker skjuta
                    else if (receivedCommand.Split(' ')[0].Equals("FIRE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var response = oceanGrid.TrimShot(receivedCommand);
                        bool isError = CheckForError(response);
                        writer.WriteLine(response);

                        if (numberOfErrors > 3)
                        {
                            Console.Clear();
                            Console.WriteLine("Two many errors");
                            break;
                        }

                        oceanGrid.DrawBoard();
                        if (!isError)
                        {
                            Console.Write("> ");
                            var sendCommand = Console.ReadLine();
                            writer.WriteLine(sendCommand);

                            if (sendCommand == "QUIT")
                                break;

                            // Markera på egen tavla
                            if (sendCommand.Contains("FIRE", StringComparison.InvariantCultureIgnoreCase))
                            {
                                //var myShot = targetGrid.TrimShot(sendCommand);
                                //targetGrid.MarkTargetGrid(myShot[0], myShot[1]);
                                //targetGrid.DrawBoard();
                            }
                        }

                    }
                    else if (receivedCommand.Contains("HELP", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.Write("Din tur igen -->");
                        writer.WriteLine(Console.ReadLine());
                    }


                    // Om det skickas en tom sträng
                    else if (string.IsNullOrWhiteSpace(receivedCommand))
                    {
                        writer.WriteLine(StatusCode.GetStatusCode(500));
                        numberOfErrors++;
                    }

                    else
                    {
                        if (!isStatusCode)
                        {
                            writer.WriteLine(StatusCode.GetStatusCode(500));
                            numberOfErrors++;
                        }

                    }

                    if (numberOfErrors > 3)
                    {
                        Console.Clear();
                        break;
                    }
                };

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
