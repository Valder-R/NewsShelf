import React from 'react'
import {NavLink, Outlet} from 'react-router-dom'
import clsx from 'clsx'

export default function PublisherLayout() {
  return (
    <div className="row g-3">
      <div className="col-12 col-lg-3">
        <div className="ns-card ns-shadow p-3">
          <div className="fw-semibold mb-2">
            <i className="bi bi-megaphone me-2"></i>
            Publisher
          </div>
          <div className="list-group list-group-flush">
            <NavLink end to="/publisher" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
              Огляд
            </NavLink>
            <NavLink to="/publisher/news" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
              Мої новини
            </NavLink>
          </div>
        </div>
      </div>

      <div className="col-12 col-lg-9">
        <Outlet />
      </div>
    </div>
  )
}
