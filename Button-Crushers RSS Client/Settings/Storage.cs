using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RSSEngine;
using System.IO;

namespace StorageEngine
{
    public static class Storage
    {
        private const string config = "CONFIG";

        public static void Save()
        {
            // Serialize the subscriptions in RSSEngine
            XmlSerializer xml = new XmlSerializer(typeof(Subscriptions));
            xml.Serialize(new FileStream(config, FileMode.CreateNew), Subscriptions);
        }

        public static void Load()
        {

        }
    }
}
