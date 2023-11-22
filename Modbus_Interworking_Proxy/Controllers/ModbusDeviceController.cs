using Microsoft.AspNetCore.Mvc;

namespace Modbus_Interworking_Proxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModbusDeviceController : ControllerBase
    {
        public ModbusDeviceController() { }

        [HttpGet("Test")]
        public ObjectResult GetTest()
        {
            return Ok("Test");
        }

        [HttpGet("Test2")]
        public ObjectResult GetTest2()
        {
            return Ok("Test2");
        }
    }
}
