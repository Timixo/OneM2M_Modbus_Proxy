using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace Modbus_Interworking_Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModbusDeviceController : ControllerBase
    {
        private string _baseConnectionAddress = "in-name";
        private string _connectionAddress = null;
        private readonly HttpClient _httpClient;

        public ModbusDeviceController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OM2MHttpClient");
        }

        [HttpPost("ConnectOM2M")]
        public async Task<IActionResult> ConnectToOM2M()
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
                    return Ok(content);
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"An error occurred while making the API call: {ex.Message}");
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
                    return Ok(content);
                } 
                else
                {
                    return StatusCode((int)response.StatusCode, $"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"An error occurred while making the API call: {ex.Message}");
            }
        }
    }
}
