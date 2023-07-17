using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using System;

namespace barash.Doors.Commands
{
    public class AddDoorCommand : IRocketCommand
    {

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "NewDoor";

        public string Help => "/AddDoor [адрес(можно писать любую хуйню)] [гос.цена продажи]";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { "newdoor", "AD" };

        public List<string> Permissions => new List<string> { "new.door" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var raycast = DoorPlugin.Raycast(caller);
            if (raycast != null)
            {
                if (raycast.GetComponent<InteractableDoorHinge>() != null)
                {
                    InteractableDoorHinge doorHinge = raycast.GetComponent<InteractableDoorHinge>();
                    ///DoorPlugin.Instance.Configuration.Instance.SaveData(raycast.parent.parent, command, caller);
                    
                    UnturnedPlayer player = (UnturnedPlayer)caller;
                    
                    if(DoorPlugin.test.Door.Find(x => x.id == doorHinge.GetInstanceID().ToString()) == null)
                    {
                        Steamworks.CSteamID stid = new Steamworks.CSteamID(123);
                        DoorPlugin.test.Door.Add(new Door { address = command[0], id = doorHinge.GetInstanceID().ToString(), owner_id = stid, owner_name = "Государство", lodger_1 = "", lodger_2 = "", lodger_3 = "", price = int.Parse(command[1]) });
                        DB.DataBaseManager.Save(DoorPlugin.test);
                        UnturnedChat.Say(caller, "Дверь успешно зарегистрирована!", UnityEngine.Color.green);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Дверь ужу зарегестрирована!", UnityEngine.Color.red);
                    }
               


                    ///InteractableDoor door = doorHinge.door;
                    ///bool open = !door.isOpen;
                    Interactable hinge = raycast.GetComponent<Interactable>();
                    ///door.updateToggle(open);
                    ///BarricadeManager.ServerSetDoorOpen(doorHinge.door, open);



                    UnturnedChat.Say(caller, "id door: " + doorHinge.GetInstanceID() + ", id player: " + player.CSteamID, UnityEngine.Color.green);
                }
                else
                {
                    UnturnedChat.Say(caller, "На дверь сомтри, чурркаш", UnityEngine.Color.red);
                }

            }
        }
    }
}
