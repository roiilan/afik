const CRITICAL_TEMP = 85
const ELEVATED_TEMP = 60
const HIGH_CURRENT = 32
const MODERATE_CURRENT = 16
const LOW_CURRENT = 6
const LOW_VOLTAGE = 200
const MAX_NORMAL_VOLTAGE = 250

export function generateInsight(device) {
  const { temperature, current, voltage, status } = device

  switch (true) {
    case temperature >= CRITICAL_TEMP && current > HIGH_CURRENT:
      return 'High temperature paired with high current may indicate cooling system failure or overheating under heavy load.'

    case status === 'critical' && current > HIGH_CURRENT:
      return 'Critical status with high current indicates severe overload or unstable operation requiring immediate attention.'

    case voltage < LOW_VOLTAGE && current > HIGH_CURRENT:
      return 'Low voltage combined with high current suggests inefficient power delivery or electrical stress on the circuit.'

    case temperature >= CRITICAL_TEMP && status === 'warning':
      return 'Elevated temperature under warning status suggests poor ventilation or an early-stage thermal problem.'

    case current > HIGH_CURRENT && voltage >= LOW_VOLTAGE && voltage <= MAX_NORMAL_VOLTAGE && temperature >= ELEVATED_TEMP && temperature < CRITICAL_TEMP:
      return 'High current draw with normal voltage and elevated temperature may indicate abnormal downstream load behavior or power imbalance.'

    default:
      return 'AI could not determine the root cause of the anomaly.'
  }
}
