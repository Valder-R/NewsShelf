export function LoadingOverlay({ label = 'Завантаження...' }) {
  return (
    <div className="ns-card ns-shadow p-4 text-center">
      <div className="spinner-border" role="status" aria-label="loading"></div>
      <div className="mt-3 ns-muted">{label}</div>
    </div>
  )
}
