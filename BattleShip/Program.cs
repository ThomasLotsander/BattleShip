using BattleShip.Managers;
using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            ShipManager shipManager = new ShipManager();
            List<Ship> ships = shipManager.CreateShip();

            GameManager oceanGrid = new GameManager(ships);
            //GameManager targetGrid = new GameManager();
            //targetGrid.SetupGame();


            Console.Write("Spelarens namn: ");
            string name = Console.ReadLine();

            while (true)
            {

                Console.Write("Text : ");
                var fire = oceanGrid.TrimShot(Console.ReadLine());

                Console.WriteLine(fire);
                oceanGrid.DrawBoard();
                //targetGrid.DrawBoard();
            }
        }
    }
}
