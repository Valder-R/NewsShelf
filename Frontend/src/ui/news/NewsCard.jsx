import {useNavigate} from 'react-router-dom'
import {formatDistanceToNow} from 'date-fns'
import {uk} from 'date-fns/locale'
import {normalizeNewsImageUrl} from '../../services/newsService.js'

export function NewsCard({ item }) {
  const navigate = useNavigate()
  const img = Array.isArray(item?.imageUrls) && item.imageUrls.length > 0
    ? normalizeNewsImageUrl(item.imageUrls[0])
    : ''
  const when = item?.publishedAt
    ? formatDistanceToNow(new Date(item.publishedAt), { addSuffix: true, locale: uk })
    : ''
  const tags = item?.tags || []

  return (
    <div
      className="ns-card p-3 h-100"
      role="button"
      tabIndex={0}
      onClick={() => navigate(`/news/${item.id}`)}
      onKeyDown={(e) => { if (e.key === 'Enter') navigate(`/news/${item.id}`) }}
      style={{ cursor: 'pointer' }}
    >
      <div className="d-flex align-items-start justify-content-between gap-3">
        <div className="d-flex gap-3 align-items-start">
          {img ? (
            <img
              src={img}
              alt=""
              style={{ width: 86, height: 86, objectFit: 'cover', borderRadius: 12, border: '1px solid rgba(255,255,255,0.12)' }}
              loading="lazy"
              onClick={(e) => e.stopPropagation()}
            />
          ) : null}

          <div>
          <h5 className="mb-1" style={{ color: 'rgba(229,231,235,0.95)' }}>
            {item.title}
          </h5>
          <div className="ns-muted small mb-2">
            {item.author ? <span className="me-2"><i className="bi bi-person me-1"></i>{item.author}</span> : null}
            {when ? <span><i className="bi bi-clock me-1"></i>{when}</span> : null}
          </div>
          </div>
        </div>

        {item.sourceUrl && (
          <a
            className="btn btn-outline-ns btn-sm"
            href={item.sourceUrl}
            target="_blank"
            rel="noreferrer"
            onClick={(e) => e.stopPropagation()}
          >
            Джерело <i className="bi bi-box-arrow-up-right ms-1"></i>
          </a>
        )}
      </div>

      {item.summary && (
        <p className="mt-2 mb-3 ns-muted ns-line-clamp-3">
          {item.summary}
        </p>
      )}

      {tags.length > 0 && (
        <div className="d-flex flex-wrap gap-2">
          {tags.slice(0, 6).map(t => (
            <span key={t} className="badge rounded-pill ns-badge">{t}</span>
          ))}
        </div>
      )}
    </div>
  )
}
