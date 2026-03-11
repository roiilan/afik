using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Calculates per-device energy efficiency metrics from raw sensor readings.
/// </summary>
/// <remarks>
/// This service is stateless — all results (including skipped readings) are returned
/// in the <see cref="EfficiencyResult"/> wrapper. Safe for any DI lifetime (singleton, scoped, transient).
/// </remarks>
public class EnergyAnalyticsService
{
    private const double EfficiencyCoefficient = 0.85;

    // adjustedTemperature values at or below this threshold are treated as near-zero and skipped
    // to prevent division by zero or extreme efficiency values from near-zero denominators.
    private const double MinDenominatorThreshold = 1e-6;

    /// <summary>
    /// Calculates per-device efficiency metrics from raw energy readings.
    /// Invalid or unsafe readings are skipped and included in <see cref="EfficiencyResult.SkippedReadings"/>.
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
    // Parameterless constructor preserved for serialization compatibility.
    public DeviceResult() { }

    /// <summary>Creates a new result for a device with its first reading.</summary>
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

    /// <summary>
    /// Internal accumulator used to compute <see cref="AverageEfficiency"/>.
    /// Do not write to this property. It is exposed as read-only for diagnostic purposes only.
    /// </summary>
    public double EfficiencySum { get; private set; }

    public double AverageEfficiency => ReadingsCount > 0 ? EfficiencySum / ReadingsCount : 0;

    /// <summary>Adds a reading to this device's accumulated totals.</summary>
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
