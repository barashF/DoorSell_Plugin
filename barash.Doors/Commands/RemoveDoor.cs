using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using System;

namespace barash.Doors.Commands
{
    class RemoveDoor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "RemoveDoor";

        public string Help => "/removedoor";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "removedoor" };

        public List<string> Permissions => new List<string> { "new.door" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if (raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    InteractableDoorHinge doorHinge = raycast.GetComponent<InteractableDoorHinge>();

                    UnturnedPlayer player = (UnturnedPlayer)caller;

                    if (DoorPlugin.test.Door.Find(x => x.id == doorHinge.GetInstanceID().ToString()) != null)
                    {
                        var door = DoorPlugin.test.Door.Find(x => x.id == doorHinge.GetInstanceID().ToString());
                        DoorPlugin.test.Door.Remove(door);
                        DB.DataBaseManager.Save(DoorPlugin.test);
                        UnturnedChat.Say(caller, "Дверь удалена", UnityEngine.Color.yellow);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Дверь не зарегестрирована!", UnityEngine.Color.red);
                    }
                }
            }
        }
    }
}
