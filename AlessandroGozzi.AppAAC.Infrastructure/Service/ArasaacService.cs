using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.Dtos;
using AlessandroGozzi.AppAAC.Domain.Interfaces;

namespace AlessandroGozzi.AppAAC.Infrastructure.Service
{
    public class ArasaacService : IArasaacService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ArasaacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.arasaac.org/api/");
        }

        public async Task<List<ArasaacPictogramDto>> SearchPictogramsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ArasaacPictogramDto>();

            try
            {
                var cleanTerm = Uri.EscapeDataString(searchTerm.Trim().ToLowerInvariant());
                var url = $"pictograms/it/bestsearch/{cleanTerm}";
            var results = await _httpClient.GetFromJsonAsync<List<ArasaacPictogramDto>>(url, JsonOptions);
                return results ?? new List<ArasaacPictogramDto>();
            }
            catch
            {
                // Fallback per assenza di rete o termine non trovato
                return new List<ArasaacPictogramDto>();
            }
        }
    }
}
