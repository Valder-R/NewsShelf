import React from 'react'
import {toast} from 'react-toastify'
import {authService} from '../services/authService.js'
import {decodeJwtPayload, extractRolesFromClaims, extractUserFromClaims} from '../lib/auth/jwt.js'

const AuthContext = React.createContext(null)
const STORAGE_KEY = 'newsshelf.auth'

function extractToken(res) {
  if (!res) return ''
  if (typeof res === 'string') return res
  return (
    res.accessToken ||
    res.token ||
    res.jwt ||
    res.AccessToken ||
    res.Token ||
    ''
  )
}

export function AuthProvider({ children }) {
  const [token, setToken] = React.useState('')
  const [user, setUser] = React.useState(null)
  const [loading, setLoading] = React.useState(true)

  const roles = React.useMemo(() => {
    const claims = decodeJwtPayload(token)
    return extractRolesFromClaims(claims)
  }, [token])

  const isAuthenticated = Boolean(token)

  function persist(nextToken) {
    const t = String(nextToken || '')
    setToken(t)
    try {
      if (t) localStorage.setItem(STORAGE_KEY, JSON.stringify({ token: t }))
      else localStorage.removeItem(STORAGE_KEY)
    } catch {  }
  }

  React.useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (raw) {
        const parsed = JSON.parse(raw)
        if (parsed?.token) setToken(String(parsed.token))
      }
    } catch {  }
    setLoading(false)
  }, [])

  React.useEffect(() => {
    let mounted = true
    ;(async () => {
      if (!token) {
        if (mounted) setUser(null)
        return
      }

      try {
        const me = await authService.me(token)
        if (!mounted) return
        setUser(me || null)
      } catch {
        if (!mounted) return
        const tu = extractUserFromClaims(decodeJwtPayload(token))
        setUser({
          id: tu.id,
          email: tu.email,
          displayName: tu.name
        })
      }
    })()
    return () => { mounted = false }
  }, [token])

  async function login({ email, password }) {
    const res = await authService.login({ email, password })
    const t = extractToken(res)
    persist(t)
    toast.success('Вхід виконано')

    return t
  }

  async function register(payload) {
    const res = await authService.register({
      email: payload?.email,
      password: payload?.password,
      accountType: payload?.accountType,
      displayName: payload?.displayName,
      bio: payload?.bio ?? '',
      favoriteTopics: payload?.favoriteTopics ?? null
    })
    const t = extractToken(res)
    persist(t)
    toast.success('Реєстрація успішна')
    return t
  }

  function logout() {
    persist('')
    setUser(null)
    toast.info('Ви вийшли з акаунту')
  }

  async function updateProfile(payload) {
    const updated = await authService.updateProfile({
      displayName: payload?.displayName,
      bio: payload?.bio ?? '',
      favoriteTopics: payload?.favoriteTopics ?? null
    }, token)
    setUser(updated)
    return updated
  }

  const value = React.useMemo(() => ({
    token,
    user,
    roles,
    loading,
    isAuthenticated,
    login,
    register,
    logout,
    updateProfile
  }), [token, user, roles, loading, isAuthenticated])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = React.useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
