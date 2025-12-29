import {AppConfig} from '../config/appConfig.js'
import {createHttpClient, resolveBaseUrl} from '../lib/api/http.js'
import {MockApi} from '../lib/mock/mockApi.js'

function client(getToken) {
  return createHttpClient({ baseURL: resolveBaseUrl('user'), getToken })
}

export const authService = {
  async register(payload) {
    if (AppConfig.useMock) return (await MockApi.register(payload)).data

    const c = client()
    const { data } = await c.post('/auth/register', payload)
    return data
  },

  async login(payload) {
    if (AppConfig.useMock) return (await MockApi.login(payload)).data

    const c = client()
    const { data } = await c.post('/auth/login', payload)
    return data
  },

  async me(token) {
    if (AppConfig.useMock) return (await MockApi.me(token)).data

    const c = client(() => token)
    const { data } = await c.get('/profiles/me')
    return data
  },

  async updateProfile(payload, token) {
    if (AppConfig.useMock) {
      const me = await MockApi.me(token)
      return { ...(me.data || {}), ...(payload || {}) }
    }

    const c = client(() => token)
    const { data } = await c.put('/profiles', payload)

    return data
  }
}
