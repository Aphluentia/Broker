using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationsManager.Configurations;

namespace OperationsManager.Database
{
    public class DatabaseProvider: IDatabaseProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _BaseUrl;
        public DatabaseProvider(IOptions<DatabaseApiConfigSection> options)
        {
            _httpClient = new HttpClient();
            _BaseUrl = options.Value.ConnectionString;
        }
        private string EndpointBuilder(DatabaseControllers controller, string? id = null, bool isModule = false, string? ModuleId = null)
        {
            string url = $"{_BaseUrl}/api/{controller.ToString()}";
            if (!string.IsNullOrEmpty(id)) url += $"/{id}";
            if (isModule)
            {
                url += "/Connection";
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(ModuleId))
                {
                    url += $"/{id}/{ModuleId}";
                }
            }
            
            return url;
        }

        public async Task<(bool, string?)> Get(DatabaseControllers controller, string? id = null, bool isModule = false)
        {
            var url = EndpointBuilder(controller, id, isModule);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return (true, responseContent);
        }

        public async Task<(bool, string?)> Post(object body, DatabaseControllers controller, string? id = null, bool isModule = false)
        {
            var url = EndpointBuilder(controller, id, isModule);
            var response = await _httpClient.PostAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return (true, responseContent);
        }

        public async Task<(bool, string?)> Put(object body, DatabaseControllers controller, string? id = null, bool isModule = false)
        {
            var url = EndpointBuilder(controller, id, isModule);
            var response = await _httpClient.PutAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return (true, responseContent);
        }

        public async Task<(bool, string?)> Delete(DatabaseControllers controller, string? id = null, bool isModule = false, string? ModuleId = null)
        {
            var url = EndpointBuilder(controller, id, isModule, ModuleId);
            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return (true, responseContent);
        }

       

    }
}
