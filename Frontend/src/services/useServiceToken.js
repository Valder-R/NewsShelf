import {useAuth} from '../state/AuthContext.jsx'

export function useServiceToken() {
  const { token } = useAuth()
  return token
}
