import React from 'react'
import {NewsCard} from '../news/NewsCard.jsx'
import {NewsCarousel} from '../news/NewsCarousel.jsx'
import {EmptyState} from '../shared/EmptyState.jsx'
import {LoadingOverlay} from '../shared/LoadingOverlay.jsx'
import {newsService} from '../../services/newsService.js'
import {recService} from '../../services/recService.js'
import {useAuth} from '../../state/AuthContext.jsx'

export default function Home() {
  const { user } = useAuth()
  const [loading, setLoading] = React.useState(true)
  const [feed, setFeed] = React.useState({ items: [], total: 0, page: 1, pageSize: 8 })
  const [trending, setTrending] = React.useState([])
  const [recommended, setRecommended] = React.useState([])

  const [page, setPage] = React.useState(1)

  React.useEffect(() => {
    ;(async () => {
      setLoading(true)
      try {
        const res = await newsService.list({ page, pageSize: 8, sort: 'new' })
        setFeed(res)

        try { setTrending(await recService.trending()) } catch {}
        try { setRecommended(await recService.recommended(user?.id)) } catch {}
      } finally {
        setLoading(false)
      }
    })()
  }, [page])

  if (loading) return <LoadingOverlay label="Завантаження стрічки..." />

  return (
    <div>
      <div className="ns-hero">
        <h1 className="display-6 fw-semibold mb-2">Стрічка новин</h1>
        <div className="ns-muted">Останні публікації, рекомендації та тренди.</div>
      </div>

      {(recommended?.length > 0) && (
        <section className="mb-4">
          <div className="d-flex justify-content-between align-items-end mb-2">
            <h2 className="h5 mb-0">Рекомендовано для вас</h2>
            <span className="ns-muted small">RecService</span>
          </div>
          <div className="row g-3">
            {recommended.slice(0, 4).map(item => (
              <div key={item.id} className="col-12 col-md-6 col-lg-3">
                <NewsCard item={item} />
              </div>
            ))}
          </div>
        </section>
      )}

      {(trending?.length > 0) && (
        <NewsCarousel
          id="trendingCarousel"
          title="Тренди"
          subtitle="Trending"
          items={trending.slice(0, 8)}
          perSlide={2}
        />
      )}

      <section>
        <div className="d-flex justify-content-between align-items-end mb-2">
          <h2 className="h5 mb-0">Останні новини</h2>
          <span className="ns-muted small">{feed.total} items</span>
        </div>

        {feed.items.length === 0 ? (
          <EmptyState hint="Спробуйте змінити фільтри або перевірте налаштування API в .env" />
        ) : (
          <>
            <NewsCarousel
              id="latestCarousel"
              title=""
              items={feed.items}
              perSlide={2}
            />

            <div className="d-flex justify-content-between align-items-center mt-2">
              <button className="btn btn-outline-ns" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
                <i className="bi bi-arrow-left me-2"></i>Попередня
              </button>
              <div className="ns-muted small">Сторінка {page}</div>
              <button
                className="btn btn-outline-ns"
                disabled={(page * feed.pageSize) >= feed.total}
                onClick={() => setPage(p => p + 1)}
              >
                Наступна<i className="bi bi-arrow-right ms-2"></i>
              </button>
            </div>
          </>
        )}
      </section>
    </div>
  )
}
