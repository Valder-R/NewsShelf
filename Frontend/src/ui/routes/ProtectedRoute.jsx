import {Navigate} from 'react-router-dom'
import {useAuth} from '../../state/AuthContext.jsx'
import {LoadingOverlay} from '../shared/LoadingOverlay.jsx'

export function ProtectedRoute({ children }) {
  const { loading, isAuthenticated } = useAuth()
  if (loading) return <LoadingOverlay label="Завантаження профілю..." />
  if (!isAuthenticated) return <Navigate to="/login" replace />
  return children
}
