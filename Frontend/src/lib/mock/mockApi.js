import {mockDb} from './mockDb.js'

function sleep(ms){ return new Promise(r => setTimeout(r, ms)) }

function ok(data){ return { data } }
function bad(status, data){ throw { status, data, message: data?.message || 'Mock error' } }

export const MockApi = {
  async register({ email, password, displayName }) {
    await sleep(350)
    if (mockDb.users.some(u => u.email === email)) return bad(409, { message: 'Email already exists' })
    const id = String(Date.now())
    const user = { id, email, displayName: displayName || email, roles: ['User'] }
    mockDb.users.push({ ...user, password })
    return ok({ token: `mock.${id}.token` })
  },

  async login({ email, password }) {
    await sleep(350)
    const u = mockDb.users.find(x => x.email === email && x.password === password)
    if (!u) return bad(401, { message: 'Invalid credentials' })
    return ok({ token: `mock.${u.id}.token` })
  },

  async me(token) {
    await sleep(200)
    const parts = String(token || '').split('.')
    const id = parts.length >= 2 ? parts[1] : ''
    const u = mockDb.users.find(x => x.id === id)
    if (!u) return bad(401, { message: 'Unauthorized' })
    const { password, ...safe } = u
    return ok(safe)
  },

  async listNews({ q, sort, page, pageSize }) {
    await sleep(250)
    let items = [...mockDb.news]
    if (q) {
      const qq = q.toLowerCase()
      items = items.filter(n => (n.title + ' ' + n.summary).toLowerCase().includes(qq))
    }
    if (sort === 'old') items.sort((a,b) => new Date(a.publishedAt) - new Date(b.publishedAt))
    if (sort === 'new') items.sort((a,b) => new Date(b.publishedAt) - new Date(a.publishedAt))
    const total = items.length
    const start = (page-1) * pageSize
    const paged = items.slice(start, start + pageSize)
    return ok({ items: paged, total, page, pageSize })
  },

  async newsDetails(id) {
    await sleep(200)
    const n = mockDb.news.find(x => x.id === id)
    if (!n) return bad(404, { message: 'Not found' })
    return ok(n)
  },

  async toggleLike({ id, userId, like }) {
    await sleep(180)
    const n = mockDb.news.find(x => x.id === id)
    if (!n) return bad(404, { message: 'Not found' })
    n.likes = n.likes || []
    const has = n.likes.includes(userId)
    if (like && !has) n.likes.push(userId)
    if (!like && has) n.likes = n.likes.filter(x => x !== userId)
    return ok({ likesCount: n.likes.length, liked: like })
  },

  async listComments(newsId) {
    await sleep(180)
    return ok(mockDb.comments.filter(c => c.newsId === newsId))
  },

  async addComment({ newsId, user, text }) {
    await sleep(250)
    const c = {
      id: String(Date.now()),
      newsId,
      text,
      createdAt: new Date().toISOString(),
      status: 'approved',
      user: { id: user.id, name: user.displayName || user.email }
    }
    mockDb.comments.unshift(c)
    return ok(c)
  },

  async adminUsers() {
    await sleep(250)
    return ok(mockDb.users.map(({ password, ...u }) => u))
  },

  async adminSetRoles({ userId, roles }) {
    await sleep(250)
    const u = mockDb.users.find(x => x.id === userId)
    if (!u) return bad(404, { message: 'User not found' })
    u.roles = roles
    const { password, ...safe } = u
    return ok(safe)
  },

  async adminHealth() {
    await sleep(180)
    return ok({ status: 'ok', services: ['user', 'news', 'search', 'admin', 'rec'], ts: new Date().toISOString() })
  }
}
