using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MusicBot.Storage.Entities;
using Newtonsoft.Json;

namespace MusicBot.Storage
{
    class PersistentStorage : IStorage
    {
        public void CreatePath(string path)
        {
            File.WriteAllText(path, "");
        }

        public void InitializeResources(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public bool FileExists(string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public async Task<T> Retreive<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task Store(object obj, string path)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            await File.WriteAllTextAsync(path, json);
        }
    }
}
