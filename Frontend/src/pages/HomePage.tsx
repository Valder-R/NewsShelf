import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';

export default function HomePage() {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  return (
    <div className="space-y-12">
      {/* Hero Section */}
      <section className="text-center py-12 bg-gradient-to-r from-blue-500 to-blue-600 text-white rounded-lg">
        <h1 className="text-4xl font-bold mb-4">Welcome to NewsShelf</h1>
        <p className="text-xl mb-8">Your personalized news recommendation platform powered by AI</p>

        {!isAuthenticated && (
          <div className="flex gap-4 justify-center">
            <Link to="/register" className="btn-primary !bg-white !text-blue-600 hover:!bg-gray-100">
              Get Started
            </Link>
            <Link to="/login" className="btn-primary !bg-transparent !border-2 !border-white">
              Sign In
            </Link>
          </div>
        )}

        {isAuthenticated && (
          <div className="flex gap-4 justify-center">
            <Link to="/search" className="btn-primary">
              Search News
            </Link>
            <Link to="/recommendations" className="btn-primary">
              View Recommendations
            </Link>
          </div>
        )}
      </section>

      {/* Features Section */}
      <section className="grid md:grid-cols-3 gap-6">
        <div className="card">
          <h3 className="text-lg font-bold mb-2">🤖 AI-Powered</h3>
          <p className="text-gray-600">
            Get personalized recommendations based on your reading history using advanced machine learning.
          </p>
        </div>

        <div className="card">
          <h3 className="text-lg font-bold mb-2">🔍 Smart Search</h3>
          <p className="text-gray-600">
            Search news by keywords, authors, categories, and date ranges with advanced filtering.
          </p>
        </div>

        <div className="card">
          <h3 className="text-lg font-bold mb-2">⭐ Curated Content</h3>
          <p className="text-gray-600">
            Discover popular news stories and trending topics from around the world.
          </p>
        </div>
      </section>

      {/* Stats Section */}
      <section className="text-center py-8">
        <h2 className="text-3xl font-bold mb-8">By The Numbers</h2>
        <div className="grid md:grid-cols-3 gap-8">
          <div>
            <div className="text-4xl font-bold text-blue-600">63,500+</div>
            <p className="text-gray-600">News Articles</p>
          </div>
          <div>
            <div className="text-4xl font-bold text-blue-600">50+</div>
            <p className="text-gray-600">Categories</p>
          </div>
          <div>
            <div className="text-4xl font-bold text-blue-600">100%</div>
            <p className="text-gray-600">Personalized</p>
          </div>
        </div>
      </section>
    </div>
  );
}
