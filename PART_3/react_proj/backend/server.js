import express from 'express'
import cors from 'cors'
import aiInsightsRouter from './routes/aiInsights.js'

const app = express()
const PORT = 3000

// Middleware
app.use(cors())
app.use(express.json())

// Health check route
app.get('/api/health', (req, res) => {
  res.json({ status: 'ok' })
})

// Routes
app.use('/api/ai-insight', aiInsightsRouter)

app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`)
})
