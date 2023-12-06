using Modbus_Interworking_Proxy.Models;
using NModbus;
using NModbus.Device;
using NModbus.Message;
using System.Net;
using System.Net.Sockets;

namespace Modbus_Interworking_Proxy.Services
{
    public class ModbusService
    {
        private readonly TcpClient _client;
        private readonly ModbusFactory _factory;
        private readonly IModbusMaster _modbusMaster;
        private readonly IModbusSlaveNetwork _modbusSlaveNetwork;

        public ModbusService(string ip, int port)
        {
            try
            {
                _client = new TcpClient(ip, port);
                _factory = new ModbusFactory();
                _modbusMaster = _factory.CreateMaster(_client);
                IPAddress ipAddress = IPAddress.Parse(ip);
                // TODO Maybe we should create this outside and when we don't want to use it anymore close it
                TcpListener tcpListener = new TcpListener(ipAddress, port);
                _modbusSlaveNetwork = _factory.CreateSlaveNetwork(tcpListener);
            }
            catch(Exception)
            {
                return;
            } 
        }

        public void Dispose()
        {
            _modbusSlaveNetwork.Dispose();
            _modbusMaster.Dispose();
            _client.Dispose();
        }

        public ModbusDeviceModel LinkModbusDevice(ModbusDeviceModel model)
        {
            try
            {
                // model.Id doesn't do anything yet, we need to figure out how to get data from a device with the slave id, which is not the same as slave address
                ushort[] data = _modbusMaster.ReadHoldingRegisters(model.Id, 0, 1);

                IModbusSlave slave = _factory.CreateSlave(model.Id);
                _modbusSlaveNetwork.AddSlave(slave);
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
                _modbusSlaveNetwork.GetSlave(slaveId);
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
