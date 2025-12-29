

export function decodeJwtPayload(token) {
  if (!token || typeof token !== 'string') return null
  const parts = token.split('.')
  if (parts.length < 2) return null

  try {
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=')
    const json = atob(padded)
    return JSON.parse(json)
  } catch {
    return null
  }
}

function normalizeRoles(value) {
  if (!value) return []
  if (Array.isArray(value)) return value.map(String)
  return [String(value)]
}

export function extractRolesFromClaims(claims) {
  if (!claims || typeof claims !== 'object') return []

  const schemaRoleClaim = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  const roles = [
    ...normalizeRoles(claims.role),
    ...normalizeRoles(schemaRoleClaim)
  ]
    .map(r => r.trim())
    .filter(Boolean)

  return Array.from(new Set(roles))
}

export function extractUserFromClaims(claims) {
  if (!claims || typeof claims !== 'object') return { id: '', email: '', name: '' }

  const id =
    claims.sub ||
    claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
    claims.nameid ||
    ''

  const email = claims.email || claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || ''
  const name = claims.name || claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || ''

  return { id: String(id || ''), email: String(email || ''), name: String(name || '') }
}

export function hasAnyRole(roles, allowed = []) {
  if (!Array.isArray(allowed) || allowed.length === 0) return true
  const set = new Set((roles || []).map(String))
  return allowed.some(r => set.has(r))
}
