using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlessandroGozzi.AppAAC.Domain.Dtos
{
    public class ArasaacKeywordDto
    {
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int? Type { get; set; }

        [JsonPropertyName("plural")]
        public string? Plural { get; set; }
    }
}
