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

        List<Ship> ships;

        int cursorLeft;
        int cursorTop;

        public GameManager() { }
        public GameManager(List<Ship> ships)
        {
            this.ships = ships;

            SetupGame();

            PlaceShip(ships[0], 1, 1, "V"); // Length 5
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

                    if (int.TryParse(grid[j, i], out int shipId))
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

        private string Fire(int vertivcalPos, int horizontalPos)
        {
            vertivcalPos = vertivcalPos - 1;
            horizontalPos = horizontalPos - 1;
            try
            {
                // Här har spelaren redan skjutit
                // Spelaren får ett nytt försök
                if (grid[horizontalPos, vertivcalPos] == "X")
                {
                    return StatusCode.GetStatusCode(500);
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
                        // Kollar något skäpp fortfarande flyter
                        if (ships.Any(c => c.Sunk == false))
                        {
                            return StatusCode.GetStatusCode(int.Parse(id) + 10);
                        }
                        else
                            return StatusCode.GetStatusCode(260);
                    }
                    return StatusCode.GetStatusCode(int.Parse(id));


                }
            }
            catch (IndexOutOfRangeException ex)
            {
                return StatusCode.GetStatusCode(500);
            }
        }

        public string TrimShot(string firePos)
        {
            firePos = firePos.ToUpper();

            // Kolla om bokstaven är en bokstav mellan A - J
            int verticalPos = Convert.ToByte(firePos[5]) - 64;
            if (verticalPos > 0 && verticalPos < 11)
            {
                // Kolla om pos 6 är en siffra mellan 1 - 10
                bool posSixIsNumber = int.TryParse(firePos[6].ToString(), out int horizontalPos);

                /* 
                 * Kolla om det finns en siffra i possition 7
                 * Kolla om den siffran är = 0
                 * Om allt stämmer gör om HorizontalPos till det nummret
                 */
                if (posSixIsNumber)
                {
                    if (firePos.Length >= 8)
                    {
                        bool posEightIsNumber = int.TryParse(firePos[7].ToString(), out int zero);
                        if (posEightIsNumber)
                        {
                            if (zero == 0)
                            {
                                horizontalPos = int.Parse(firePos.Substring(6, 2));
                                return Fire(horizontalPos, verticalPos);
                            }
                            return StatusCode.GetStatusCode(500);
                        }
                    }
                    return Fire(horizontalPos, verticalPos);
                }
            }

            return StatusCode.GetStatusCode(500);

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
