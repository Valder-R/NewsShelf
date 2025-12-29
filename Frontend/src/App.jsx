import React from 'react'
import {BrowserRouter, Navigate, Route, Routes} from 'react-router-dom'

import {AppNavbar} from './ui/layout/AppNavbar.jsx'
import {AppFooter} from './ui/layout/AppFooter.jsx'
import {ProtectedRoute} from './ui/routes/ProtectedRoute.jsx'
import {RoleRoute} from './ui/routes/RoleRoute.jsx'

import Home from './ui/pages/Home.jsx'
import Search from './ui/pages/Search.jsx'
import NewsDetails from './ui/pages/NewsDetails.jsx'
import Login from './ui/pages/auth/Login.jsx'
import Register from './ui/pages/auth/Register.jsx'
import Profile from './ui/pages/Profile.jsx'

import AdminLayout from './ui/pages/admin/AdminLayout.jsx'
import AdminDashboard from './ui/pages/admin/AdminDashboard.jsx'
import AdminUsers from './ui/pages/admin/AdminUsers.jsx'
import AdminNews from './ui/pages/admin/AdminNews.jsx'
import AdminSystemStatus from './ui/pages/admin/AdminSystemStatus.jsx'
import AdminPublishers from './ui/pages/admin/AdminPublishers.jsx'
import AdminPublisherNews from './ui/pages/admin/AdminPublisherNews.jsx'

import PublisherLayout from './ui/pages/publisher/PublisherLayout.jsx'
import PublisherDashboard from './ui/pages/publisher/PublisherDashboard.jsx'
import PublisherNews from './ui/pages/publisher/PublisherNews.jsx'

export default function App() {
  return (
    <BrowserRouter>
      <AppNavbar />
      <main className="container py-4">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/search" element={<Search />} />
          <Route path="/news/:id" element={<NewsDetails />} />

          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <Profile />
              </ProtectedRoute>
            }
          />

          <Route
            path="/admin"
            element={
              <RoleRoute allowed={['ADMIN']}>
                <AdminLayout />
              </RoleRoute>
            }
          >
            <Route index element={<AdminDashboard />} />
            <Route path="news" element={<RoleRoute allowed={['ADMIN']}><AdminNews /></RoleRoute>} />
            <Route path="users" element={<RoleRoute allowed={['ADMIN']}><AdminUsers /></RoleRoute>} />
            <Route path="publishers" element={<RoleRoute allowed={['ADMIN']}><AdminPublishers /></RoleRoute>} />
            <Route path="publishers/:userId" element={<RoleRoute allowed={['ADMIN']}><AdminPublisherNews /></RoleRoute>} />
            <Route path="status" element={<RoleRoute allowed={['ADMIN']}><AdminSystemStatus /></RoleRoute>} />
          </Route>

          <Route
            path="/publisher"
            element={
              <RoleRoute allowed={['PUBLISHER']}>
                <PublisherLayout />
              </RoleRoute>
            }
          >
            <Route index element={<PublisherDashboard />} />
            <Route path="news" element={<RoleRoute allowed={['PUBLISHER']}><PublisherNews /></RoleRoute>} />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
      <AppFooter />
    </BrowserRouter>
  )
}
