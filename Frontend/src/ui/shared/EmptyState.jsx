export function EmptyState({ title = 'Нічого не знайдено', hint }) {
  return (
    <div className="ns-card ns-shadow p-4 text-center">
      <div className="display-6 mb-2"><i className="bi bi-inbox"></i></div>
      <div className="fw-semibold">{title}</div>
      {hint && <div className="ns-muted mt-2">{hint}</div>}
    </div>
  )
}
