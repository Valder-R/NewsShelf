import React from 'react'
import {useParams} from 'react-router-dom'
import {toast} from 'react-toastify'
import {LoadingOverlay} from '../shared/LoadingOverlay.jsx'
import {EmptyState} from '../shared/EmptyState.jsx'
import {LikeButton} from '../news/LikeButton.jsx'
import {CommentList} from '../news/CommentList.jsx'
import {CommentForm} from '../news/CommentForm.jsx'
import {newsService, normalizeNewsImageUrl} from '../../services/newsService.js'
import {useAuth} from '../../state/AuthContext.jsx'

export default function NewsDetails() {
  const { id } = useParams()
  const { isAuthenticated, user, token } = useAuth()

  const [loading, setLoading] = React.useState(true)
  const [item, setItem] = React.useState(null)
  const [comments, setComments] = React.useState([])
  const [likeState, setLikeState] = React.useState({ liked: false, count: 0 })

  async function loadAll() {
    setLoading(true)
    try {
      const n = await newsService.details(id)
      setItem(n)
      const c = await newsService.comments(id)
      setComments(c || [])

      const count = Number(n.likesCount ?? (n.likes?.length ?? 0))
      const liked = Boolean(n.liked ?? (n.likes?.includes?.(user?.id)))
      setLikeState({ liked, count })
    } finally {
      setLoading(false)
    }
  }

  React.useEffect(() => { loadAll() }, [id, user?.id])

  if (loading) return <LoadingOverlay label="Завантаження новини..." />
  if (!item) return <EmptyState title="Новину не знайдено" />

  const canLike = isAuthenticated

  return (
    <div className="row g-3">
      <div className="col-12 col-lg-8">
        <div className="ns-card ns-shadow p-4">
          <div className="d-flex justify-content-between align-items-start gap-3">
            <div>
              <h1 className="h3 fw-semibold mb-2">{item.title}</h1>
              <div className="ns-muted">
                {item.author && <span className="me-3"><i className="bi bi-person me-1"></i>{item.author}</span>}
                {item.publishedAt && <span><i className="bi bi-clock me-1"></i>{new Date(item.publishedAt).toLocaleString()}</span>}
              </div>
            </div>

            <div className="d-flex gap-2">
              {item.sourceUrl && (
                <a className="btn btn-outline-light btn-sm" href={item.sourceUrl} target="_blank" rel="noreferrer">
                  Джерело <i className="bi bi-box-arrow-up-right ms-1"></i>
                </a>
              )}

              <LikeButton
                liked={likeState.liked}
                count={likeState.count}
                disabled={!canLike}
                onToggle={async () => {
                  if (!isAuthenticated) { toast.info('Увійдіть, щоб ставити лайки'); return }
                  const next = !likeState.liked
                  setLikeState(s => ({ ...s, liked: next, count: s.count + (next ? 1 : -1) }))
                  try {
                    const r = await newsService.setLike(id, next, token)
                    if (r?.likesCount != null) setLikeState({ liked: next, count: Number(r.likesCount) })
                  } catch (e) {

                    setLikeState(s => ({ ...s, liked: !next, count: s.count + (next ? -1 : 1) }))
                    toast.error(e?.data?.message || 'Не вдалося оновити лайк')
                  }
                }}
              />
            </div>
          </div>

          {item.summary && <p className="mt-3 ns-muted">{item.summary}</p>}

          {Array.isArray(item.imageUrls) && item.imageUrls.length > 0 && (
            <div className="mt-3">
              <div className="row g-2">
                {item.imageUrls.slice(0, 6).map((u) => {
                  const src = normalizeNewsImageUrl(u)
                  return (
                    <div key={u} className="col-6 col-md-4">
                      <a href={src} target="_blank" rel="noreferrer" onClick={(e) => e.stopPropagation()}>
                        <img
                          src={src}
                          alt=""
                          style={{ width: '100%', height: 160, objectFit: 'cover', borderRadius: 14, border: '1px solid rgba(255,255,255,0.12)' }}
                          loading="lazy"
                        />
                      </a>
                    </div>
                  )
                })}
              </div>
            </div>
          )}

          <hr className="border-secondary my-4" />

          <article style={{ whiteSpace: 'pre-wrap' }}>
            {item.content || item.body || 'Контент відсутній або має інший формат у вашому бекенді.'}
          </article>

          {Array.isArray(item.tags) && item.tags.length > 0 && (
            <div className="mt-4 d-flex flex-wrap gap-2">
              {item.tags.map(t => <span key={t} className="badge rounded-pill ns-badge">{t}</span>)}
            </div>
          )}
        </div>
      </div>

      <div className="col-12 col-lg-4">
        <div className="d-flex flex-column gap-3">
          <div className="ns-card ns-shadow p-3">
            <div className="fw-semibold mb-2">Коментарі</div>
            <div className="ns-muted small">Кількість: {comments.length}</div>
          </div>

          {isAuthenticated ? (
            <CommentForm onSubmit={async ({ text }) => {
              try {
                const c = await newsService.addComment(id, { text }, token)
                setComments(prev => [c, ...prev])
                toast.success('Коментар додано')
              } catch (e) {
                toast.error(e?.data?.message || 'Не вдалося додати коментар')
              }
            }} />
          ) : (
            <div className="ns-card p-3">
              <div className="ns-muted">Увійдіть, щоб залишати коментарі.</div>
            </div>
          )}

          {comments.length === 0 ? (
            <EmptyState title="Поки немає коментарів" hint="Станьте першим, хто залишить відгук." />
          ) : (
            <CommentList comments={comments} />
          )}
        </div>
      </div>
    </div>
  )
}
