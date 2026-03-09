var data = new List<RawData>
{
    new RawData { DeviceId = "dev-1",    Voltage = 220, Current = 2.0, Temperature = 24 },
    new RawData { DeviceId = "dev-1",    Voltage = 230, Current = 1.5, Temperature = 29 },
    new RawData { DeviceId = "dev-2",    Voltage = 110, Current = 5.0, Temperature = 9  },

    null,

    new RawData { DeviceId = "",         Voltage = 220, Current = 1.0, Temperature = 20 },
    new RawData { DeviceId = "   ",      Voltage = 220, Current = 1.0, Temperature = 20 },

    new RawData { DeviceId = "dev-nan",  Voltage = double.NaN,              Current = 1.0,                Temperature = 20 },
    new RawData { DeviceId = "dev-inf",  Voltage = 220,                     Current = double.PositiveInfinity, Temperature = 20 },

    new RawData { DeviceId = "dev-div0", Voltage = 220, Current = 1.0, Temperature = -1.0      },
    new RawData { DeviceId = "dev-near", Voltage = 220, Current = 1.0, Temperature = -0.9999995 },

    new RawData { DeviceId = "dev-neg",  Voltage = 100, Current = 2.0, Temperature = -0.5 },
};

var service = new EnergyAnalyticsService();
var result = service.CalculateEfficiencyMetrics(data);

Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
