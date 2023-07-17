using Rocket.API;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using Logger = Rocket.Core.Logging.Logger;
using System.Timers;
using UnityEngine.UI;
using Rocket.Unturned;

namespace barash.Doors
{
    public class DoorPlugin : RocketPlugin<Config>
    {
        public static DoorPlugin Instance;
        public static Server test;
        private Timer timer;
        #region Loading/Unloading
        protected override void Load()
        {
            base.Load();
            Instance = this;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEvents_OnPlayerUpdateGesture;
            EffectManager.onEffectButtonClicked += OnClicked;
            EffectManager.onEffectTextCommitted += OnText;
            timer = new Timer(86400000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            try
            {
                test = DB.DataBaseManager.GetServerFromDB();
            }
            catch
            {
                DB.DataBaseManager.SerializeServ();
            }
            test = DB.DataBaseManager.GetServerFromDB();
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if(DoorPlugin.test.Duty.Find(y => y.debtor == player.CSteamID) != null)
            {
                player.Experience += DoorPlugin.test.Duty.Find(y => y.debtor == player.CSteamID).duty;
                UnturnedChat.Say(player, "Вы успешно продали недвижимость за: " + DoorPlugin.test.Duty.Find(y => y.debtor == player.CSteamID).duty, UnityEngine.Color.green);
                var door = DoorPlugin.test.Duty.Find(y => y.debtor == player.CSteamID);
                DoorPlugin.test.Duty.Remove(door);
                DB.DataBaseManager.Save(DoorPlugin.test);
            }
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
            Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEvents_OnPlayerUpdateGesture;
            EffectManager.onEffectButtonClicked -= OnClicked;
            EffectManager.onEffectTextCommitted -= OnText;
            timer.Elapsed -= OnTimerElapsed;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
        }
        public void OnTimerElapsed(object sender, ElapsedEventArgs args)
        {
            UnturnedChat.print("Плоти Налоги!");
        }
        public override TranslationList DefaultTranslations => new TranslationList
        {

        };
        #endregion

        private void UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture gesture)
        {
            Transform raycast = Raycast(player);

            if (gesture.Equals(Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchRight) && raycast != null && Instance.Configuration.Instance.OpenOnHit == true
             )
            {
                string id;
                try
                {
                    id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID().ToString();
                }
                catch
                {
                    id = "";
                }
                if (Raycast(player).GetComponent<InteractableDoorHinge>() != null && DoorPlugin.test.Door.Find(x => x.id == id.ToString()) != null)
                {                
                    Execute(player, id);
                }
            }
            else if(gesture.Equals(Rocket.Unturned.Events.UnturnedPlayerEvents.PlayerGesture.PunchLeft) && raycast != null)
            {
                InteractableDoorHinge d;
                string id;
                try
                {
                    d = Raycast(player).GetComponent<InteractableDoorHinge>();
                    id = d.GetInstanceID().ToString();
                }
                catch
                {
                    d = null;
                    id = "";
                }
                if (Raycast(player).GetComponent<InteractableDoorHinge>() != null && DoorPlugin.test.Door.Find(x => x.id == id.ToString()) != null)
                {
                    if(player.CSteamID == DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id || player.CSteamID.ToString().Equals(DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_1) || player.CSteamID.ToString().Equals(DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_2) || player.CSteamID.ToString().Equals(DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_3))
                    {
                        InteractableDoorHinge doorHinge = raycast.GetComponent<InteractableDoorHinge>();
                        InteractableDoor door = doorHinge.door;
                        bool open = !door.isOpen;
                        BarricadeManager.ServerSetDoorOpen(doorHinge.door, open);
                    }
                }
            }
        }
        string price = "";
        string lodger_1 = "";
        string lodger_2 = "";
        string lodger_3 = "";
        public void OnText(Player uplayer, string field_name, string text)
        {
            UnturnedPlayer ownerplayer = UnturnedPlayer.FromPlayer(uplayer);
            if (field_name == "InputFieldPrice")
            {
                price = text;
            }
            if(field_name == "InputFieldLodger1")
            {
                lodger_1 = text;
            }
            if (field_name == "InputFieldLodger2")
            {
                lodger_2 = text;
            }
            if (field_name == "InputFieldLodger3")
            {
                lodger_3 = text;
            }
        }
        public void Execute(IRocketPlayer caller, string id)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            EffectManager.sendUIEffect(20063, 1, player.CSteamID, true);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            if(DoorPlugin.test.Door.Find(x => x.id == id).price == 0)
            {
                if (player.CSteamID == DoorPlugin.test.Door.Find(x => x.id == id).owner_id)
                {
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "addressinp", DoorPlugin.test.Door.Find(x => x.id == id).address);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "ownerinp", DoorPlugin.test.Door.Find(x => x.id == id).owner_name);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "imageinput3", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_buy", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_remove", false);

                }
                else
                {
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "addressinp", DoorPlugin.test.Door.Find(x => x.id == id).address);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "ownerinp", DoorPlugin.test.Door.Find(x => x.id == id).owner_name);

                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonSell", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonLodgers", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "imageinput3", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_buy", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_remove", false);
                }
            }
            else
            {
                if (player.CSteamID == (DoorPlugin.test.Door.Find(x => x.id == id).owner_id))
                {
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "addressinp", DoorPlugin.test.Door.Find(x => x.id == id).address);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "ownerinp", DoorPlugin.test.Door.Find(x => x.id == id).owner_name);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "priceinp", DoorPlugin.test.Door.Find(x => x.id == id).price.ToString());
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonLodgers", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonSell", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_buy", false);
                }
                else
                {
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "addressinp", DoorPlugin.test.Door.Find(x => x.id == id).address);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "ownerinp", DoorPlugin.test.Door.Find(x => x.id == id).owner_name);
                    EffectManager.sendUIEffectText(1, player.CSteamID, true, "priceinp", DoorPlugin.test.Door.Find(x => x.id == id).price.ToString());
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonLodgers", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "ButtonSell", false);
                    EffectManager.sendUIEffectVisibility(1, player.CSteamID, false, "Button_remove", false);
                }
            }
        }
        public void OnClicked(Player uplayer, string button)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(uplayer);
            if (button == "button_close")
            {
                EffectManager.askEffectClearByID(20063,  player.CSteamID);
                EffectManager.askEffectClearByID(20051, player.CSteamID);
                EffectManager.askEffectClearByID(20052, player.CSteamID);
                EffectManager.askEffectClearByID(20053, player.CSteamID);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            else if(button == "ButtonSell")
            {
                
                EffectManager.askEffectClearByID(20063, player.CSteamID);
                EffectManager.sendUIEffect(20052, 1, player.CSteamID, true);
                
            }
            else if(button == "Button_buy")
            {
                var id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID();
                if (player.Experience >= (uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price)
                {
                    if((uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price != 0)
                    {
                        player.Experience -= (uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price;

                        try
                        {
                            UnturnedPlayer player2 = UnturnedPlayer.FromCSteamID(DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id);
                            player2.Experience += (uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price;
                        }
                        catch
                        {
                            if (DoorPlugin.test.Duty.Find(y => y.debtor == DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id) == null)
                            {
                                DoorPlugin.test.Duty.Add(new Duty { debtor = DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id, duty = (uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price });
                                DB.DataBaseManager.Save(DoorPlugin.test);
                            }
                            else
                            {
                                DoorPlugin.test.Duty.Find(y => y.debtor == DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id).duty += (uint)DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price;
                                DB.DataBaseManager.Save(DoorPlugin.test);
                            }
                        }
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price = 0;
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_name = player.CharacterName;
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).owner_id = player.CSteamID;
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_1 = "";
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_2 = "";
                        DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_3 = "";
                        DB.DataBaseManager.Save(DoorPlugin.test);
                    }
                    else
                    {
                        UnturnedChat.Say(player, "Дом был куплен другим игроком", UnityEngine.Color.red);
                    }
                }
                else
                {
                    UnturnedChat.Say(player, "У вас недостаточно средств для покупки", UnityEngine.Color.red);
                }
                EffectManager.askEffectClearByID(20063, player.CSteamID);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);

            }
            else if(button == "ButtonGoSell")
            {
                var id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID();
                DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price = int.Parse(price);
                DB.DataBaseManager.Save(DoorPlugin.test);
                UnturnedChat.Say(player, "Дом успешно выставлен на продажу", UnityEngine.Color.yellow);
                EffectManager.askEffectClearByID(20052, player.CSteamID);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                price = "";

            }
            else if(button == "Button_remove")
            {
                var id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID();
                DoorPlugin.test.Door.Find(x => x.id == id.ToString()).price = 0;
                DB.DataBaseManager.Save(DoorPlugin.test);
                EffectManager.askEffectClearByID(20063, player.CSteamID);
                UnturnedChat.Say(player, "Дом снят с продажи", UnityEngine.Color.yellow);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            else if(button == "ButtonLodgers")
            {
                var id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID();
                EffectManager.askEffectClearByID(20063, player.CSteamID);
                EffectManager.sendUIEffect(20053, 1, player.CSteamID, true);
                EffectManager.sendUIEffectText(1, player.CSteamID, true, "InputFieldLodger1", DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_1);
                EffectManager.sendUIEffectText(1, player.CSteamID, true, "InputFieldLodger2", DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_2);
                EffectManager.sendUIEffectText(1, player.CSteamID, true, "InputFieldLodger3", DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_3);
                lodger_1 = DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_1;
                lodger_2 = DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_2;
                lodger_3 = DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_3;
            }
            else if(button == "ButtonLodgersSave")
            {
                var id = Raycast(player).GetComponent<InteractableDoorHinge>().GetInstanceID();
                DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_1 = lodger_1;
                DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_2 = lodger_2;
                DoorPlugin.test.Door.Find(x => x.id == id.ToString()).lodger_3 = lodger_3;
                DB.DataBaseManager.Save(DoorPlugin.test);
                EffectManager.askEffectClearByID(20053, player.CSteamID);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
        }

        //Checks If The Player Has One Of The Required Permissions
        public static bool CheckPermissions(IRocketPlayer caller, List<string> perms)
        {
            if (perms.Count > 0)
            {
                foreach (var i in perms)
                {
                    foreach (var t in caller.GetPermissions())
                    {
                        if (t.Name == i)
                        {
                            return true;
                        }
                    }
                }
            }
            else { return true; }
            return true;
        }

        public void DeleteData(Transform transform, string[] permissions, IRocketPlayer rocketPlayer)
        {
            var i = Instance.Configuration.Instance.conf.Find(c => new Vector3 { x = c.transform.x, y = c.transform.y, z = c.transform.z } == Raycast(rocketPlayer).parent.parent.position);
            if (i != null)
            {
                Instance.Configuration.Instance.conf.Remove(i);
                Instance.Configuration.Save();
            }
            else
            {
            }

        }

        public static bool ShouldOpen(Transform transform)
        {
            if (transform.GetComponent<InteractableDoorHinge>() != null)
            {
                transform = transform.parent.parent;
                if (transform.GetComponent<InteractableDoor>().isOpen)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        #region DoorPackets
        public static void OpenDoor(Transform transform, bool ShouldOpen)
        {
            byte x;
            byte y;
            BarricadeRegion r;
            ushort index;
            ushort plant;


            if (BarricadeManager.tryGetInfo(transform, out x, out y, out plant, out index, out r))
            {

                BarricadeManager.instance.channel.send("askToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] {
                        x,
                        y,
                        plant,
                        index,
                        ShouldOpen

                    });
                BarricadeManager.instance.channel.send("tellToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] {
                        x,
                        y,
                        plant,
                        index,
                        ShouldOpen

                    });
            }
        }
        #endregion
        public static Transform Raycast(IRocketPlayer rocketPlayer)
        {
            RaycastHit hit;
            UnturnedPlayer player = (UnturnedPlayer)rocketPlayer;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out hit, DoorPlugin.Instance.Configuration.Instance.OpenDistance, RayMasks.BARRICADE_INTERACT))
            {
                Transform transform = hit.transform;


                return transform;
            }
            return null;
        }
    }
}
