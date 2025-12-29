import {AppConfig} from '../config/appConfig.js'
import {createHttpClient, resolveBaseUrl} from '../lib/api/http.js'
import {MockApi} from '../lib/mock/mockApi.js'

function client(getToken) {
  return createHttpClient({ baseURL: resolveBaseUrl('news') || resolveBaseUrl('search'), getToken })
}

function sortItems(items, sort) {
  const arr = Array.isArray(items) ? [...items] : []
  if (sort === 'old') {
    arr.sort((a, b) => new Date(a.publishedAt || 0) - new Date(b.publishedAt || 0))
  } else if (sort === 'new') {
    arr.sort((a, b) => new Date(b.publishedAt || 0) - new Date(a.publishedAt || 0))
  }
  return arr
}

function pageItems(items, page, pageSize) {
  const p = Math.max(1, Number(page) || 1)
  const ps = Math.max(1, Number(pageSize) || 10)
  const start = (p - 1) * ps
  return {
    items: items.slice(start, start + ps),
    total: items.length,
    page: p,
    pageSize: ps
  }
}

export function normalizeNewsImageUrl(url) {
  if (!url) return ''
  const s = String(url)
  if (s.includes('/api/search/')) return s
  const base = `${AppConfig.services?.news || ''} ${AppConfig.services?.search || ''}`
  const behindGateway = base.includes('/api/search')
  if (!behindGateway) return s



  try {
    const u = new URL(s, window.location.origin)
    if (u.pathname.startsWith('/uploads/news/')) {
      u.pathname = `/api/search${u.pathname}`
      return u.toString()
    }
    return u.toString()
  } catch {

    if (s.startsWith('/uploads/news/')) return `/api/search${s}`
    return s
  }
}

export const newsService = {

  async list({ q = '', sort = 'new', page = 1, pageSize = 8, userId = null } = {}) {
    if (AppConfig.useMock) return (await MockApi.listNews({ q, sort, page, pageSize })).data

    const c = client()
    let data

    if (q && String(q).trim()) {
      const { data: d } = await c.get(AppConfig.endpoints.news.searchByText, {
        params: { query: q, sortBy: sort === 'old' ? 'publishedAt' : 'publishedAt', sortDesc: sort !== 'old', userId }
      })
      data = d
    } else {
      const { data: d } = await c.get(AppConfig.endpoints.news.list)
      data = d
    }

    const items = sortItems(Array.isArray(data) ? data : (data?.items || []), sort)
    return pageItems(items, page, pageSize)
  },

  async details(id) {
    if (AppConfig.useMock) return (await MockApi.newsDetails(id)).data
    const c = client()
    const { data } = await c.get(AppConfig.endpoints.news.details(id))
    return data
  },

  async categories() {
    if (AppConfig.useMock) return []
    const c = client()
    const { data } = await c.get(AppConfig.endpoints.news.categories)
    return Array.isArray(data) ? data : []
  },

  async comments(id) {
    if (AppConfig.useMock) return (await MockApi.listComments(id)).data
    try {
      const c = client()
      const { data } = await c.get(`/News/${id}/comments`)
      return Array.isArray(data) ? data : (data?.items || [])
    } catch {
      return []
    }
  },

  async addComment(id, payload, token) {
    if (AppConfig.useMock) {
      const me = await MockApi.me(token)
      return (await MockApi.addComment({ newsId: id, user: me.data, text: payload.text })).data
    }
    const c = client(() => token)
    const { data } = await c.post(`/News/${id}/comments`, payload)
    return data
  },

  async setLike(id, like, token) {
    if (AppConfig.useMock) {
      const me = await MockApi.me(token)
      return (await MockApi.toggleLike({ id, like, userId: me.data.id })).data
    }
    const c = client(() => token)

    try {
      if (like) {
        const { data } = await c.post(`/News/${id}/like`)
        return data
      } else {
        const { data } = await c.delete(`/News/${id}/like`)
        return data
      }
    } catch {
      const { data } = await c.post(`/News/${id}/like`, { like })
      return data
    }
  }
}
