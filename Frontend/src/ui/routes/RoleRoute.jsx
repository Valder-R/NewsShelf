import {Navigate, useLocation} from 'react-router-dom'
import {useAuth} from '../../state/AuthContext.jsx'
import {LoadingOverlay} from '../shared/LoadingOverlay.jsx'
import {decodeJwtPayload, extractRolesFromClaims, hasAnyRole} from '../../lib/auth/jwt.js'

export function RoleRoute({ children, allowed = [] }) {
  const { loading, isAuthenticated, token } = useAuth()
  const loc = useLocation()

  if (loading) return <LoadingOverlay label="Перевірка доступу..." />
  if (!isAuthenticated) return <Navigate to="/login" replace state={{ from: loc.pathname }} />

  const claims = decodeJwtPayload(token)
  const roles = extractRolesFromClaims(claims)

  const ok = hasAnyRole(roles, allowed)
  if (!ok) return <Navigate to="/" replace />

  return children
}
