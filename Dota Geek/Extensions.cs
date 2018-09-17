using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Dota_Geek
{
    public static class Extensions
    {
        private static List<ulong> ProUsers { get; set; }

        static Extensions()
        {
            if (File.Exists("Resources/ProList.json"))
            {
                var file = File.ReadAllText("Resources/ProList.json");
                ProUsers = JsonConvert.DeserializeObject<List<ulong>>(file);
            }
            else
            {
                ProUsers = new List<ulong>();
                Save();
            }
        }

        private static void Save()
        {
            var json = JsonConvert.SerializeObject(ProUsers, Formatting.Indented);
            File.WriteAllText("Resources/ProList.json", json);
        }

        public static bool IsPro(this SocketUser socketUser)
        {
            return ProUsers.Contains(socketUser.Id);
        }

        public static bool MakePro(this SocketUser socketUser)
        {
            if (socketUser.IsPro())
            {
                return false;
            }

            ProUsers.Add(socketUser.Id);
            Save();
            return true;
        }

        public static bool RemovePro(this SocketUser socketUser)
        {
            if (!socketUser.IsPro())
            {
                return false;
            }

            ProUsers.Remove(socketUser.Id);
            Save();
            return true;
        }
    }
}
