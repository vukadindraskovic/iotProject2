using MQTTnet.Client;
using MQTTnet;
using System.Text;
using AnalyticsService;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Drawing;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

var sensorDummyTopic = "sensor_dummy/values";
var eKuiperTopic = "eKuiper/anomalies"; // broker.emqx.io
string address = "192.168.100.6";
var port = 1883;
var client = InfluxDBClientFactory.Create(url: "http://192.168.100.6:8086", "admin", "adminadmin".ToCharArray());
int i = 1;

var mqttService = MqttService.Instance();

await mqttService.ConnectAsync(address, port);
await mqttService.SubsribeToTopicsAsync(new List<string> { sensorDummyTopic, eKuiperTopic });

async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
{
    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    if (e.ApplicationMessage.Topic == sensorDummyTopic)
    {
        mqttService.PublishMessage("analytics/values", payload);
        return;
    }

    Console.WriteLine($"eKuiper send: {payload}");
    var data = (JObject)JsonConvert.DeserializeObject(payload);
    string roomId = data.SelectToken("room_id").Value<string>();
    string date = data.SelectToken("noted_date").Value<string>();
    int temperature = data.SelectToken("temp").Value<int>();

    await WriteToDatabase(temperature, date, roomId);
}

async Task WriteToDatabase(int temp, string date, string roomId)
{
    var point = PointData
        .Measurement("temperature")
        .Tag("room", roomId)
        .Tag("date", date)
        .Field("celsius_degrees", temp)
        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

    await client.GetWriteApiAsync().WritePointAsync(point, "iot2", "organization");
    Console.WriteLine($"Write in InfluxDb: temperature{i}");
    i++;
}

mqttService.AddApplicationMessageReceived(ApplicationMessageReceivedAsync);

while (true) ;