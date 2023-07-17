using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using System;

namespace barash.Doors.Commands
{
    class TakeTaxes : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "taketaxes" };

        public List<string> Permissions => new List<string> { "take.taxes" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            
        }
    }
}
