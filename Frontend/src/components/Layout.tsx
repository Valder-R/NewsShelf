import { Outlet, Link } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';

export default function Layout() {
  const { isAuthenticated, logout } = useAuthStore();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow">
        <nav className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <Link to="/" className="text-2xl font-bold text-blue-600">
            📰 NewsShelf
          </Link>

          <div className="flex gap-4 items-center">
            <Link to="/" className="hover:text-blue-600 transition">
              Home
            </Link>

            {isAuthenticated ? (
              <>
                <Link to="/search" className="hover:text-blue-600 transition">
                  Search
                </Link>
                <Link to="/recommendations" className="hover:text-blue-600 transition">
                  Recommendations
                </Link>
                <Link to="/profile" className="hover:text-blue-600 transition">
                  Profile
                </Link>
                <button
                  onClick={logout}
                  className="btn-primary !bg-red-600 hover:!bg-red-700"
                >
                  Logout
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="btn-secondary">
                  Login
                </Link>
                <Link to="/register" className="btn-primary">
                  Register
                </Link>
              </>
            )}
          </div>
        </nav>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="bg-gray-800 text-white mt-16 py-8">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <p>© 2024 NewsShelf. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
}
