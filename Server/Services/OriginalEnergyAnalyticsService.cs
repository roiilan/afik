using System;
using System.Collections.Generic;
using System.Linq;

public class EnergyAnalyticsService
{
    // פונקציה לחישוב מדדי נצילות - דוגמה לקוד שדורש שיפור לוגי וביצועי
    public List<DeviceResult> CalculateEfficiencyMetrics(List<RawData> data)
    {
        var results = new List<DeviceResult>();
        
        foreach (var item in data)
        {
            double powerUsage = item.Voltage * item.Current;
            
            double efficiencyFactor = Math.Pow(Math.Sqrt(powerUsage * 0.85), 2) / (item.Temperature + 1);
            
            var existingDevice = results.FirstOrDefault(r => r.DeviceId == item.DeviceId);
            
            if (existingDevice != null)
            {
                existingDevice.TotalPower += powerUsage;
                existingDevice.ReadingsCount++;
                existingDevice.AverageEfficiency = (existingDevice.AverageEfficiency + efficiencyFactor) / 2;
            }
            else
            {
                results.Add(new DeviceResult 
                { 
                    DeviceId = item.DeviceId, 
                    TotalPower = powerUsage, 
                    ReadingsCount = 1,
                    AverageEfficiency = efficiencyFactor
                });
            }
        }

        return results;
    }
}

public class RawData {
    public string DeviceId { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Temperature { get; set; }
}

public class DeviceResult {
    public string DeviceId { get; set; }
    public double TotalPower { get; set; }
    public int ReadingsCount { get; set; }
    public double AverageEfficiency { get; set; }
}