using MusicBot.Storage.Entities;
using System.Threading.Tasks;

namespace MusicBot.Storage
{
    public interface IStorage
    {
        Task<T> Retreive<T>(string path);
        Task Store(object obj, string path);
        bool FileExists(string path);
        void CreatePath(string path);
        void InitializeResources(string directory);
    }
}