using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip
{
    public class Ship
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Lenght { get; set; }
        public bool Sunk { get; set; }

        public Ship(string id, string name, int lenght)
        {
            ID = id;
            Name = name;
            Lenght = lenght;
        }
    }
}
