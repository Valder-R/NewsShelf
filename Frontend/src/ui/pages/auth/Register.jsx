import React from 'react'
import {Link, useNavigate} from 'react-router-dom'
import {useForm} from 'react-hook-form'
import {yupResolver} from '@hookform/resolvers/yup'
import * as yup from 'yup'
import {toast} from 'react-toastify'
import {useAuth} from '../../../state/AuthContext.jsx'
import {useTopicOptions} from '../../../hooks/useTopicOptions.js'
import {TopicsPicker} from '../../shared/TopicsPicker.jsx'

const schema = yup.object({
  accountType: yup.string().oneOf(['reader', 'publisher']).required('Обовʼязково'),
  displayName: yup.string().trim().max(128, 'Занадто довге').required('Обовʼязково'),
  email: yup.string().trim().email('Некоректний email').required('Обовʼязково'),
  password: yup.string().min(6, 'Мінімум 6 символів').required('Обовʼязково'),
  confirm: yup.string().oneOf([yup.ref('password')], 'Паролі не співпадають').required('Обовʼязково'),
  bio: yup.string().max(1024, 'Максимум 1024 символів').nullable()
}).required()

function extractApiError(e) {
  const data = e?.response?.data || e?.data
  if (data?.errors && typeof data.errors === 'object') {
    const firstField = Object.keys(data.errors)[0]
    const firstMsg = data.errors[firstField]?.[0]
    if (firstMsg) return firstMsg
  }
  return (
    data?.message ||
    data?.title ||
    (typeof data === 'string' ? data : null) ||
    e?.message ||
    'Не вдалося зареєструватись'
  )
}

export default function Register() {
  const { register: regAuth } = useAuth()
  const nav = useNavigate()

  const { topics, loading: topicsLoading } = useTopicOptions()
  const [selectedTopics, setSelectedTopics] = React.useState([])

  const { register, handleSubmit, formState: { errors, isSubmitting }, watch } = useForm({
    resolver: yupResolver(schema),
    defaultValues: { accountType: 'reader', displayName: '', email: '', password: '', confirm: '', bio: '' }
  })

  const accountType = watch('accountType')

  async function onSubmit(v) {
    try {
      const favoriteTopics = Array.isArray(selectedTopics) && selectedTopics.length ? selectedTopics : null

      await regAuth({

        email: v.email,
        password: v.password,
        displayName: v.displayName,
        bio: v.bio || '',
        favoriteTopics,

        accountType: v.accountType
      })

      if (accountType === 'publisher') {
        toast.info('Акаунт створено як Читач. Роль Публікатора призначає адміністратор.')
      }

      nav('/', { replace: true })
    } catch (e) {
      toast.error(extractApiError(e))
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <div className="ns-card ns-shadow p-4">
          <h1 className="h4 fw-semibold mb-3">Реєстрація</h1>

          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="row g-3">
              <div className="col-12 col-md-5">
                <label className="form-label">Тип акаунта</label>
                <select className="form-select" {...register('accountType')}>
                  <option value="reader">Читач</option>
                  <option value="publisher">Публікатор новин</option>
                </select>
                {errors.accountType && <div className="text-danger small mt-1">{errors.accountType.message}</div>}
              </div>

              <div className="col-12 col-md-7">
                <label className="form-label">DisplayName</label>
                <input className="form-control" placeholder="Ваше імʼя / псевдонім" {...register('displayName')} />
                {errors.displayName && <div className="text-danger small mt-1">{errors.displayName.message}</div>}
              </div>

              <div className="col-12">
                <label className="form-label">Email</label>
                <input className="form-control" placeholder="email@example.com" {...register('email')} />
                {errors.email && <div className="text-danger small mt-1">{errors.email.message}</div>}
              </div>

              <div className="col-12 col-md-6">
                <label className="form-label">Пароль</label>
                <input type="password" className="form-control" {...register('password')} />
                {errors.password && <div className="text-danger small mt-1">{errors.password.message}</div>}
              </div>

              <div className="col-12 col-md-6">
                <label className="form-label">Підтвердження</label>
                <input type="password" className="form-control" {...register('confirm')} />
                {errors.confirm && <div className="text-danger small mt-1">{errors.confirm.message}</div>}
              </div>

              <div className="col-12">
                <label className="form-label">Bio (опційно)</label>
                <textarea className="form-control" rows={4} {...register('bio')} />
                {errors.bio && <div className="text-danger small mt-1">{errors.bio.message}</div>}
              </div>

              <div className="col-12">
                <TopicsPicker
                  label="FavoriteTopics"
                  options={topics}
                  value={selectedTopics}
                  onChange={setSelectedTopics}
                  disabled={topicsLoading}
                />
              </div>

              <div className="col-12">
                <button className="btn btn-light w-100" disabled={isSubmitting}>
                  <i className="bi bi-person-plus me-2"></i>
                  {isSubmitting ? 'Створення...' : 'Створити акаунт'}
                </button>
              </div>
            </div>
          </form>

          <div className="ns-muted mt-3">
            Уже є акаунт? <Link to="/login">Увійти</Link>
          </div>
        </div>
      </div>
    </div>
  )
}
