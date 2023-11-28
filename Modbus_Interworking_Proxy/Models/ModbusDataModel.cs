namespace Modbus_Interworking_Proxy.Models
{
    public class ModbusDataModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ModbusDeviceModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public List<string> Fields { get; set; }
    }

    public class ModbusDeviceDataModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public List<ModbusDataModel> Data { get; set; }
    }
}
