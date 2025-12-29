import React from 'react'
import {useAuth} from '../../../state/AuthContext.jsx'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {adminService} from '../../../services/adminService.js'
import {toast} from 'react-toastify'

export default function AdminDashboard() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [stats, setStats] = React.useState({ users: 0, publishers: 0, news: 0 })

  async function load() {
    setLoading(true)
    try {
      const [uAll, uPub, news] = await Promise.all([
        adminService.listUsers({}, token),
        adminService.listUsers({ role: 'PUBLISHER' }, token),
        adminService.newsList(token)
      ])
      setStats({
        users: Number(uAll?.total || (uAll?.items || []).length || 0),
        publishers: Number(uPub?.total || (uPub?.items || []).length || 0),
        news: Array.isArray(news) ? news.length : 0
      })
    } catch (e) {
      toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося завантажити статистику')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [])

  if (loading) return <LoadingOverlay label="Завантаження огляду..." />

  return (
    <div className="ns-card ns-shadow p-4">
      <div className="d-flex justify-content-between align-items-end mb-3">
        <div>
          <h1 className="h4 fw-semibold mb-1">Огляд</h1>
          <div className="ns-muted small">Швидка статистика по системі (через AdminService + SearchService).</div>
        </div>
        <button className="btn btn-outline-light btn-sm" onClick={load}>
          <i className="bi bi-arrow-clockwise me-2"></i>Оновити
        </button>
      </div>

      <div className="row g-3">
        <div className="col-12 col-md-4">
          <div className="ns-card p-3">
            <div className="ns-muted small">Користувачі</div>
            <div className="display-6 fw-semibold">{stats.users}</div>
          </div>
        </div>
        <div className="col-12 col-md-4">
          <div className="ns-card p-3">
            <div className="ns-muted small">Публікатори</div>
            <div className="display-6 fw-semibold">{stats.publishers}</div>
          </div>
        </div>
        <div className="col-12 col-md-4">
          <div className="ns-card p-3">
            <div className="ns-muted small">Новини</div>
            <div className="display-6 fw-semibold">{stats.news}</div>
          </div>
        </div>
      </div>
    </div>
  )
}
