using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlessandroGozzi.MioEco_Material.Domain;
using AlessandroGozzi.MioEco_Material.Domain.Interfaces;

namespace AlessandroGozzi.MioEco_Material.Infrastructure
{
    public class ArasaacService : IArasaacService
    {
        private readonly HttpClient _httpClient;

        public ArasaacService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private class ArasaacApiResult
        {
            [JsonPropertyName("_id")]
            public int Id { get; set; }

            [JsonPropertyName("keywords")]
            public List<ArasaacKeyword>? Keywords { get; set; }
        }

        private class ArasaacKeyword
        {
            [JsonPropertyName("keyword")]
            public string Keyword { get; set; } = string.Empty;
        }

        public async Task<List<ArasaacPictogramDto>> SearchPictogramsAsync(string searchTerm)
        {
            var results = new List<ArasaacPictogramDto>();
            if (string.IsNullOrWhiteSpace(searchTerm)) return results;

            try
            {
                string url = $"https://api.arasaac.org/api/pictograms/it/search/{Uri.EscapeDataString(searchTerm.Trim())}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return results;

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<ArasaacApiResult>>(json);

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        var kw = item.Keywords?.FirstOrDefault()?.Keyword ?? searchTerm;
                        results.Add(new ArasaacPictogramDto
                        {
                            Id = item.Id,
                            Keyword = kw
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore Arasaac: {ex.Message}");
            }

            return results;
        }
    }
}
