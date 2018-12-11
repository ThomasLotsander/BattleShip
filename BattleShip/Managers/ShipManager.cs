using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip.Managers
{
    public class ShipManager
    {
        public List<Ship> CreateShip()
        {
            List<Ship> ships = new List<Ship>();

            ships.Add(new Ship("241","CARRIER", 5));
            ships.Add(new Ship("242", "BATTLESHIP", 4));
            ships.Add(new Ship("243", "DESTROYER", 3));
            ships.Add(new Ship("244", "SUBMARINE", 3));
            ships.Add(new Ship("245", "PATROL BOAT", 2));

            return ships;

        }

    }
}
