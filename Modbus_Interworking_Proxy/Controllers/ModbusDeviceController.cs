using Microsoft.AspNetCore.Mvc;

namespace Modbus_Interworking_Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModbusDeviceController : ControllerBase
    {
        [HttpGet(Name = "GetTest")]
        public ObjectResult GetTest()
        {
            return Ok("Test");
        }
    }
}
