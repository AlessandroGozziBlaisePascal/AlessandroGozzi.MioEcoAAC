using System;
using System.Collections.Generic;
using System.Text;

namespace AlessandroGozzi.MioEco_Material.Domain
{
    public class ArasaacPictogramDto
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public string ImageUrl => $"https://static.arasaac.org/pictograms/{Id}/{Id}_500.png";
    }
}
