import {AppConfig} from '../config/appConfig.js'
import {createHttpClient, resolveBaseUrl} from '../lib/api/http.js'
import {MockApi} from '../lib/mock/mockApi.js'
import {newsService} from './newsService.js'

function client() {
  return createHttpClient({ baseURL: resolveBaseUrl('rec') || resolveBaseUrl('news') })
}

export const recService = {
  async recommended(userId) {
    if (AppConfig.useMock) {

      const feed = await MockApi.listNews({ q: '', sort: 'new', page: 1, pageSize: 8 })
      return feed.data.items
    }
    if (!userId) return []
    const c = client()
    const { data } = await c.get(AppConfig.endpoints.rec.recommended(userId), { params: { count: 8 } })
    return await hydrateRecommendations(data)
  },

  async trending() {
    if (AppConfig.useMock) {
      const feed = await MockApi.listNews({ q: '', sort: 'old', page: 1, pageSize: 8 })
      return feed.data.items
    }
    const c = client()
    const { data } = await c.get(AppConfig.endpoints.rec.trending, { params: { count: 8 } })
    return await hydrateRecommendations(data)
  }
}

async function hydrateRecommendations(data) {
  if (!data) return []
  const list = data.recommendations || data.items || data.news || []
  if (!Array.isArray(list) || list.length === 0) return []

  const ids = list
    .map(x => x.news_id ?? x.newsId ?? x.id)
    .filter(Boolean)

  try {
    const out = await Promise.all(ids.slice(0, 8).map(id => newsService.details(id).catch(() => null)))
    return out.filter(Boolean)
  } catch {
    return []
  }
}
