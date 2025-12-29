import React from 'react'
import {useAuth} from '../../../state/AuthContext.jsx'
import {toast} from 'react-toastify'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {EmptyState} from '../../shared/EmptyState.jsx'
import {adminService} from '../../../services/adminService.js'

export default function AdminModeration() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [items, setItems] = React.useState([])

  async function load() {
    setLoading(true)
    try { setItems(await adminService.moderationComments(token)) }
    finally { setLoading(false) }
  }

  React.useEffect(() => { load() }, [])

  if (loading) return <LoadingOverlay label="Завантаження модерації..." />
  if (items.length === 0) return <EmptyState title="Немає коментарів для модерації" hint="Якщо бекенд не підтримує модерацію — відключіть цей пункт меню." />

  return (
    <div className="ns-card ns-shadow p-4">
      <div className="d-flex justify-content-between align-items-end mb-3">
        <div>
          <h1 className="h4 fw-semibold mb-1">Модерація коментарів</h1>
          <div className="ns-muted small">Approve/Reject для коментарів у статусі pending.</div>
        </div>
        <button className="btn btn-outline-light btn-sm" onClick={load}>
          <i className="bi bi-arrow-clockwise me-2"></i>Оновити
        </button>
      </div>

      <div className="d-flex flex-column gap-3">
        {items.map(c => (
          <div key={c.id} className="ns-card p-3">
            <div className="d-flex justify-content-between align-items-start gap-3">
              <div>
                <div className="fw-semibold">{c.user?.name || c.user?.email || 'User'}</div>
                <div className="ns-muted small">News: {c.newsId || c.news?.id || '—'} · {c.createdAt ? new Date(c.createdAt).toLocaleString() : ''}</div>
              </div>
              <div className="d-flex gap-2">
                <button className="btn btn-light btn-sm" onClick={async () => {
                  try {
                    await adminService.moderationApprove(c.id, token)
                    setItems(prev => prev.filter(x => x.id !== c.id))
                    toast.success('Approved')
                  } catch (e) {
                    toast.error(e?.data?.message || 'Не вдалося')
                  }
                }}>
                  <i className="bi bi-check2 me-1"></i>Approve
                </button>
                <button className="btn btn-outline-danger btn-sm" onClick={async () => {
                  try {
                    await adminService.moderationReject(c.id, token)
                    setItems(prev => prev.filter(x => x.id !== c.id))
                    toast.info('Rejected')
                  } catch (e) {
                    toast.error(e?.data?.message || 'Не вдалося')
                  }
                }}>
                  <i className="bi bi-x-lg me-1"></i>Reject
                </button>
              </div>
            </div>
            <div className="mt-2">{c.text}</div>
          </div>
        ))}
      </div>
    </div>
  )
}
