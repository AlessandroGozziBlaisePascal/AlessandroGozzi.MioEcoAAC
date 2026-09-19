using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Application.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task InitializeAndSeedAsync();
    }
}
