using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsService;

public class SensorValue
{
    public string Id { get; set; }
    public string RoomId { get; set; }
    public string Date { get; set; }
    public int Temperature { get; set; }
    public string OutIn { get; set; }
}
