import React from 'react'
import {newsService} from '../services/newsService.js'

function normalizeTopicList(raw) {
  const arr = Array.isArray(raw) ? raw : []

  const names = arr
    .map(x => (typeof x === 'string' ? x : (x?.name || x?.Name || x?.title || x?.Title)))
    .filter(Boolean)
    .map(s => String(s))

  return Array.from(new Set(names)).sort((a, b) => a.localeCompare(b))
}

export function useTopicOptions() {
  const [loading, setLoading] = React.useState(true)
  const [topics, setTopics] = React.useState([])
  const [error, setError] = React.useState(null)

  React.useEffect(() => {
    let mounted = true
    ;(async () => {
      setLoading(true)
      setError(null)
      try {
        const data = await newsService.categories()
        const list = normalizeTopicList(data)
        if (mounted) setTopics(list)
      } catch (e) {
        if (mounted) setError(e)
      } finally {
        if (mounted) setLoading(false)
      }
    })()
    return () => { mounted = false }
  }, [])

  return { loading, topics, error }
}
