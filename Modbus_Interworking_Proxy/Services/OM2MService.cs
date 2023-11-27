using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace Modbus_Interworking_Proxy.Services
{
    public class OM2MService
    {
        private string _baseConnectionAddress = "in-name";
        private string _connectionAddress = null;
        private readonly HttpClient _httpClient;

        public OM2MService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OM2MHttpClient");
        }

        public async Task<string> ConnectToOM2M()
        {
            string rn = "Modbus-Interworking-Proxy";
            string api = "MIP";
            string rr = "true";
            string poa = "http://localhost:1000";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_baseConnectionAddress + "/" + rn);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    _connectionAddress = _baseConnectionAddress + "/" + rn;
                    return content;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while making the API call: {ex.Message}");
            }

            string payload = "{ \"m2m:ae\": { \"rn\": \"" + rn + "\", \"api\": \"" + api + "\", \"rr\": \"" + rr + "\", \"poa\": [\"" + poa + "\"] } }";

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseConnectionAddress);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json"); // Set custom Content-Type header

                // Set custom Content-Type header
                request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("ty", "2"));

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _connectionAddress = _baseConnectionAddress + "/" + rn;
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    throw new Exception($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while making the API call: {ex.Message}");
            }
        }
    }
}
