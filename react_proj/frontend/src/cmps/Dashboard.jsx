import { useMemo } from 'react'
import { AgGridReact } from 'ag-grid-react'
import { AllCommunityModule, ModuleRegistry, themeQuartz } from 'ag-grid-community'
import exceptionalDevices from '../data/exceptionalDevices'

ModuleRegistry.registerModules([AllCommunityModule])

const theme = themeQuartz

function formatTimestamp(value) {
  if (!value) return ''
  const date = new Date(value)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`
}

function Dashboard() {
  const columnDefs = useMemo(() => [
    { field: 'id', headerName: 'ID', width: 100 },
    { field: 'device_id', headerName: 'Device ID', width: 140 },
    {
      field: 'timestamp',
      headerName: 'Timestamp',
      flex: 1,
      minWidth: 180,
      valueFormatter: (params) => formatTimestamp(params.value)
    },
    { field: 'voltage', headerName: 'Voltage', width: 120 },
    { field: 'current', headerName: 'Current', width: 120 },
    { field: 'temperature', headerName: 'Temperature', width: 140 },
    { field: 'status', headerName: 'Status', width: 120 }
  ], [])

  const defaultColDef = useMemo(() => ({
    sortable: true,
    resizable: true
  }), [])

  if (!exceptionalDevices || exceptionalDevices.length === 0) {
    return (
      <section className="dashboard">
        <div className="dashboard-header">
          <h1>Dashboard</h1>
        </div>
        <div className="dashboard-card">
          <div className="dashboard-empty">
            No exceptional devices found
          </div>
        </div>
      </section>
    )
  }

  return (
    <section className="dashboard">
      <div className="dashboard-header">
        <h1>Dashboard</h1>
      </div>
      <div className="dashboard-card">
        <div className="dashboard-card-header">
          <h2>Exceptional Devices</h2>
          <span className="device-count">{exceptionalDevices.length} devices</span>
        </div>
        <div className="dashboard-grid-wrapper">
          <AgGridReact
            theme={theme}
            rowData={exceptionalDevices}
            columnDefs={columnDefs}
            defaultColDef={defaultColDef}
            overlayNoRowsTemplate="No exceptional devices found"
          />
        </div>
      </div>
    </section>
  )
}

export default Dashboard
