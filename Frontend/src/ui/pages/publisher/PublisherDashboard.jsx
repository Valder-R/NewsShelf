import React from 'react'
import {useAuth} from '../../../state/AuthContext.jsx'

export default function PublisherDashboard() {
  const { user, roles } = useAuth()

  return (
    <div className="ns-card ns-shadow p-4">
      <h1 className="h4 fw-semibold mb-1">Publisher панель</h1>
      <div className="ns-muted small mb-3">Керування вашими публікаціями</div>

      <div className="ns-muted">
        <div className="mb-2"><i className="bi bi-person me-2"></i>{user?.displayName || user?.email || '—'}</div>
        <div className="mb-2"><i className="bi bi-shield-check me-2"></i>Ролі: {roles.length ? roles.join(', ') : '—'}</div>
        <div className="mt-3">
          Перейдіть у розділ <span className="fw-semibold">"Мої новини"</span>, щоб створювати, редагувати та видаляти власні публікації.
        </div>
      </div>
    </div>
  )
}
