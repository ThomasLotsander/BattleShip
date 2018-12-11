using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip
{
   public static class StatusCode
    {
        public static string GetStatusCode(int code)
        {
            string response = "";
            switch (code)
            {
                case 210:
                    response = "210 BATTLESHIP / 1.0";
                    break;

                case 221:
                    response = "221 Client Starts";
                        break;
                case 222:
                    response = "222 Host Starts";
                        break;
                case 230:
                    response = "230 MISS";
                        break;
                case 241:
                    response = "241 YOU HIT MY CARRIER";
                        break;
                case 242:
                    response = "242 You hit my Battleship";
                        break;
                case 243:
                    response = "243 You hit my Destroyer";
                        break;
                case 244:
                    response = "244 You hit my Submarine";
                    break;
                case 245:
                    response = "245 You hit my Patrol Boat";
                    break;
                case 251:
                    response = "251 You sunk my Carrier";
                    break;
                case 252:
                    response = "252 You sunk my Battleship";
                    break;
                case 253:
                    response = "253 You sunk my Destroyer";
                    break;
                case 254:
                    response = "254 You sunk my Submarine";
                    break;
                case 255:
                    response = "255 You sunk my Patrol Boat";
                    break;
                case 260:
                    response = "260 You win!";
                    break;
                case 270:
                    response = "270 Connection closed";
                    break;
                case 500:
                    response = "500 Syntax error";
                    break;
                case 501:
                    response = "501 Sequence error";
                    break;
                
            }


            return response;               
                
        }
    }
}
