using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Calculates per-device energy efficiency metrics from raw sensor readings.
/// </summary>
/// <remarks>
/// <b>Thread safety:</b> This service holds instance state in <see cref="LastSkippedReadings"/>.
/// It must NOT be registered as a singleton in a DI container. Use transient or scoped lifetime only.
/// Concurrent calls on the same instance will race on <see cref="LastSkippedReadings"/>.
/// </remarks>
public class EnergyAnalyticsService
{
    private const double EfficiencyCoefficient = 0.85;

    // Temperature + 1 values at or below this threshold are treated as near-zero and skipped
    // to prevent division by zero or extreme efficiency values from near-zero denominators.
    private const double MinDenominatorThreshold = 1e-6;

    /// <summary>
    /// Readings skipped during the most recent call to <see cref="CalculateEfficiencyMetrics"/>.
    /// A reading is skipped when it is null, has an invalid DeviceId, contains non-finite numeric
    /// values, or has a denominator (Temperature + 1) at or below <see cref="MinDenominatorThreshold"/>.
    /// Populated and reset on every call; never carries over from a previous call.
    /// </summary>
    public IReadOnlyList<SkippedReading> LastSkippedReadings { get; private set; } = Array.Empty<SkippedReading>();

    /// <summary>
    /// Calculates per-device efficiency metrics from raw energy readings.
    /// Invalid or unsafe readings are skipped and exposed via <see cref="LastSkippedReadings"/> after the call.
    /// </summary>
 
 
    public List<DeviceResult> CalculateEfficiencyMetrics(List<RawData> data)
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

            if (Math.Abs(item.Temperature + 1) <= MinDenominatorThreshold)
            {
                skippedReadings.Add(new SkippedReading
                {
                    DeviceId = item.DeviceId,
                    Reason = "Temperature + 1 is near zero (at or below MinDenominatorThreshold), which would cause division by zero or extreme efficiency values"
                });
                continue;
            }

            double powerUsage = item.Voltage * item.Current;
            double efficiencyFactor = (powerUsage * EfficiencyCoefficient) / (item.Temperature + 1);

            if (deviceMap.TryGetValue(item.DeviceId, out var existingDevice))
            {
                existingDevice.AccumulateReading(powerUsage, efficiencyFactor);
            }
            else
            {
                deviceMap[item.DeviceId] = new DeviceResult(item.DeviceId, powerUsage, efficiencyFactor);
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

public class SkippedReading
{
    public string DeviceId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
