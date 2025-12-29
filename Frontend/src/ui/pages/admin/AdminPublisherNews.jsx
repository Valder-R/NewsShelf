import React from 'react'
import {Link, useLocation, useParams} from 'react-router-dom'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {adminService} from '../../../services/adminService.js'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {EmptyState} from '../../shared/EmptyState.jsx'
import {ConfirmModal} from '../../shared/ConfirmModal.jsx'
import {normalizeNewsImageUrl} from '../../../services/newsService.js'

function formatDate(iso) {
  try {
    return new Date(iso).toLocaleString()
  } catch {
    return ''
  }
}

export default function AdminPublisherNews() {
  const { userId } = useParams()
  const loc = useLocation()
  const { token } = useAuth()
  const publisherEmail = loc.state?.email || ''

  const [loading, setLoading] = React.useState(true)
  const [items, setItems] = React.useState([])
  const [cats, setCats] = React.useState([])
  const [editing, setEditing] = React.useState(null)

  async function load() {
    setLoading(true)
    try {
      let email = publisherEmail
      if (!email) {

        const res = await adminService.listUsers({}, token)
        const u = (res?.items || []).find(x => String(x.id) === String(userId))
        email = u?.email || ''
      }

      const [news, categories] = await Promise.all([
        email ? adminService.newsByAuthor(email, token) : Promise.resolve([]),
        adminService.newsCategories()
      ])

      setItems(Array.isArray(news) ? news : [])
      setCats(categories)
    } catch (e) {
      toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося завантажити новини публікатора')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [userId])

  if (loading) return <LoadingOverlay label="Завантаження..." />

  const email = publisherEmail

  return (
    <>
      <div className="ns-card ns-shadow p-4">
        <div className="d-flex justify-content-between align-items-end mb-3">
          <div>
            <h1 className="h4 fw-semibold mb-1">Новини публікатора</h1>
            <div className="ns-muted small">
              {email ? <>Author = <span className="fw-semibold">{email}</span></> : 'Автор не визначений'}
            </div>
          </div>
          <div className="d-flex gap-2">
            <Link to="/admin/publishers" className="btn btn-outline-light btn-sm">
              <i className="bi bi-arrow-left me-2"></i>Назад
            </Link>
            <button className="btn btn-outline-light btn-sm" onClick={load}>
              <i className="bi bi-arrow-clockwise me-2"></i>Оновити
            </button>
          </div>
        </div>

        {items.length === 0 ? (
          <EmptyState title="Новин немає" hint="У цього публікатора поки немає опублікованих новин." />
        ) : (
          <div className="table-responsive">
            <table className="table table-dark table-hover align-middle">
              <thead>
                <tr>
                  <th style={{ width: 80 }}>ID</th>
                  <th>Заголовок</th>
                  <th style={{ width: 160 }}>Категорія</th>
                  <th style={{ width: 190 }}>Дата</th>
                  <th style={{ width: 220 }}>Дії</th>
                </tr>
              </thead>
              <tbody>
                {items.map(n => (
                  <tr key={n.id}>
                    <td>{n.id}</td>
                    <td>
                      <div className="d-flex gap-2 align-items-center">
                        {Array.isArray(n.imageUrls) && n.imageUrls.length ? (
                          <img
                            src={normalizeNewsImageUrl(n.imageUrls[0])}
                            alt=""
                            style={{ width: 44, height: 44, objectFit: 'cover', borderRadius: 10, border: '1px solid rgba(255,255,255,0.12)' }}
                            loading="lazy"
                          />
                        ) : null}
                        <div className="text-truncate" style={{ maxWidth: 520 }}>{n.title}</div>
                      </div>
                      <div className="ns-muted small text-truncate" style={{ maxWidth: 640 }}>{n.content}</div>
                    </td>
                    <td>{String(n.category ?? '')}</td>
                    <td className="ns-muted small">{formatDate(n.publishedAt)}</td>
                    <td>
                      <div className="d-flex gap-2">
                        <button
                          className="btn btn-outline-light btn-sm"
                          onClick={() => setEditing(n)}
                          title="Редагувати (в цьому меню дозволено)"
                        >
                          <i className="bi bi-pencil"></i>
                        </button>

                        <button
                          className="btn btn-outline-danger btn-sm"
                          data-bs-toggle="modal"
                          data-bs-target={`#del-post-${n.id}`}
                          title="Видалити"
                        >
                          <i className="bi bi-trash"></i>
                        </button>

                        <ConfirmModal
                          id={`del-post-${n.id}`}
                          title="Видалити новину?"
                          body={<div>Видалити <span className="fw-semibold">{n.title}</span>?</div>}
                          confirmText="Видалити"
                          onConfirm={async () => {
                            try {

                              await adminService.deletePost(String(n.id), token)
                              toast.success('Видалено')
                              load()
                            } catch (e) {

                              try {
                                await adminService.newsDelete(n.id, token)
                                toast.success('Видалено')
                                load()
                              } catch {
                                toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося видалити')
                              }
                            }
                          }}
                        />
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {editing && (
        <EditNewsModal
          value={editing}
          categories={cats}
          token={token}
          onClose={() => setEditing(null)}
          onSaved={() => {
            setEditing(null)
            load()
          }}
        />
      )}
    </>
  )
}

function EditNewsModal({ value, onClose, onSaved, token, categories }) {
  const [model, setModel] = React.useState(() => ({
    id: value?.id,
    title: value?.title || '',
    content: value?.content || '',
    author: value?.author || '',
    category: value?.category ?? 0,
    imageUrls: Array.isArray(value?.imageUrls) ? value.imageUrls : []
  }))

  return (
    <div className="modal show" style={{ display: 'block', background: 'rgba(0,0,0,0.6)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content ns-card">
          <div className="modal-header">
            <h5 className="modal-title">Редагування новини</h5>
            <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <div className="mb-2">
              <label className="form-label">Заголовок</label>
              <input className="form-control ns-input" value={model.title} onChange={e => setModel({ ...model, title: e.target.value })} />
            </div>
            <div className="mb-2">
              <label className="form-label">Контент</label>
              <textarea className="form-control ns-input" rows={8} value={model.content} onChange={e => setModel({ ...model, content: e.target.value })} />
            </div>
            <div className="row g-2">
              <div className="col-12 col-md-6">
                <label className="form-label">Категорія</label>
                <select
                  className="form-select ns-input"
                  value={String(model.category)}
                  onChange={e => setModel({ ...model, category: Number(e.target.value) })}
                >
                  {(categories || []).map(c => (
                    <option key={c.value} value={c.value}>{c.name}</option>
                  ))}
                </select>
              </div>
              <div className="col-12 col-md-6">
                <label className="form-label">Автор</label>
                <input className="form-control ns-input" value={model.author} onChange={e => setModel({ ...model, author: e.target.value })} />
              </div>
            </div>
            <div className="mt-2">
              <label className="form-label">ImageUrls (JSON-масив рядків)</label>
              <textarea
                className="form-control ns-input"
                rows={3}
                value={JSON.stringify(model.imageUrls || [], null, 2)}
                onChange={(e) => {
                  try {
                    const parsed = JSON.parse(e.target.value)
                    if (Array.isArray(parsed)) setModel({ ...model, imageUrls: parsed })
                  } catch {

                  }
                }}
              />
            </div>
          </div>
          <div className="modal-footer">
            <button className="btn btn-outline-light" onClick={onClose}>Скасувати</button>
            <button
              className="btn btn-light"
              onClick={async () => {
                try {
                  await adminService.newsUpdate(model.id, model, token)
                  toast.success('Збережено')
                  onSaved()
                } catch (e) {
                  toast.error(e?.response?.data?.message || e?.data?.message || 'Не вдалося зберегти')
                }
              }}
            >
              <i className="bi bi-save me-2"></i>Зберегти
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
