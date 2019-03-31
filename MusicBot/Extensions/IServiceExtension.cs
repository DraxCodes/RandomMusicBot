using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Extensions
{
    public interface IServiceExtension
    {
        Task InitializeAsync();
    }
}
