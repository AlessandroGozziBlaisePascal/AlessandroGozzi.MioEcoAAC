using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Dtos;

namespace AlessandroGozzi.AppAAC.Domain.Interfaces
{
    public interface IArasaacService
    {
        Task<List<ArasaacPictogramDto>> SearchPictogramsAsync(string searchTerm);
    }
}
