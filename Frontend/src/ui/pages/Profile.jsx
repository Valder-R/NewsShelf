import React from 'react'
import {useForm} from 'react-hook-form'
import {yupResolver} from '@hookform/resolvers/yup'
import * as yup from 'yup'
import {toast} from 'react-toastify'
import {useAuth} from '../../state/AuthContext.jsx'
import {useTopicOptions} from '../../hooks/useTopicOptions.js'
import {TopicsPicker} from '../shared/TopicsPicker.jsx'

const schema = yup.object({
    displayName: yup.string().trim().max(128, 'Занадто довге').required('Обовʼязково'),
    bio: yup.string().max(1024, 'Максимум 1024 символів').nullable()
}).required()

export default function Profile() {
    const {user, roles, updateProfile} = useAuth()
    const {topics, loading: topicsLoading} = useTopicOptions()
    const [selectedTopics, setSelectedTopics] = React.useState([])
    const [topicsError, setTopicsError] = React.useState(null)

    const {
        register,
        handleSubmit,
        reset,
        formState: {errors, isSubmitting}
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {displayName: '', bio: ''}
    })

    React.useEffect(() => {
        reset({
            displayName: user?.displayName || '',
            bio: user?.bio || ''
        })
        setSelectedTopics(
            Array.isArray(user?.favoriteTopics) ? user.favoriteTopics : []
        )
    }, [user, reset])

    async function onSubmit(v) {
        if (!Array.isArray(selectedTopics) || selectedTopics.length === 0) {
            setTopicsError('Потрібно обрати хоча б одну тему')
            return
        }

        setTopicsError(null)

        try {
            await updateProfile({
                displayName: v.displayName,
                bio: v.bio || '',
                favoriteTopics: selectedTopics
            })
            toast.success('Профіль оновлено')
        } catch (e) {
            toast.error(
                e?.response?.data?.message ||
                e?.data?.message ||
                'Не вдалося оновити профіль'
            )
        }
    }

    return (
        <div className="row justify-content-center">
            <div className="col-12 col-lg-8">
                <div className="ns-card ns-shadow p-4">
                    <h1 className="h4 fw-semibold mb-3">Профіль</h1>

                    <div className="row g-3 mb-3">
                        <div className="col-12 col-md-6">
                            <div className="ns-muted small">Email</div>
                            <div className="fw-semibold">{user?.email || '—'}</div>
                        </div>

                        <div className="col-12 col-md-6">
                            <div className="ns-muted small">Ролі</div>
                            <div className="d-flex flex-wrap gap-2">
                                {(roles || []).length ? (
                                    roles.map(r => (
                                        <span key={r} className="badge text-bg-secondary">
                      {r}
                    </span>
                                    ))
                                ) : (
                                    <span className="ns-muted">—</span>
                                )}
                            </div>
                        </div>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className="row g-3">
                            <div className="col-12">
                                <label className="form-label">DisplayName</label>
                                <input
                                    className="form-control ns-input"
                                    {...register('displayName')}
                                />
                                {errors.displayName && (
                                    <div className="text-danger small mt-1">
                                        {errors.displayName.message}
                                    </div>
                                )}
                            </div>

                            <div className="col-12">
                                <label className="form-label">Bio</label>
                                <textarea
                                    className="form-control ns-input"
                                    rows={4}
                                    {...register('bio')}
                                />
                                {errors.bio && (
                                    <div className="text-danger small mt-1">
                                        {errors.bio.message}
                                    </div>
                                )}
                            </div>

                            <div className="col-12">
                                <TopicsPicker
                                    label="FavoriteTopics"
                                    options={topics}
                                    value={selectedTopics}
                                    onChange={setSelectedTopics}
                                    disabled={topicsLoading || isSubmitting}
                                />
                                {topicsError && (
                                    <div className="text-danger small mt-1">
                                        {topicsError}
                                    </div>
                                )}
                            </div>

                            <div className="col-12">
                                <button
                                    className="btn btn-ns w-100"
                                    disabled={isSubmitting}
                                >
                                    <i className="bi bi-save me-2"></i>
                                    {isSubmitting ? 'Збереження...' : 'Зберегти'}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}
