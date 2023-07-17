using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace barash.Doors
{
    public class Server
    {
        [XmlArray("Door")]
        public List<Door> Door = new List<Door>();

        [XmlArray("Duty")]
        public List<Duty> Duty = new List<Duty>();
        public Server()
        {

        }
    }
    public class Duty
    {
        public Steamworks.CSteamID debtor;

        public uint duty;

        public Duty()
        {

        }
        public Duty(Steamworks.CSteamID debtor, uint duty)
        {
            this.debtor = debtor;
            this.duty = duty;
        }
    }
    public class Door
    {
        public string id;

        public string address;

        public Steamworks.CSteamID owner_id;

        public string owner_name;

        public string lodger_1;

        public string lodger_2;

        public string lodger_3;

        public int price;

        public Door()
        {

        }

        public Door(string id, string address, Steamworks.CSteamID owner_id, string owner_name, string lodger_1, string lodger_2, string lodger_3, int price)
        {
            this.address = address;
            this.id = id;
            this.owner_id = owner_id;
            this.owner_name = owner_name;
            this.lodger_1 = lodger_1;
            this.lodger_2 = lodger_2;
            this.lodger_3 = lodger_3;
            this.price = price;
        }
    }
}
