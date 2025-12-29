import React from 'react'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {resolveBaseUrl} from '../../../lib/api/http.js'

async function ping(url, opts = {}) {
  try {
    const res = await fetch(url, { ...opts })
    return { ok: true, status: res.status }
  } catch (e) {
    return { ok: false, status: null, error: e?.message || 'network error' }
  }
}

export default function AdminSystemStatus() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [rows, setRows] = React.useState([])

  async function load() {
    setLoading(true)
    try {
      const userBase = resolveBaseUrl('user')
      const searchBase = resolveBaseUrl('search')
      const recommendationBase = resolveBaseUrl('recommendations')
      const adminBase = resolveBaseUrl('admin')

      const checks = [
        {
          name: 'Gateway → UserService',
          url: `${userBase}/profiles/me`,
          init: token ? { headers: { Authorization: `Bearer ${token}` } } : {}
        },
        {
          name: 'Gateway → SearchService',
          url: `${searchBase}/News`,
          init: {}
        },
        {
          name: 'Gateway → RecService',
          url: `${recommendationBase}/popular/news`,
          init: {}
        },
        {
          name: 'Gateway → AdminService',
          url: `${adminBase}/users`,
          init: token ? { headers: { Authorization: `Bearer ${token}` } } : {}
        }
      ]

      const results = await Promise.all(checks.map(async c => ({
        ...c,
        ...(await ping(c.url, c.init))
      })))

      setRows(results)
    } catch (e) {
      toast.error('Не вдалося перевірити стан системи')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [])

  if (loading) return <LoadingOverlay label="Перевірка сервісів..." />

  return (
    <div className="ns-card ns-shadow p-4">
      <div className="d-flex justify-content-between align-items-end mb-3">
        <div>
          <h1 className="h4 fw-semibold mb-1">Стан системи</h1>
          <div className="ns-muted small">Перевірка доступності сервісів через gateway (враховує 401/403 як "up").</div>
        </div>
        <button className="btn btn-outline-light btn-sm" onClick={load}>
          <i className="bi bi-arrow-clockwise me-2"></i>Оновити
        </button>
      </div>

      <div className="table-responsive">
        <table className="table table-dark table-hover align-middle">
          <thead>
            <tr>
              <th>Сервіс</th>
              <th>Endpoint</th>
              <th style={{ width: 160 }}>Статус</th>
            </tr>
          </thead>
          <tbody>
            {rows.map(r => {
              const up = r.ok
              const code = r.status
              const consideredUp = up && (code >= 200 && code < 600)
              return (
                <tr key={r.name}>
                  <td>{r.name}</td>
                  <td className="ns-muted small text-truncate" style={{ maxWidth: 520 }}>{r.url}</td>
                  <td>
                    <span className={"badge rounded-pill " + (consideredUp ? 'bg-success' : 'bg-danger')}>
                      {consideredUp ? `UP (${code})` : `DOWN`}
                    </span>
                    {!consideredUp && r.error ? (
                      <div className="ns-muted small mt-1">{r.error}</div>
                    ) : null}
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    </div>
  )
}
