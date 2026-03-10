const BASE_URL = 'http://localhost:3000/api/ai-insight'

export async function fetchAiInsight(device) {
  const params = new URLSearchParams({
    id: device.id,
    device_id: device.device_id,
    timestamp: device.timestamp,
    voltage: device.voltage,
    current: device.current,
    temperature: device.temperature,
    status: device.status
  })

  const response = await fetch(`${BASE_URL}?${params}`)

  if (!response.ok) {
    throw new Error(`Server responded with status ${response.status}`)
  }

  const data = await response.json()
  return data.insight
}
