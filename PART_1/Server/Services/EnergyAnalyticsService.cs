using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Aggregates energy readings per device and returns both valid results and skipped readings.
/// </summary>
public class EnergyAnalyticsService
{
    private const double EfficiencyCoefficient = 0.85;
    private const double MinDenominatorThreshold = 1e-6;

    /// <summary>
    /// Groups valid readings by device, calculates totals and average efficiency,
    /// and records invalid readings with the reason they were skipped.
    /// </summary>
    public EfficiencyResult CalculateEfficiencyMetrics(List<RawData> data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var deviceMap = new Dictionary<string, DeviceResult>();
        var skippedReadings = new List<SkippedReading>();

        foreach (var item in data)
        {
            if (item == null)
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = string.Empty,
                    Reason = "Reading is null"
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.DeviceId))
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = item.DeviceId ?? string.Empty,
                    Reason = "DeviceId is null, empty, or whitespace"
                });
                continue;
            }

            if (!double.IsFinite(item.Voltage) || !double.IsFinite(item.Current) || !double.IsFinite(item.Temperature))
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = item.DeviceId,
                    Reason = "One or more numeric fields (Voltage, Current, Temperature) contain NaN or Infinity"
                });
                continue;
            }

            double adjustedTemperature = item.Temperature + 1;

            // Skip unsafe denominators before calculating efficiency.
            if (Math.Abs(adjustedTemperature) <= MinDenominatorThreshold)
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = item.DeviceId,
                    Reason = "adjustedTemperature is near zero (at or below MinDenominatorThreshold), which would cause division by zero or extreme efficiency values"
                });
                continue;
            }

            double powerUsage = item.Voltage * item.Current;
            double efficiencyFactor = (powerUsage * EfficiencyCoefficient) / adjustedTemperature;

            if (deviceMap.TryGetValue(item.DeviceId, out var existingDevice))
            {
                existingDevice.AccumulateReading(powerUsage, efficiencyFactor);
            }
            else
            {
                deviceMap[item.DeviceId] = new DeviceResult(item.DeviceId, powerUsage, efficiencyFactor);
            }
        }

        return new EfficiencyResult
        {
            DeviceResults = deviceMap.Values.ToList(),
            SkippedReadings = skippedReadings
        };
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
    // Kept for serializers that require a parameterless constructor.
    public DeviceResult() { }

    public DeviceResult(string deviceId, double initialPower, double initialEfficiency)
    {
        DeviceId = deviceId;
        TotalPower = initialPower;
        ReadingsCount = 1;
        EfficiencySum = initialEfficiency;
    }

    public string DeviceId { get; set; } = string.Empty;
    public double TotalPower { get; set; }
    public int ReadingsCount { get; set; }

    // Used internally to calculate AverageEfficiency.
    public double EfficiencySum { get; private set; }

    public double AverageEfficiency => ReadingsCount > 0 ? EfficiencySum / ReadingsCount : 0;

    public void AccumulateReading(double power, double efficiency)
    {
        TotalPower += power;
        ReadingsCount++;
        EfficiencySum += efficiency;
    }
}

public class EfficiencyResult
{
    public List<DeviceResult> DeviceResults { get; set; } = new();
    public List<SkippedReading> SkippedReadings { get; set; } = new();
}

public class SkippedReading
{
    public string DeviceId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}