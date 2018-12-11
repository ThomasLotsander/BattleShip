using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Managers
{
    public class GameManager
    {
        string[,] grid = new string[10, 10];
        string[] Letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        string[] ShipsIDs = new string[] { "1", "2", "3", "4", "5" };

        List<Ship> ships;

        int cursorLeft;
        int cursorTop;

        public GameManager() { }
        public GameManager(List<Ship> ships)
        {
            this.ships = ships;

            SetupGame();

            PlaceShip(ships[0], 2, 2, "V"); // Length 5
            PlaceShip(ships[1], 1, 3, "H"); // Length 4
            PlaceShip(ships[2], 6, 5, "V"); // Length 3
            PlaceShip(ships[3], 8, 7, "H"); // Length 3
            PlaceShip(ships[4], 1, 9, "V"); // Length 2

            DrawBoard();
        }

        public void SetupGame()
        {
            // Fyller brädet med tomma platser
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    grid[i, j] = " ";
                }
            }
        }

        

        

        public void DrawBoard()
        {
            cursorLeft = Console.CursorLeft;
            cursorTop = Console.CursorTop;
            Console.SetCursorPosition(0, 0);

            // Skriver ut bokstäver i toppen
            Console.Write("\t ");
            for (int i = 0; i < Letters.Length; i++)
            {
                Console.Write("  " + Letters[i] + " ");
            }
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("\t  ----------------------------------------");
                Console.Write(i + 1 + ")\t");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(" | ");
                    // Det är en båt på platsen
                    if (grid[j, i] == "241" || grid[j, i] == "242" || grid[j, i] == "243" || grid[j, i] == "244" || grid[j, i] == "245")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("#");
                        Console.ResetColor();
                    }

                    // Båten är träffad
                    else if (grid[j, i] == "H")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X");
                        Console.ResetColor();
                    }
                    else if (grid[j, i] == "X")
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(grid[j, i]);
                        Console.ResetColor();
                    }
                    else
                        Console.Write(grid[j, i]);

                }
                Console.Write(" | ");
                Console.WriteLine();


            }
            Console.WriteLine("\t  ----------------------------------------");

            if (cursorTop == 0)
                cursorTop = Console.CursorTop;

            Console.SetCursorPosition(cursorLeft, cursorTop);

        }

        public void MarkTargetGrid(int vertivcalPos, int horizontalPos)
        {
            // Markerar mina skott på egna brädet
            // Ändrar positionen till 0 indexering för att anpassa Arrayerna
            vertivcalPos = vertivcalPos - 1;
            horizontalPos = horizontalPos - 1;

            // Det är en tom ruta men ingen träff
            if (grid[horizontalPos, vertivcalPos] == " ")
            {
                grid[horizontalPos, vertivcalPos] = "X";
            }
            else
            {
                grid[horizontalPos, vertivcalPos] = "H";
            }
        }

        public string Fire(int vertivcalPos, int horizontalPos)
        {
            vertivcalPos = vertivcalPos - 1;
            horizontalPos = horizontalPos - 1;
            try
            {
                // Här har spelaren redan skjutit
                // Spelaren får ett nytt försök
                if (grid[horizontalPos, vertivcalPos] == "X")
                {
                    return StatusCode.GetStatusCode(501);
                }
                // Det är en tom ruta men ingen träff
                else if (grid[horizontalPos, vertivcalPos] == " ")
                {
                    grid[horizontalPos, vertivcalPos] = "X";
                    return StatusCode.GetStatusCode(230);
                }
                // Träff
                else
                {
                    string id = grid[horizontalPos, vertivcalPos];
                    Ship ship = ships.FirstOrDefault(c => c.ID == id);
                    grid[horizontalPos, vertivcalPos] = "H";

                    ship.Lenght -= 1;
                    if (ship.Lenght <= 0)
                    {
                        ship.Sunk = true;
                        return StatusCode.GetStatusCode(int.Parse(id) + 10);
                    }
                    return StatusCode.GetStatusCode(int.Parse(id));


                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public int[] TrimShot(string str)
        {
            str = str.ToUpper();
            char verticalPos = str[5];
            char horizontalPos = str[6];

            var ascii = Convert.ToByte(verticalPos) - 64;

            int[] pos = new int[] { horizontalPos - 48, ascii };

            return pos;
        }

        public bool PlaceShip(Ship ship, int VerticalStartPos, int HorizontalStartPos, string direction)
        {
            if (direction.ToUpper() != "V" && direction.ToUpper() != "H")
            {
                return false;
            }

            // Placera vertikalt 
            if (direction.ToUpper() == "V")
            {
                for (int i = VerticalStartPos; i < VerticalStartPos + ship.Lenght; i++)
                {
                    grid[i - 1, HorizontalStartPos - 1] = ship.ID;
                }
            }

            // Placera horisontellt
            else
            {
                for (int i = HorizontalStartPos; i < HorizontalStartPos + ship.Lenght; i++)
                {
                    grid[VerticalStartPos - 1, i - 1] = ship.ID;
                }
            }

            return true;

        }



    }
}
