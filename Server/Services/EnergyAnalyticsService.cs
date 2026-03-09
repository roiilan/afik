using System;
using System.Collections.Generic;
using System.Linq;

public class EnergyAnalyticsService
{
    private const double EfficiencyCoefficient = 0.85;

    /// <summary>
    /// Readings skipped during the most recent call to <see cref="CalculateEfficiencyMetrics"/>.
    /// A reading is skipped when Temperature + 1 == 0 (division by zero).
    /// Populated after every call; empty when no readings were skipped.
    /// </summary>
    public IReadOnlyList<SkippedReading> LastSkippedReadings { get; private set; } = Array.Empty<SkippedReading>();

    /// <summary>
    /// Calculates per-device efficiency metrics from raw energy readings.
    /// Readings that would cause division by zero (Temperature + 1 == 0) are silently skipped
    /// and exposed via <see cref="LastSkippedReadings"/> after the call.
    /// </summary>
    public List<DeviceResult> CalculateEfficiencyMetrics(List<RawData> data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var deviceMap = new Dictionary<string, DeviceResult>();
        var skippedReadings = new List<SkippedReading>();

        foreach (var item in data)
        {
            if (item.Temperature + 1 == 0)
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = item.DeviceId,
                    Reason = "Temperature + 1 equals zero, which would cause division by zero"
                });
                continue;
            }

            double powerUsage = item.Voltage * item.Current;
            double efficiencyFactor = (powerUsage * EfficiencyCoefficient) / (item.Temperature + 1);

            if (deviceMap.TryGetValue(item.DeviceId, out var existingDevice))
            {
                existingDevice.TotalPower += powerUsage;
                existingDevice.ReadingsCount++;
                existingDevice.EfficiencySum += efficiencyFactor;
            }
            else
            {
                deviceMap[item.DeviceId] = new DeviceResult
                {
                    DeviceId = item.DeviceId,
                    TotalPower = powerUsage,
                    ReadingsCount = 1,
                    EfficiencySum = efficiencyFactor
                };
            }
        }

        LastSkippedReadings = skippedReadings;
        return deviceMap.Values.ToList();
    }
}

public class RawData
{
    public string DeviceId { get; set; } = string.Empty;
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Temperature { get; set; }
}

public class DeviceResult
{
    public string DeviceId { get; set; } = string.Empty;
    public double TotalPower { get; set; }
    public int ReadingsCount { get; set; }
    public double EfficiencySum { get; set; }
    public double AverageEfficiency => ReadingsCount > 0 ? EfficiencySum / ReadingsCount : 0;
}

public class SkippedReading
{
    public string DeviceId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
