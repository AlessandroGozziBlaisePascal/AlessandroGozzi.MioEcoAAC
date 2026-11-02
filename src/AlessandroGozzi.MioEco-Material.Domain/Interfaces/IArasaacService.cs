using System;
using System.Collections.Generic;
using System.Text;

namespace AlessandroGozzi.MioEco_Material.Domain.Interfaces
{
    public interface IArasaacService
    {
        Task<List<ArasaacPictogramDto>> SearchPictogramsAsync(string searchTerm);
    }
}
