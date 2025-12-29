import {formatDistanceToNow} from 'date-fns'
import {uk} from 'date-fns/locale'

export function CommentList({ comments = [] }) {
  return (
    <div className="d-flex flex-column gap-3">
      {comments.map(c => {
        const when = c?.createdAt ? formatDistanceToNow(new Date(c.createdAt), { addSuffix: true, locale: uk }) : ''
        return (
          <div key={c.id} className="ns-card p-3">
            <div className="d-flex justify-content-between align-items-start gap-3">
              <div className="fw-semibold">{c?.user?.name || 'User'}</div>
              <div className="ns-muted small">{when}</div>
            </div>
            <div className="mt-2">{c.text}</div>
          </div>
        )
      })}
    </div>
  )
}
