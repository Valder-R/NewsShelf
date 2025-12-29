import React from 'react'
import {useAuth} from '../../../state/AuthContext.jsx'
import {toast} from 'react-toastify'
import {LoadingOverlay} from '../../shared/LoadingOverlay.jsx'
import {EmptyState} from '../../shared/EmptyState.jsx'
import {adminService} from '../../../services/adminService.js'
import {ConfirmModal} from '../../shared/ConfirmModal.jsx'
import {createPortal} from 'react-dom'

export default function AdminNews() {
  const { token } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [items, setItems] = React.useState([])
  const [editing, setEditing] = React.useState(null)

  const [cats, setCats] = React.useState([])
  const catsByValue = React.useMemo(() => {
    const m = new Map()
    for (const c of cats) m.set(c.value, c.name)
    return m
  }, [cats])

  async function load() {
    setLoading(true)
    try {
      const [news, categories] = await Promise.all([
        adminService.newsList(token),
        adminService.newsCategories()
      ])
      setItems(news)
      setCats(categories)
    } catch (e) {
      toast.error(e?.data?.message || 'Не вдалося завантажити дані')
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { load() }, [])

  if (loading) return <LoadingOverlay label="Завантаження новин..." />

  return (
      <div className="ns-card ns-shadow p-4">
        <div className="d-flex justify-content-between align-items-end mb-3">
          <div>
            <h1 className="h4 fw-semibold mb-1">Новини</h1>
            <div className="ns-muted small">POST /api/News (multipart/form-data) + GET /api/News/categories</div>
          </div>
          <div className="d-flex gap-2">
            <button className="btn btn-outline-light btn-sm" onClick={load}>
              <i className="bi bi-arrow-clockwise me-2"></i>Оновити
            </button>
            <button
                className="btn btn-light btn-sm"
                onClick={() => setEditing({ id: 0, title: '', content: '', author: '', category: 0, imageUrls: [] })}
            >
              <i className="bi bi-plus-lg me-2"></i>Додати
            </button>
          </div>
        </div>

        {items.length === 0 ? (
            <EmptyState title="Новин немає" hint="Додайте першу новину." />
        ) : (
            <div className="table-responsive">
              <table className="table table-dark table-hover align-middle">
                <thead>
                <tr>
                  <th style={{ width: 90 }}>ID</th>
                  <th>Title</th>
                  <th style={{ width: 160 }}>Category</th>
                  <th style={{ width: 180 }}>Author</th>
                  <th style={{ width: 140 }}>Actions</th>
                </tr>
                </thead>
                <tbody>
                {items.map(n => (
                    <tr key={n.id}>
                      <td className="text-truncate" style={{ maxWidth: 90 }}>{n.id}</td>
                      <td className="text-truncate" style={{ maxWidth: 520 }}>{n.title}</td>
                      <td>
                    <span className="badge rounded-pill ns-badge">
                      {catsByValue.get(n.category) ?? String(n.category)}
                    </span>
                      </td>
                      <td className="text-truncate" style={{ maxWidth: 180 }}>{n.author || '—'}</td>
                      <td>
                        <div className="d-flex gap-2">
                          <button className="btn btn-outline-light btn-sm" onClick={() => setEditing(n)}>
                            <i className="bi bi-pencil"></i>
                          </button>
                          <button className="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target={`#del-${n.id}`}>
                            <i className="bi bi-trash"></i>
                          </button>

                          <ConfirmModal
                              id={`del-${n.id}`}
                              title="Видалити новину?"
                              body={<div>Ви впевнені, що хочете видалити <span className="fw-semibold">{n.title}</span>?</div>}
                              confirmText="Видалити"
                              onConfirm={async () => {
                                try {
                                  await adminService.newsDelete(n.id, token)
                                  setItems(prev => prev.filter(x => x.id !== n.id))
                                  toast.success('Видалено')
                                } catch (e) {
                                  toast.error(e?.data?.message || 'Не вдалося видалити')
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

        {editing && (
            <EditNewsModal
                token={token}
                categories={cats}
                value={editing}
                onClose={() => setEditing(null)}
                onSaved={(saved) => {
                  setItems(prev => {
                    const exists = prev.some(x => x.id === saved.id)
                    if (exists) return prev.map(x => x.id === saved.id ? saved : x)
                    return [saved, ...prev]
                  })
                }}
            />
        )}
      </div>
  )
}

function EditNewsModal({ value, onClose, onSaved, token, categories }) {
  const isEdit = !!value?.id

  const [model, setModel] = React.useState(() => ({
    id: value?.id || 0,
    title: value?.title || '',
    content: value?.content || '',
    author: value?.author || '',
    category: typeof value?.category === 'number' ? value.category : 0,
    images: [],
    imageUrlsText: Array.isArray(value?.imageUrls) ? value.imageUrls.join('\n') : ''
  }))

  const [saving, setSaving] = React.useState(false)

  React.useEffect(() => {
    const prev = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => { document.body.style.overflow = prev }
  }, [])

  async function save() {
    setSaving(true)
    try {
      if (!model.title.trim() || !model.content.trim()) {
        toast.error('Title та Content обовʼязкові')
        return
      }

      if (!isEdit) {

        const saved = await adminService.newsCreate({
          title: model.title,
          content: model.content,
          author: model.author,
          category: Number(model.category) || 0,
          images: model.images
        }, token)

        toast.success('Створено')
        onSaved(saved)
        onClose()
      } else {

        const imageUrls = model.imageUrlsText
            .split('\n')
            .map(x => x.trim())
            .filter(Boolean)

        const saved = await adminService.newsUpdate(model.id, {
          title: model.title,
          content: model.content,
          author: model.author,
          category: Number(model.category) || 0,
          imageUrls
        }, token)

        toast.success('Збережено')
        onSaved(saved)
        onClose()
      }
    } catch (e) {
      toast.error(e?.data?.message || e?.data?.title || 'Не вдалося зберегти')
    } finally {
      setSaving(false)
    }
  }

  return createPortal(
      <>
        <div className="modal fade show" style={{ display: 'block' }} role="dialog" aria-modal="true"
             onMouseDown={(e) => { if (e.target === e.currentTarget) onClose() }}>
          <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div className="modal-content bg-dark text-light border border-secondary">
              <div className="modal-header border-secondary">
                <h5 className="modal-title">{isEdit ? 'Редагувати новину' : 'Нова новина'}</h5>
                <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
              </div>

              <div className="modal-body">
                <div className="mb-2">
                  <label className="form-label">Title</label>
                  <input className="form-control ns-input"
                         value={model.title}
                         onChange={e => setModel(m => ({ ...m, title: e.target.value }))} />
                </div>

                <div className="mb-2">
                  <label className="form-label">Content</label>
                  <textarea className="form-control ns-input" rows={8}
                            value={model.content}
                            onChange={e => setModel(m => ({ ...m, content: e.target.value }))} />
                </div>

                <div className="mb-2">
                  <label className="form-label">Author</label>
                  <input className="form-control ns-input"
                         value={model.author}
                         onChange={e => setModel(m => ({ ...m, author: e.target.value }))} />
                </div>

                <div className="mb-3">
                  <label className="form-label">Category</label>
                  <select className="form-select ns-input"
                          value={model.category}
                          onChange={e => setModel(m => ({ ...m, category: Number(e.target.value) }))}>
                    {categories.map(c => (
                        <option key={c.value} value={c.value}>{c.name}</option>
                    ))}
                  </select>
                </div>

                {!isEdit ? (
                    <div className="mb-2">
                      <label className="form-label">Images (files)</label>
                      <input
                          className="form-control ns-input"
                          type="file"
                          multiple
                          accept=".jpg,.jpeg,.png"
                          onChange={(e) => {
                            const files = Array.from(e.target.files || [])
                            setModel(m => ({ ...m, images: files }))
                          }}
                      />
                      <div className="ns-muted small mt-1">
                        Надсилається як <code>Images[]</code> у multipart/form-data.
                      </div>
                    </div>
                ) : (
                    <div className="mb-2">
                      <label className="form-label">ImageUrls (по 1 на рядок)</label>
                      <textarea
                          className="form-control ns-input"
                          rows={4}
                          value={model.imageUrlsText}
                          onChange={e => setModel(m => ({ ...m, imageUrlsText: e.target.value }))}
                      />
                      <div className="ns-muted small mt-1">
                        UPDATE у бекенді приймає тільки URLs (upload файлів для PUT у твоєму контролері не зроблено).
                      </div>
                    </div>
                )}
              </div>

              <div className="modal-footer border-secondary">
                <button className="btn btn-outline-light" onClick={onClose}>Скасувати</button>
                <button className="btn btn-light" disabled={saving} onClick={save}>
                  <i className="bi bi-save me-2"></i>Зберегти
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="modal-backdrop fade show" onClick={onClose}></div>
      </>,
      document.body
  )
}
