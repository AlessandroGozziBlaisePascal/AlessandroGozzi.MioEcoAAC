using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Domain.Dtos
{
    public class ArasaacPictogramDto
    {
        [JsonPropertyName("_id")]
        public int Id { get; set; }

        [JsonPropertyName("keywords")]
        public List<ArasaacKeywordDto>? Keywords { get; set; }

        [JsonIgnore]
        public string ImageUrl => $"https://static.arasaac.org/pictograms/{Id}/{Id}_500.png";
    }
}
