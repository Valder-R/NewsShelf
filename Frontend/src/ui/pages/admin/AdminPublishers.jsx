import React from 'react'
import {Link} from 'react-router-dom'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {adminService} from '../../../services/adminService.js'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {EmptyState} from '../../shared/EmptyState.jsx'

export default function AdminPublishers() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [items, setItems] = React.useState([])
  const [q, setQ] = React.useState('')

  async function load() {
    setLoading(true)
    try {
      const res = await adminService.listUsers({ role: 'PUBLISHER' }, token)
      const raw = res?.items || []
      const filtered = q.trim()
        ? raw.filter(u => String(u.email || '').toLowerCase().includes(q.trim().toLowerCase()))
        : raw
      setItems(filtered)
    } catch (e) {
      toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося завантажити публікаторів')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [])

  if (loading) return <LoadingOverlay label="Завантаження публікаторів..." />

  return (
    <div className="ns-card ns-shadow p-4">
      <div className="d-flex justify-content-between align-items-end mb-3">
        <div>
          <h1 className="h4 fw-semibold mb-1">Публікатори</h1>
          <div className="ns-muted small">Перегляд та керування новинами конкретного публікатора.</div>
        </div>
        <button className="btn btn-outline-light btn-sm" onClick={load}>
          <i className="bi bi-arrow-clockwise me-2"></i>Оновити
        </button>
      </div>

      <div className="row g-2 mb-3">
        <div className="col-12 col-md-6">
          <label className="form-label">Пошук (email)</label>
          <input
            className="form-control ns-input"
            value={q}
            onChange={e => setQ(e.target.value)}
            onKeyDown={(e) => { if (e.key === 'Enter') load() }}
            placeholder="publisher@example.com"
          />
        </div>
      </div>

      {items.length === 0 ? (
        <EmptyState title="Публікатори не знайдені" hint="Спробуйте інший запит або оновіть список." />
      ) : (
        <div className="table-responsive">
          <table className="table table-dark table-hover align-middle">
            <thead>
              <tr>
                <th style={{ width: 80 }}>ID</th>
                <th>Email</th>
                <th style={{ width: 140 }}>Status</th>
                <th style={{ width: 140 }}>Дії</th>
              </tr>
            </thead>
            <tbody>
              {items.map(u => (
                <tr key={u.id}>
                  <td className="text-truncate" style={{ maxWidth: 80 }}>{String(u.id || '').slice(0, 6)}</td>
                  <td className="text-truncate" style={{ maxWidth: 520 }}>{u.email || '—'}</td>
                  <td><span className="badge rounded-pill ns-badge">{u.status || '—'}</span></td>
                  <td>
                    <Link
                      className="btn btn-outline-light btn-sm"
                      to={`/admin/publishers/${encodeURIComponent(u.id)}`}
                      state={{ email: u.email, status: u.status, role: u.role }}
                    >
                      Новини <i className="bi bi-chevron-right ms-1"></i>
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
