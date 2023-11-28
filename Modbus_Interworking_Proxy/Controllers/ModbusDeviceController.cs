using Microsoft.AspNetCore.Mvc;
using Modbus_Interworking_Proxy.Models;
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

        [HttpPost("ConnectModbusDevice")]
        public async Task<IActionResult> ConnectModbusDevice([FromBody] ModbusDeviceModel model)
        {
            try
            {
                _modbusService.LinkModbusDevice(model);
                string response = await _om2mService.ConnectModbusDevice(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetModbusDeviceData")]
        public async Task<IActionResult> GetModbusDeviceData([FromBody] ModbusDeviceModel model)
        {
            try
            {
                ushort[] data = _modbusService.ReadHoldingRegisters(model.Id, 0, (ushort)model.Fields.Count);
                ModbusDeviceDataModel dataModel = new ModbusDeviceDataModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Data = new List<ModbusDataModel>()
                };
                dataModel.Data.AddRange(data.Select((value, index) => new ModbusDataModel { Name = model.Fields[index], Value = value.ToString() }));

                string response = await _om2mService.PutModbusDeviceData(dataModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
