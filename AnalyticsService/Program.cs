using MQTTnet.Client;
using MQTTnet;
using System.Text;
using AnalyticsService;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using InfluxDB.Client;

var sensorDummyTopic = "sensor_dummy/values";
var eKuiperTopic = "eKuiper/anomalies";

var mqttService = MqttService.Instance();

await mqttService.ConnectAsync("broker.emqx.io");
await mqttService.SubsribeToTopicsAsync(new List<string> { sensorDummyTopic, eKuiperTopic });

Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
{
    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    if (e.ApplicationMessage.Topic == sensorDummyTopic)
    {
        mqttService.PublishMessage("analytics/values", payload);
        return Task.CompletedTask;
    }

    /*var data = (JObject)JsonConvert.DeserializeObject(payload);
    string id = data.SelectToken("id").Value<string>();
    string roomId = data.SelectToken("room_id").Value<string>();
    string date = data.SelectToken("noted_date").Value<string>();
    int temperature = data.SelectToken("temp").Value<int>();
    string outIn = data.SelectToken("out_in").Value<string>();

    var client = new InfluxDBClient("http://127.0.0.1:8086", "L5_mcTPl70ui3VhT98P-nbTAZfOd1k8TX6KawjLoL_C7Pwf_QUbQVlzt2TvWq_WYxBb5GJVUTwtO2QVXtzZojA==");

    using (var writeApi = client.GetWriteApi())
    {
        var point = PointData.Measurement("temperature")
            .Tag("location", "in")
            .Field("value", temperature)
            .Timestamp(DateTime.Parse(date), WritePrecision.Ns);

        writeApi.WritePoint(point, "iot2", "organization");
        Console.WriteLine("SRECA SRECA RADOST");
    }*/

    Console.WriteLine(payload);

    return Task.CompletedTask;
}

mqttService.AddApplicationMessageReceived(ApplicationMessageReceivedAsync);

while (true) ;