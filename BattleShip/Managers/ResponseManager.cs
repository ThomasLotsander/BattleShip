using BattleShip.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip.Managers
{
    public static class ResponseManager
    {
        
        public static string GetHelpMessages()
        {
            string HelpMessages = "HELP: Alla svar kommer med en Svarskod: " +
                "230 = miss. " +
                "241 - 245 = träff på båt. " +
                "251 - 255 = Sänkt skäpp. " +
                "260 = Du vann. " +
                "270 = Connection Lost. " +
                "5xx = Error. " +
                "Kommandon som går att skriva: " +
                "HELO<SP><PLAYERNAME><CRLF> - " +
                "START <CRLF> - " +
                "FIRE <SP> <coordinate> [ <SP> <message> ] <CRLF> - " +
                "HELP <CRLF> - " +
                "QUIT <CRLF>";
            return HelpMessages;

        }


    }
}
