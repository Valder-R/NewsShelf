import React from 'react'
import {Link, NavLink, useNavigate} from 'react-router-dom'
import clsx from 'clsx'
import {useAuth} from '../../state/AuthContext.jsx'

export function AppNavbar() {
  const { isAuthenticated, user, roles, logout } = useAuth()
  const navigate = useNavigate()
  const isAdmin = roles.includes('ADMIN')
  const isPublisher = !isAdmin && roles.includes('PUBLISHER')

  return (
      <nav className="navbar navbar-expand-lg navbar-dark sticky-top">
        <div className="container">
          <Link className="navbar-brand fw-semibold" to="/">
            <span className="me-2"><i className="bi bi-newspaper"></i></span>
            NewsShelf
          </Link>

          <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#nsNav"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div id="nsNav" className="collapse navbar-collapse">
            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
              <li className="nav-item">
                <NavLink className={({ isActive }) => clsx('nav-link', isActive && 'active')} to="/">
                  Головна
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className={({ isActive }) => clsx('nav-link', isActive && 'active')} to="/search">
                  Пошук
                </NavLink>
              </li>

              {isAdmin && (
                  <li className="nav-item">
                    <NavLink className={({ isActive }) => clsx('nav-link', isActive && 'active')} to="/admin">
                      <i className="bi bi-shield-lock me-1"></i> Адмін-панель
                    </NavLink>
                  </li>
              )}

              {isPublisher && (
                <li className="nav-item">
                  <NavLink className={({ isActive }) => clsx('nav-link', isActive && 'active')} to="/publisher">
                    <i className="bi bi-megaphone me-1"></i> Publisher
                  </NavLink>
                </li>
              )}
            </ul>

            <div className="d-flex gap-2 align-items-center">
              {!isAuthenticated ? (
                  <>
                    <Link className="btn btn-outline-ns btn-sm" to="/login">Увійти</Link>
                    <Link className="btn btn-ns btn-sm" to="/register">Реєстрація</Link>
                  </>
              ) : (
                  <>
                    <Link className="btn btn-outline-ns btn-sm" to="/profile">
                      <i className="bi bi-person-circle me-2"></i>
                      {user?.displayName || user?.email || 'Профіль'}
                    </Link>

                    <button
                        className="btn btn-outline-ns btn-sm"
                        onClick={() => {
                          logout()
                          navigate('/')
                        }}
                    >
                      <i className="bi bi-box-arrow-right me-2"></i>
                      Вийти
                    </button>
                  </>
              )}
            </div>
          </div>
        </div>
      </nav>
  )
}