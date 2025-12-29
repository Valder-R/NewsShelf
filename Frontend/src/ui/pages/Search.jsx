import React from 'react'
import {useSearchParams} from 'react-router-dom'
import {useDebouncedValue} from '../../hooks/useDebouncedValue.js'
import {EmptyState} from '../shared/EmptyState.jsx'
import {LoadingOverlay} from '../shared/LoadingOverlay.jsx'
import {NewsCard} from '../news/NewsCard.jsx'
import {searchService} from '../../services/searchService.js'
import {useAuth} from '../../state/AuthContext.jsx'

export default function Search() {
  const { user } = useAuth()
  const [params, setParams] = useSearchParams()
  const [q, setQ] = React.useState(params.get('q') || '')
  const [sort, setSort] = React.useState(params.get('sort') || 'new')

  const debounced = useDebouncedValue(q, 400)

  const [loading, setLoading] = React.useState(false)
  const [res, setRes] = React.useState({ items: [], total: 0, page: 1, pageSize: 12 })
  const [page, setPage] = React.useState(Number(params.get('page') || 1))

  React.useEffect(() => {
    setParams({ q: debounced || '', sort, page: String(page) }, { replace: true })
  }, [debounced, sort, page])

  React.useEffect(() => {
    ;(async () => {
      setLoading(true)
      try {
        const r = await searchService.search({ q: debounced, sort, page, pageSize: 12, userId: user?.id || null })
        setRes(r)
      } finally {
        setLoading(false)
      }
    })()
  }, [debounced, sort, page])

  return (
    <div>
      <div className="ns-hero">
        <h1 className="display-6 fw-semibold mb-2">Пошук</h1>
        <div className="ns-muted">Пошук за ключовими словами, авторами, датою (за підтримки бекенду).</div>
      </div>

      <div className="ns-card ns-shadow p-3 mb-3">
        <div className="row g-2 align-items-center">
          <div className="col-12 col-md-8">
            <div className="input-group">
              <span className="input-group-text bg-transparent text-light border-secondary">
                <i className="bi bi-search"></i>
              </span>
              <input
                className="form-control ns-input"
                value={q}
                onChange={(e) => setQ(e.target.value)}
                placeholder="Наприклад: AI, економіка, спорт..."
              />
            </div>
          </div>

          <div className="col-12 col-md-4">
            <select className="form-select ns-input" value={sort} onChange={(e) => setSort(e.target.value)}>
              <option value="new">Спочатку нові</option>
              <option value="old">Спочатку старі</option>
              <option value="relevance">За релевантністю</option>
            </select>
          </div>
        </div>
      </div>

      {loading ? (
        <LoadingOverlay label="Пошук..." />
      ) : res.items.length === 0 ? (
        <EmptyState hint="Спробуйте інший запит або перевірте SearchService (base URL та ендпоїнти)." />
      ) : (
        <>
          <div className="row g-3">
            {res.items.map(item => (
              <div key={item.id} className="col-12 col-md-6 col-lg-4">
                <NewsCard item={item} />
              </div>
            ))}
          </div>

          <div className="d-flex justify-content-between align-items-center mt-3">
            <button className="btn btn-outline-light" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
              <i className="bi bi-arrow-left me-2"></i>Попередня
            </button>
            <div className="ns-muted small">{res.total} результатів · Сторінка {page}</div>
            <button
              className="btn btn-outline-light"
              disabled={(page * res.pageSize) >= res.total}
              onClick={() => setPage(p => p + 1)}
            >
              Наступна<i className="bi bi-arrow-right ms-2"></i>
            </button>
          </div>
        </>
      )}
    </div>
  )
}
