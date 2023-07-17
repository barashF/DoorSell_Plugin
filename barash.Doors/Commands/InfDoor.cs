using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using SQLite;
using Rocket.Unturned.Player;
using System;

namespace barash.Doors.Commands
{
    class InfDoor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "InfDoor";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "infdoor" };

        public List<string> Permissions => new List<string> { "add.door", "inf.door"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if (raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    InteractableDoorHinge doorHinge = raycast.GetComponent<InteractableDoorHinge>();

                    if (DoorPlugin.test.Door.Find(x => x.id == doorHinge.GetInstanceID().ToString()) != null)
                    {
                        var door = DoorPlugin.test.Door.Find(x => x.id == doorHinge.GetInstanceID().ToString());
                        UnturnedChat.Say(caller, "Владелец: " + door.owner_name + " - " + door.owner_id, UnityEngine.Color.green);
                        UnturnedChat.Say(caller, "Адрес: " + door.address + ", id: " + door.id, UnityEngine.Color.green);
                        UnturnedChat.Say(caller, "Жилец 1: " + door.lodger_1, UnityEngine.Color.green);
                        UnturnedChat.Say(caller, "Жилец 2: " + door.lodger_2, UnityEngine.Color.green);
                        UnturnedChat.Say(caller, "Жилец 3: " + door.lodger_3, UnityEngine.Color.green);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Дверь не зарегестрирована!", UnityEngine.Color.red);
                    }

                }
                else
                {
                    UnturnedChat.Say(caller, "На дверь сомтри, чурркаш", UnityEngine.Color.red);
                }

            }
        }
    }
}
