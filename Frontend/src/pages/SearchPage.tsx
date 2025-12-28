import { useState } from 'react';
import api from '@/services/api';
import { News } from '@/types';

export default function SearchPage() {
  const [query, setQuery] = useState('');
  const [searchResults, setSearchResults] = useState<News[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.get('/news/search/by-text', {
        params: {
          query,
        },
      });
      setSearchResults(response.data || []);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Search failed');
      setSearchResults([]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold mb-2">Search News</h1>
        <p className="text-gray-600">Find articles by keywords, authors, categories, and dates</p>
      </div>

      {/* Search Form */}
      <form onSubmit={handleSearch} className="card">
        <div className="flex gap-2">
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Search articles..."
            className="input-field flex-1"
          />
          <button type="submit" className="btn-primary" disabled={isLoading}>
            {isLoading ? 'Searching...' : 'Search'}
          </button>
        </div>
      </form>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {/* Results */}
      <div>
        {searchResults.length === 0 && !isLoading && query && (
          <div className="text-center py-8 text-gray-600">
            <p>No results found for "{query}"</p>
          </div>
        )}

        {searchResults.length > 0 && (
          <div>
            <h2 className="text-xl font-bold mb-4">
              Found {searchResults.length} article{searchResults.length !== 1 ? 's' : ''}
            </h2>
            <div className="grid gap-4">
              {searchResults.map((news) => (
                <div key={news.id} className="card">
                  <div className="flex gap-4">
                    {news.imageUrls.length > 0 && (
                      <img
                        src={news.imageUrls[0]}
                        alt={news.title}
                        className="w-32 h-32 object-cover rounded"
                        onError={(e) => {
                          (e.target as HTMLImageElement).style.display = 'none';
                        }}
                      />
                    )}
                    <div className="flex-1">
                      <h3 className="text-lg font-bold mb-2">{news.title}</h3>
                      <p className="text-gray-600 mb-2 line-clamp-2">{news.content}</p>
                      <div className="flex justify-between items-center text-sm text-gray-500">
                        <span>{news.category}</span>
                        <span>{news.author}</span>
                        <span>
                          {new Date(news.createdAt).toLocaleDateString()}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
