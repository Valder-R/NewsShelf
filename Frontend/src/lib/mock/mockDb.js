import {subHours} from 'date-fns'

const now = new Date()

export const mockDb = {
  users: [
    { id: '1', email: 'user@demo.com', password: 'User123!', firstName: 'Demo', lastName: 'User', roles: ['User'] },
    { id: '2', email: 'moder@demo.com', password: 'Moder123!', firstName: 'Demo', lastName: 'Moderator', roles: ['Moderator'] },
    { id: '3', email: 'admin@demo.com', password: 'Admin123!', firstName: 'Demo', lastName: 'Admin', roles: ['Admin'] }
  ],
  news: [
    {
      id: 'n1',
      title: 'NewsShelf: демо-новина №1',
      summary: 'Це демо-стрічка новин. Увімкніть VITE_USE_MOCK=true, щоб тестувати UI без бекенду.',
      content: 'Повний текст демо-новини. Тут може бути HTML або Markdown, залежно від вашого бекенду.',
      author: 'Редакція',
      sourceUrl: 'https://example.com',
      tags: ['Tech', 'Product'],
      imageUrl: '',
      publishedAt: subHours(now, 2).toISOString(),
      likes: ['1', '3']
    },
    {
      id: 'n2',
      title: 'Персональні рекомендації та тренди',
      summary: 'Окремий блок для рекомендацій з RecService та блок "Trending".',
      content: 'Демо-контент для рекомендацій. Якщо бекенд повертає id новин — адаптуйте в services/recService.js.',
      author: 'NewsShelf Bot',
      sourceUrl: 'https://example.com',
      tags: ['AI', 'Recommendations'],
      imageUrl: '',
      publishedAt: subHours(now, 10).toISOString(),
      likes: []
    }
  ],
  comments: [
    {
      id: 'c1',
      newsId: 'n1',
      text: 'Класний дизайн. Можна додати ще фільтри по тегах.',
      createdAt: subHours(now, 1).toISOString(),
      status: 'approved',
      user: { id: '2', name: 'Demo Moderator' }
    }
  ]
}
