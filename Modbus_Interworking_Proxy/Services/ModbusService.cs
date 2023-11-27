using Modbus_Interworking_Proxy.Models;
using NModbus;
using NModbus.Message;
using System.Net.Sockets;

namespace Modbus_Interworking_Proxy.Services
{
    public class ModbusService
    {
        private readonly TcpClient _client;
        private readonly ModbusFactory _factory;
        private readonly IModbusMaster _modbusMaster;

        public ModbusService(string ip, int port)
        {
            try
            {
                _client = new TcpClient(ip, port);
                _factory = new ModbusFactory();
                _modbusMaster = _factory.CreateMaster(_client);
            }
            catch(Exception)
            {
                return;
            }
        }

        public void Dispose()
        {
            _modbusMaster.Dispose();
            _client.Dispose();
        }

        public ModbusDeviceModel LinkModbusDevice(ModbusDeviceModel model)
        {
            try
            {
                ushort[] data = _modbusMaster.ReadHoldingRegisters(model.Id, 0, 1);
                return model;
            }
            catch(Exception)
            {
                throw new Exception("Error connecting to Modbus device");
            }
        }

        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort length)
        {
            try
            {
                return _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading holding registers: {ex.Message}");
                throw;
            }
        }
    }
}
