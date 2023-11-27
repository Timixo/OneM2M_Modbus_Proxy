using Microsoft.AspNetCore.Mvc;
using Modbus_Interworking_Proxy.Services;
using System.Net.Http.Headers;
using System.Text;

namespace Modbus_Interworking_Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModbusDeviceController : ControllerBase
    {
        private readonly OM2MService _om2mService;
        private readonly ModbusService _modbusService;

        public ModbusDeviceController(OM2MService om2mService, ModbusService modbusService)
        {
            _om2mService = om2mService;
            _modbusService = modbusService;
        }

        [HttpPost("ConnectOM2M")]
        public async Task<IActionResult> ConnectToOM2M()
        {
            try
            {
                string response = await _om2mService.ConnectToOM2M();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
