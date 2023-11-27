using Microsoft.AspNetCore.Mvc;
using Modbus_Interworking_Proxy.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Modbus_Interworking_Proxy.Services
{
    public class OM2MService
    {
        private static string _baseConnectionAddress = "in-name";
        private static string _connectionAddress = null;
        private readonly HttpClient _httpClient;

        public OM2MService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OM2MHttpClient");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
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

        public async Task<string> ConnectModbusDevice(ModbusDeviceModel model)
        {
            try
            {
                // Check if the container exists
                HttpResponseMessage response = await _httpClient.GetAsync(_connectionAddress + "/" + model.Name);
                if (!response.IsSuccessStatusCode)
                {
                    // Create the container
                    string payload = "{ \"m2m:cnt\": { \"rn\": \"" + model.Name + "\" } }";
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _connectionAddress);
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("ty", "3"));

                    response = await _httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"API call failed with status code: {response.StatusCode}");
                    }
                }

                // Create data containers
                foreach (string field in model.Fields)
                {
                    response = await _httpClient.GetAsync(_connectionAddress + "/" + model.Name + "/" + field);
                    if (!response.IsSuccessStatusCode)
                    {
                        string payload = "{ \"m2m:cnt\": { \"rn\": \"" + field + "\" } }";
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _connectionAddress + "/" + model.Name);
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                        request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("ty", "3"));

                        response = await _httpClient.SendAsync(request);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"API call failed with status code: {response.StatusCode}");
                        }
                    }
                }

                return "Creation Succesfull";
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while making the API call: {ex.Message}");
            }
        }
    }
}
