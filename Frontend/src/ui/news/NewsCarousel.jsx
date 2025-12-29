import React from 'react'
import {NewsCard} from './NewsCard.jsx'

function chunk(arr, size) {
  const res = []
  for (let i = 0; i < arr.length; i += size) res.push(arr.slice(i, i + size))
  return res
}

export function NewsCarousel({ id, title, subtitle, items = [], perSlide = 2 }) {
  if (!items?.length) return null

  const slides = chunk(items, perSlide)

  return (
    <section className="mb-4">
      {(title || subtitle) && (
        <div className="d-flex justify-content-between align-items-end mb-2">
          <div>
            {title ? <h2 className="h5 mb-0">{title}</h2> : null}
            {subtitle ? <div className="ns-muted small">{subtitle}</div> : null}
          </div>
          <div className="ns-muted small">{items.length} items</div>
        </div>
      )}

      <div id={id} className="carousel slide" data-bs-ride="carousel">
        <div className="carousel-indicators">
          {slides.map((_, idx) => (
            <button
              key={idx}
              type="button"
              data-bs-target={`#${id}`}
              data-bs-slide-to={idx}
              className={idx === 0 ? 'active' : ''}
              aria-current={idx === 0 ? 'true' : 'false'}
              aria-label={`Slide ${idx + 1}`}
            />
          ))}
        </div>

        <div className="carousel-inner">
          {slides.map((group, idx) => (
            <div key={idx} className={`carousel-item ${idx === 0 ? 'active' : ''}`}>
              <div className="row g-3 px-1 pb-4">
                {group.map(item => (
                  <div key={item.id} className={`col-12 col-md-${perSlide === 2 ? 6 : 4}`}>
                    <NewsCard item={item} />
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>

        {slides.length > 1 && (
          <>
            <button className="carousel-control-prev" type="button" data-bs-target={`#${id}`} data-bs-slide="prev">
              <span className="carousel-control-prev-icon" aria-hidden="true"></span>
              <span className="visually-hidden">Previous</span>
            </button>
            <button className="carousel-control-next" type="button" data-bs-target={`#${id}`} data-bs-slide="next">
              <span className="carousel-control-next-icon" aria-hidden="true"></span>
              <span className="visually-hidden">Next</span>
            </button>
          </>
        )}
      </div>
    </section>
  )
}
