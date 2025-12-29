import React from 'react'
import {NavLink, Outlet} from 'react-router-dom'
import clsx from 'clsx'
import {useAuth} from '../../../state/AuthContext.jsx'

export default function AdminLayout() {
  const { roles } = useAuth()
  const isAdmin = roles.includes('ADMIN')

  return (
    <div className="row g-3">
      <div className="col-12 col-lg-3">
        <div className="ns-card ns-shadow p-3">
          <div className="fw-semibold mb-2">
            <i className="bi bi-speedometer2 me-2"></i>
            Адмін-панель
          </div>
          <div className="list-group list-group-flush">
            <NavLink end to="/admin" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
              Огляд
            </NavLink>
            <NavLink to="/admin/news" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
              Новини
            </NavLink>
            {isAdmin && (
              <>
                <NavLink to="/admin/users" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
                  Користувачі
                </NavLink>
                <NavLink to="/admin/publishers" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
                  Публікатори
                </NavLink>
                <NavLink to="/admin/status" className={({isActive}) => clsx('list-group-item list-group-item-action bg-transparent text-light border-secondary', isActive && 'fw-semibold')}>
                  Стан системи
                </NavLink>
              </>
            )}
          </div>
        </div>
      </div>

      <div className="col-12 col-lg-9">
        <Outlet />
      </div>
    </div>
  )
}
