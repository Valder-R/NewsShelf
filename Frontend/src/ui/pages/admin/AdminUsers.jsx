import React from 'react'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {adminService} from '../../../services/adminService.js'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {EmptyState} from '../../shared/EmptyState.jsx'
import {ConfirmModal} from '../../shared/ConfirmModal.jsx'

function sortUsers(items, sortKey, sortDir) {
  const arr = Array.isArray(items) ? [...items] : []
  const dir = sortDir === 'desc' ? -1 : 1
  const norm = (v) => String(v || '').toLowerCase()
  arr.sort((a, b) => {
    const av = sortKey === 'role'
      ? norm(a.role)
      : sortKey === 'status'
        ? norm(a.status)
        : norm(a.email)
    const bv = sortKey === 'role'
      ? norm(b.role)
      : sortKey === 'status'
        ? norm(b.status)
        : norm(b.email)
    if (av < bv) return -1 * dir
    if (av > bv) return 1 * dir
    return 0
  })
  return arr
}

export default function AdminUsers() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [items, setItems] = React.useState([])
  const [total, setTotal] = React.useState(0)

  const [role, setRole] = React.useState('')
  const [status, setStatus] = React.useState('')
  const [q, setQ] = React.useState('')
  const [sortKey, setSortKey] = React.useState('email')
  const [sortDir, setSortDir] = React.useState('asc')

  async function load() {
    setLoading(true)
    try {
      const res = await adminService.listUsers({ role, status }, token)
      const raw = res?.items || []
      const filtered = q.trim()
        ? raw.filter(u => String(u.email || '').toLowerCase().includes(q.trim().toLowerCase()))
        : raw
      const sorted = sortUsers(filtered, sortKey, sortDir)
      setItems(sorted)
      setTotal(res?.total ?? sorted.length)
    } catch (e) {
      toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося завантажити користувачів')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [role, status, sortKey, sortDir])

  if (loading) return <LoadingOverlay label="Завантаження користувачів..." />

  return (
    <div className="ns-card ns-shadow p-4">
      <div className="d-flex justify-content-between align-items-end mb-3">
        <div>
          <h1 className="h4 fw-semibold mb-1">Користувачі</h1>
          <div className="ns-muted small">/api/admin/users + assignRole + deleteUser</div>
        </div>
        <button className="btn btn-outline-light btn-sm" onClick={load}>
          <i className="bi bi-arrow-clockwise me-2"></i>Оновити
        </button>
      </div>

      <div className="row g-2 mb-3">
        <div className="col-12 col-md-4">
          <label className="form-label">Пошук (email)</label>
          <input
            className="form-control ns-input"
            value={q}
            onChange={e => setQ(e.target.value)}
            onKeyDown={(e) => { if (e.key === 'Enter') load() }}
            placeholder="user@example.com"
          />
        </div>
        <div className="col-6 col-md-2">
          <label className="form-label">Role</label>
          <select className="form-select ns-input" value={role} onChange={e => setRole(e.target.value)}>
            <option value="">Усі</option>
            <option value="PUBLISHER">PUBLISHER</option>
            <option value="ADMIN">ADMIN</option>
          </select>
        </div>
        <div className="col-6 col-md-2">
          <label className="form-label">Status</label>
          <select className="form-select ns-input" value={status} onChange={e => setStatus(e.target.value)}>
            <option value="">Усі</option>
            <option value="ACTIVE">ACTIVE</option>
            <option value="BLOCKED">BLOCKED</option>
          </select>
        </div>
        <div className="col-6 col-md-2">
          <label className="form-label">Сортування</label>
          <select className="form-select ns-input" value={sortKey} onChange={e => setSortKey(e.target.value)}>
            <option value="email">Email</option>
            <option value="role">Role</option>
            <option value="status">Status</option>
          </select>
        </div>
        <div className="col-6 col-md-2">
          <label className="form-label">Напрям</label>
          <select className="form-select ns-input" value={sortDir} onChange={e => setSortDir(e.target.value)}>
            <option value="asc">ASC</option>
            <option value="desc">DESC</option>
          </select>
        </div>
      </div>

      {items.length === 0 ? (
        <EmptyState title="Немає користувачів" hint="Спробуйте змінити фільтри." />
      ) : (
        <>
          <div className="ns-muted small mb-2">Показано: {items.length} / {total}</div>
          <div className="table-responsive">
            <table className="table table-dark table-hover align-middle">
              <thead>
              <tr>
                <th style={{ width: 80 }}>ID</th>
                <th>Email</th>
                <th style={{ width: 140 }}>Role</th>
                <th style={{ width: 140 }}>Status</th>
                <th style={{ width: 240 }}>Дії</th>
              </tr>
              </thead>
              <tbody>
              {items.map(u => (
                <UserRow key={u.id} u={u} token={token} onChanged={load} />
              ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}

function UserRow({ u, token, onChanged }) {
  const [role, setRole] = React.useState(String(u?.role || ''))
  React.useEffect(() => setRole(String(u?.role || '')), [u?.role])

  return (
    <tr>
      <td className="text-truncate" style={{ maxWidth: 80 }}>{String(u.id || '').slice(0, 6)}</td>
      <td className="text-truncate" style={{ maxWidth: 420 }}>{u.email || '—'}</td>
      <td>
        <span className="badge rounded-pill ns-badge">{u.role || '—'}</span>
      </td>
      <td>
        <span className="badge rounded-pill ns-badge">{u.status || '—'}</span>
      </td>
      <td>
        <div className="d-flex gap-2">
          <select
            className="form-select form-select-sm ns-input"
            value={role}
            onChange={e => setRole(e.target.value)}
            style={{ maxWidth: 150 }}
          >
            <option value={String(u.role || '')} disabled>{String(u.role || 'Current')}</option>
            <option value="PUBLISHER">PUBLISHER</option>
            <option value="ADMIN">ADMIN</option>
          </select>
          <button
            className="btn btn-outline-light btn-sm"
            onClick={async () => {
              try {
                if (!role || role === u.role) return
                await adminService.assignRole(u.id, role, token)
                toast.success('Роль змінено')
                onChanged()
              } catch (e) {
                toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося змінити роль')
              }
            }}
            title="Змінити роль"
          >
            <i className="bi bi-check2"></i>
          </button>

          <button
            className="btn btn-outline-danger btn-sm"
            data-bs-toggle="modal"
            data-bs-target={`#del-user-${u.id}`}
            title="Видалити користувача"
          >
            <i className="bi bi-trash"></i>
          </button>

          <ConfirmModal
            id={`del-user-${u.id}`}
            title="Видалити користувача?"
            body={<div>Ви впевнені, що хочете видалити <span className="fw-semibold">{u.email || u.id}</span>?</div>}
            confirmText="Видалити"
            onConfirm={async () => {
              try {
                await adminService.deleteUser(u.id, token)
                toast.success('Видалено')
                onChanged()
              } catch (e) {
                toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося видалити')
              }
            }}
          />
        </div>
      </td>
    </tr>
  )
}
