using Microsoft.Extensions.Configuration;
using Modbus_Interworking_Proxy.Services;
using System.Net.Http.Headers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<OM2MService>();

string modbusIpAddress = config.GetValue<string>("Modbus:IpAddress");
int modbusPort = config.GetValue<int>("Modbus:Port");
builder.Services.AddScoped(provider => new ModbusService(modbusIpAddress, modbusPort));

// Add HttpClient configuration
builder.Services.AddHttpClient("OM2MHttpClient", client =>
{
    // Set the base address of your API
    client.BaseAddress = new Uri("http://127.0.0.1:8080/");
    // Set default headers
    client.DefaultRequestHeaders.Add("X-M2M-RI", "123");
    client.DefaultRequestHeaders.Add("X-M2M-Origin", "admin:admin");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();