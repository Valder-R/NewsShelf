import {AppConfig} from '../config/appConfig.js'
import {createHttpClient, resolveBaseUrl} from '../lib/api/http.js'
import {MockApi} from '../lib/mock/mockApi.js'

function client() {
  return createHttpClient({ baseURL: resolveBaseUrl('search') || resolveBaseUrl('news') })
}

function sortItems(items, sort) {
  const arr = Array.isArray(items) ? [...items] : []
  if (sort === 'old') arr.sort((a, b) => new Date(a.publishedAt || 0) - new Date(b.publishedAt || 0))
  else if (sort === 'new') arr.sort((a, b) => new Date(b.publishedAt || 0) - new Date(a.publishedAt || 0))
  return arr
}

function pageItems(items, page, pageSize) {
  const p = Math.max(1, Number(page) || 1)
  const ps = Math.max(1, Number(pageSize) || 10)
  const start = (p - 1) * ps
  return { items: items.slice(start, start + ps), total: items.length, page: p, pageSize: ps }
}

export const searchService = {
  async search({ q = '', sort = 'new', page = 1, pageSize = 12, userId = null } = {}) {
    if (AppConfig.useMock) return (await MockApi.listNews({ q, sort, page, pageSize })).data

    const c = client()
    let data

    if (q && String(q).trim()) {

      try {
        const { data: d } = await c.get(AppConfig.endpoints.news.searchByText, {
          params: { query: q, sortBy: 'publishedAt', sortDesc: sort !== 'old', userId }
        })
        data = d
      } catch {
        const { data: d } = await c.get(AppConfig.endpoints.search.search, {
          params: { query: q, sortBy: 'publishedAt', sortDesc: sort !== 'old', userId }
        })
        data = d
      }
    } else {
      const { data: d } = await c.get(AppConfig.endpoints.news.list)
      data = d
    }

    const items = sortItems(Array.isArray(data) ? data : (data?.items || []), sort)
    return pageItems(items, page, pageSize)
  }
}
