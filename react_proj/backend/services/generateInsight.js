const CRITICAL_TEMP = 85
const ELEVATED_TEMP = 60
const HIGH_CURRENT = 32
const MODERATE_CURRENT = 16
const LOW_CURRENT = 6
const LOW_VOLTAGE = 200
const MAX_NORMAL_VOLTAGE = 250

export function generateInsight(device) {
  const { temperature, current, voltage, status } = device
  const normalizedStatus = String(status || '').toLowerCase().trim()

  switch (true) {
    // 1. Most severe: critical temperature + high current
    case temperature >= CRITICAL_TEMP && current > HIGH_CURRENT:
      return 'High temperature paired with high current may indicate cooling system failure or overheating under heavy load.'

    // 2. Critical temperature + at least moderate current
    case temperature >= CRITICAL_TEMP && current >= MODERATE_CURRENT:
      return 'Critical temperature with sustained charging current may indicate serious thermal stress or degraded cooling performance.'

    // 3. Critical status + high current
    case normalizedStatus === 'critical' && current > HIGH_CURRENT:
      return 'Critical status with high current indicates severe overload or unstable operation requiring immediate attention.'

    // 4. Critical status + low voltage
    case normalizedStatus === 'critical' && voltage < LOW_VOLTAGE:
      return 'Critical status combined with low voltage may indicate unstable power delivery or a stressed charging circuit.'

    // 5. Critical status alone
    case normalizedStatus === 'critical':
      return 'Critical device status indicates a serious fault condition that should be inspected immediately.'

    // 6. Low voltage + high current
    case voltage < LOW_VOLTAGE && current > HIGH_CURRENT:
      return 'Low voltage combined with high current suggests inefficient power delivery or electrical stress on the circuit.'

    // 7. Low voltage + moderate current
    case voltage < LOW_VOLTAGE && current >= MODERATE_CURRENT:
      return 'Low voltage under active charging load may indicate supply weakness, cable loss, or electrical stress in the charging path.'

    // 8. Elevated temperature + warning status
    case temperature >= ELEVATED_TEMP &&
         temperature < CRITICAL_TEMP &&
         normalizedStatus === 'warning':
      return 'Elevated temperature under warning status suggests poor ventilation or an early-stage thermal problem.'

    // 9. High current + normal voltage + elevated temperature
    case current > HIGH_CURRENT &&
         voltage >= LOW_VOLTAGE &&
         voltage <= MAX_NORMAL_VOLTAGE &&
         temperature >= ELEVATED_TEMP &&
         temperature < CRITICAL_TEMP:
      return 'High current draw with normal voltage and elevated temperature may indicate abnormal downstream load behavior or power imbalance.'

    // 10. Moderate current + elevated temperature
    case current >= MODERATE_CURRENT &&
         current <= HIGH_CURRENT &&
         temperature >= ELEVATED_TEMP &&
         temperature < CRITICAL_TEMP:
      return 'Sustained moderate charging current together with elevated temperature may indicate gradual overheating or reduced thermal efficiency.'

    // 11. High current + normal voltage
    case current > HIGH_CURRENT &&
         voltage >= LOW_VOLTAGE &&
         voltage <= MAX_NORMAL_VOLTAGE:
      return 'High charging current under normal voltage may indicate heavy load conditions or abnormal downstream demand.'

    // 12. Low current + warning status
    case current < LOW_CURRENT && normalizedStatus === 'warning':
      return 'Low charging current together with warning status may indicate unstable charging, weak connection, or restricted power delivery.'

    // 13. Low current + critical status
    case current < LOW_CURRENT && normalizedStatus === 'critical':
      return 'Very low charging current under critical status may indicate interrupted charging or a serious delivery fault.'

    // 14. Elevated temperature alone
    case temperature >= ELEVATED_TEMP && temperature < CRITICAL_TEMP:
      return 'Elevated temperature may indicate developing thermal stress, insufficient ventilation, or prolonged charging load.'

    // 15. Low voltage alone
    case voltage < LOW_VOLTAGE:
      return 'Low voltage may indicate unstable input supply or excessive voltage drop during charging.'

    // 16. Moderate current + warning status
    case current >= MODERATE_CURRENT && normalizedStatus === 'warning':
      return 'Warning status during active charging current may indicate unstable operating conditions that should be monitored.'

    default:
      return 'AI could not determine the root cause of the anomaly.'
  }
}