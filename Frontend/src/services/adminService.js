import {AppConfig} from '../config/appConfig.js'
import {createHttpClient, resolveBaseUrl} from '../lib/api/http.js'
import {MockApi} from '../lib/mock/mockApi.js'

function http(baseKey, getToken) {
  return createHttpClient({ baseURL: resolveBaseUrl(baseKey), getToken })
}

function newsHttp(getToken) {
  const base = resolveBaseUrl('news') || resolveBaseUrl('search')
  return createHttpClient({ baseURL: base, getToken })
}

const normalizeUsersResponse = (data) => {




  const items =
      Array.isArray(data?.items) ? data.items :
          Array.isArray(data?.users) ? data.users :
              Array.isArray(data) ? data :
                  []

  const total =
      Number.isFinite(Number(data?.total)) ? Number(data.total) :
          Number.isFinite(Number(data?.count)) ? Number(data.count) :
              items.length

  return { items, total }
}

export const adminService = {




  async listUsers({ role = '', status = '' } = {}, token) {
    if (AppConfig.useMock) {
      const mock = await MockApi.adminUsers()
      return normalizeUsersResponse(mock?.data)
    }

    const c = http('admin', () => token)
    const { data } = await c.get('/users', {
      params: {
        ...(role ? { role } : {}),
        ...(status ? { status } : {})
      }
    })

    return normalizeUsersResponse(data)
  },

  async assignRole(userId, role, token) {
    if (AppConfig.useMock) return true
    const c = http('admin', () => token)
    await c.patch(`/users/${userId}/role`, { role })
    return true
  },

  async deleteUser(userId, token) {
    if (AppConfig.useMock) return true
    const c = http('admin', () => token)
    await c.delete(`/users/${userId}`)
    return true
  },


  async deletePost(postId, token) {
    if (AppConfig.useMock) return true
    const c = http('admin', () => token)
    await c.delete(`/posts/${postId}`)
    return true
  },


  async deleteComment(commentId, token) {
    if (AppConfig.useMock) return true
    const c = http('admin', () => token)
    await c.delete(`/comments/${commentId}`)
    return true
  },




  async newsCategories(token) {
    if (AppConfig.useMock) {
      return [
        { name: 'Other', value: 0 },
        { name: 'Politics', value: 1 },
        { name: 'Economy', value: 2 }
      ]
    }
    const c = newsHttp(() => token)
    const { data } = await c.get(AppConfig.endpoints.news.categories)
    return Array.isArray(data) ? data : []
  },

  async newsList(token) {
    if (AppConfig.useMock) {
      const feed = await MockApi.listNews({ q: '', sort: 'new', page: 1, pageSize: 50 })
      return feed.data.items || []
    }
    const c = newsHttp(() => token)
    const { data } = await c.get(AppConfig.endpoints.news.list)
    return Array.isArray(data) ? data : (data?.items || [])
  },

  async newsByAuthor(author, token) {
    if (AppConfig.useMock) {
      const feed = await MockApi.listNews({ q: '', sort: 'new', page: 1, pageSize: 200 })
      const items = feed.data.items || []
      const a = String(author || '').trim()
      return items.filter(n => String(n.author || '').trim() === a)
    }
    const c = newsHttp(() => token)
    const { data } = await c.get(AppConfig.endpoints.news.searchByAuthor, { params: { author } })
    return Array.isArray(data) ? data : (data?.items || [])
  },



  async newsCreate(payload, token) {
    if (AppConfig.useMock) return { ...payload, id: String(Date.now()) }

    const fd = new FormData()
    fd.append('Title', payload?.title ?? '')
    fd.append('Content', payload?.content ?? '')
    fd.append('Author', payload?.author ?? '')
    fd.append('Category', String(payload?.category ?? 0))

    for (const file of (payload?.images || [])) {
      fd.append('Images', file)
    }

    const c = newsHttp(() => token)
    const { data } = await c.post(AppConfig.endpoints.news.list, fd, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    return data
  },

  async newsUpdate(id, payload, token) {
    if (AppConfig.useMock) return { id, ...payload }

    const body = {
      title: payload?.title ?? '',
      content: payload?.content ?? '',
      author: payload?.author ?? '',
      category: payload?.category ?? 0,
      imageUrls: payload?.imageUrls ?? []
    }

    const c = newsHttp(() => token)
    const res = await c.put(AppConfig.endpoints.news.details(id), body)

    const data = res?.data
    if (data && typeof data === 'object') return data
    return { id, ...body }
  },

  async newsDelete(id, token) {
    if (AppConfig.useMock) return true
    const c = newsHttp(() => token)
    await c.delete(AppConfig.endpoints.news.details(id))
    return true
  }
}
