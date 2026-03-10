import { useState } from 'react'

// Hourglass SVG icon for loading state
function HourglassSpinner() {
  return (
    <svg
      className="ai-insight-spinner"
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M5 4h14M5 20h14M7 4v2a5 5 0 0 0 2.5 4.33L12 12l-2.5 1.67A5 5 0 0 0 7 18v2M17 4v2a5 5 0 0 1-2.5 4.33L12 12l2.5 1.67A5 5 0 0 1 17 18v2"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

function AiInsightCell() {
  const [status, setStatus] = useState('idle')

  function onGenerateClick() {
    setStatus('loading')
  }

  if (status === 'loading') {
    return (
      <div className="ai-insight-cell">
        <HourglassSpinner />
      </div>
    )
  }

  return (
    <div className="ai-insight-cell">
      <button className="btn btn-primary ai-insight-btn" onClick={onGenerateClick}>
        Generate AI Insights
      </button>
    </div>
  )
}

export default AiInsightCell
