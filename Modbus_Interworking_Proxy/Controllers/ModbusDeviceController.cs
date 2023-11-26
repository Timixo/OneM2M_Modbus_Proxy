using Microsoft.AspNetCore.Mvc;
using Modbus_Interworking_Proxy.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Modbus_Interworking_Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModbusDeviceController : ControllerBase
    {
        private string _connectionAddress = "in-name";
        private readonly HttpClient _httpClient;

        public ModbusDeviceController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OM2MHttpClient");
        }

        [HttpGet]
        public async Task<IActionResult> GetDevice([FromQuery] string id)
        {
            try
            {
                // Set the Accept header to specify that we want JSON
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await _httpClient.GetAsync(_connectionAddress + "?api=" + id);

                if (response.IsSuccessStatusCode)
                {
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

        [HttpPost("ConnectOM2M")]
        public async Task<IActionResult> ConnectToOM2M([FromBody] OM2MServerConnectModel model)
        {
            if (string.IsNullOrEmpty(model.ConnectionAddress))
            {
                return BadRequest("ConnectionAddress is required");
            }

            //var payload = new
            //{
            //    m2m = new
            //    {
            //        ae = new
            //        {
            //            rn = "Modbus Interworking Proxy",
            //            api = "MIP"
            //        }
            //    }
            //};

            string payload = "{ \"m2m:ae\": { \"rn\": \"Modbus-Interworking-Proxy\", \"api\": \"MIP\", \"rr\": \"true\", \"poa\": [\"http://localhost:1000\"] } }";

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, model.ConnectionAddress);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json"); // Set custom Content-Type header

                // Set custom Content-Type header
                request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("ty", "2"));

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
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
