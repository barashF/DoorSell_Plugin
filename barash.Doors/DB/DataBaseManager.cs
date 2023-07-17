using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace barash.Doors.DB
{
    class DataBaseManager
    {
        public static Server GetServerFromDB()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));
            using (FileStream fs = new FileStream("Plugins/Doors.xml", FileMode.OpenOrCreate))
            {
                Server server = (Server)xmlSerializer.Deserialize(fs);
                return server;

            }
        }

        public static void SerializeServ()
        {
            Server server = new Server();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));

            using(FileStream fs = new FileStream("Plugins/Doors.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, server);
            }
        }

        public static void Save(Server server)
        {
            FileInfo fileInfo = new FileInfo("Plugins/Doors.xml");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Server));
            fileInfo.Delete();

            using (FileStream fs = new FileStream("Plugins/Doors.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, server);
            }
        }
    }
}
