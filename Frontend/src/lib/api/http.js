import axios from 'axios'
import {AppConfig} from '../../config/appConfig.js'

export function createHttpClient({ baseURL, getToken }) {
  const client = axios.create({
    baseURL,
    timeout: 20000,
    headers: { 'Content-Type': 'application/json' }
  })

  client.interceptors.request.use((config) => {
    const token = getToken?.()
    if (token) {

      config.headers.Authorization = `Bearer ${token}`
      config.headers.Token = token
    }
    return config
  })



  return client
}

export function resolveBaseUrl(serviceKey) {
  const aliases = {
    recommendations: 'rec',
    news: 'search'
  }
  const key = aliases[serviceKey] || serviceKey
  return AppConfig.services?.[key] || AppConfig.gatewayBaseUrl || ''
}



