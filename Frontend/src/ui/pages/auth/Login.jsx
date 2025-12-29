import React from 'react'
import {Link, useLocation, useNavigate} from 'react-router-dom'
import {useForm} from 'react-hook-form'
import {yupResolver} from '@hookform/resolvers/yup'
import * as yup from 'yup'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {decodeJwtPayload, extractRolesFromClaims} from '../../../lib/auth/jwt.js'

const schema = yup.object({
  email: yup.string().email('Некоректний email').required('Обовʼязково'),
  password: yup.string().required('Обовʼязково')
}).required()

function extractApiError(e) {
  const data = e?.response?.data || e?.data
  if (data?.errors && typeof data.errors === 'object') {
    const k = Object.keys(data.errors)[0]
    const msg = data.errors[k]?.[0]
    if (msg) return msg
  }
  return (
      data?.message ||
      data?.title ||
      (typeof data === 'string' ? data : null) ||
      e?.message ||
      'Не вдалося увійти'
  )
}

export default function Login() {
  const { login } = useAuth()
  const nav = useNavigate()
  const loc = useLocation()
  const redirect = loc.state?.from || '/'

  function readStoredToken() {
    try {
      const raw = localStorage.getItem('newsshelf.auth')
      const parsed = raw ? JSON.parse(raw) : null
      return parsed?.token || ''
    } catch {
      return ''
    }
  }

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    resolver: yupResolver(schema),
    defaultValues: { email: '', password: '' }
  })

  return (
      <div className="row justify-content-center">
        <div className="col-12 col-md-7 col-lg-5">
          <div className="ns-card ns-shadow p-4">
            <h1 className="h4 fw-semibold mb-3">Вхід</h1>

            <form onSubmit={handleSubmit(async (v) => {
              try {
                await login(v)

                const claims = decodeJwtPayload(readStoredToken())
                const roles = extractRolesFromClaims(claims)

                if (roles.includes('ADMIN')) {
                  nav('/admin', { replace: true })
                } else if (roles.includes('PUBLISHER')) {
                  nav('/publisher', { replace: true })
                } else {
                  nav(redirect, { replace: true })
                }
              } catch (e) {
                toast.error(extractApiError(e))
              }
            })}>
              <div className="mb-3">
                <label className="form-label">Email</label>
                <input className="form-control ns-input" placeholder="email@example.com" {...register('email')} />
                {errors.email && <div className="text-danger small mt-1">{errors.email.message}</div>}
              </div>

              <div className="mb-3">
                <label className="form-label">Пароль</label>
                <input className="form-control ns-input" type="password" {...register('password')} />
                {errors.password && <div className="text-danger small mt-1">{errors.password.message}</div>}
              </div>

              <button className="btn btn-light w-100" disabled={isSubmitting} type="submit">
                <i className="bi bi-box-arrow-in-right me-2"></i>
                Увійти
              </button>
            </form>

            <div className="ns-muted small mt-3">
              Немає акаунта? <Link to="/register" className="text-light fw-semibold">Зареєструватись</Link>
            </div>

            <hr className="border-secondary my-3" />

            <div className="ns-muted small">
              Якщо у вас налаштований OAuth (Google/Facebook) у бекенді — додайте відповідні кнопки тут.
            </div>
          </div>
        </div>
      </div>
  )
}
