using NModbus;
using NModbus.Device;
using System.IO.Ports;
using System.Net.Sockets;

namespace Modbus_Interworking_Proxy.Services
{
    public class ModbusService
    {
        private readonly IModbusMaster _modbusMaster;

        public ModbusService(string ip, int port)
        {
            using TcpClient client = new TcpClient(ip, port);
            ModbusFactory factory = new ModbusFactory();
            _modbusMaster = factory.CreateMaster(client);
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
