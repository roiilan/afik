import { Router } from 'express'
import { generateInsight } from '../services/generateInsight.js'

const router = Router()

const REQUIRED_FIELDS = ['voltage', 'current', 'temperature', 'status']
const NUMERIC_FIELDS = ['voltage', 'current', 'temperature']

router.get('/', (req, res) => {
  const missingFields = REQUIRED_FIELDS.filter(field => !req.query[field])

  if (missingFields.length > 0) {
    return res.status(400).json({ error: `Missing required fields: ${missingFields.join(', ')}` })
  }

  const device = {}
  for (const field of REQUIRED_FIELDS) {
    device[field] = NUMERIC_FIELDS.includes(field)
      ? Number(req.query[field])
      : req.query[field]
  }

  const insight = generateInsight(device)
  res.json({ insight })
})

export default router
