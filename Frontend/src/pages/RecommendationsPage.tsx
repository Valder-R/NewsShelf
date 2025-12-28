import { useState, useEffect } from 'react';
import api from '@/services/api';
import { Recommendation } from '@/types';

export default function RecommendationsPage() {
  const [recommendations, setRecommendations] = useState<Recommendation[]>([]);
  const [interests, setInterests] = useState<Record<string, number>>({});
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const userId = localStorage.getItem('userId') || '1';

  useEffect(() => {
    const fetchRecommendations = async () => {
      setIsLoading(true);
      setError(null);
      try {
        // Fetch recommendations
        const recResponse = await api.get(`/recommendations/${userId}`);
        setRecommendations(recResponse.data.recommendations || []);

        // Fetch interests
        const interestsResponse = await api.get(`/recommendations/${userId}/interests`);
        setInterests(interestsResponse.data.interests || {});
      } catch (err: any) {
        setError(err.response?.data?.message || 'Failed to load recommendations');
      } finally {
        setIsLoading(false);
      }
    };

    fetchRecommendations();
  }, [userId]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p className="mt-4 text-gray-600">Loading recommendations...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold mb-2">Your Recommendations</h1>
        <p className="text-gray-600">Personalized news articles based on your reading history</p>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {/* Interests Section */}
      {Object.keys(interests).length > 0 && (
        <div className="card">
          <h2 className="text-xl font-bold mb-4">Your Interests</h2>
          <div className="flex flex-wrap gap-2">
            {Object.entries(interests)
              .sort((a, b) => b[1] - a[1])
              .slice(0, 5)
              .map(([category, weight]) => (
                <div
                  key={category}
                  className="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm font-medium"
                >
                  {category} ({Math.round(weight * 100)}%)
                </div>
              ))}
          </div>
        </div>
      )}

      {/* Recommendations */}
      <div className="space-y-4">
        <h2 className="text-xl font-bold">Recommended Articles</h2>

        {recommendations.length === 0 ? (
          <div className="text-center py-8 text-gray-600">
            <p>No recommendations yet. Read some articles to get started!</p>
          </div>
        ) : (
          <div className="grid gap-4">
            {recommendations.map((rec) => (
              <div key={rec.news_id} className="card">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded text-sm font-medium">
                        {rec.category}
                      </span>
                      <span className="text-green-600 font-bold">
                        {(rec.similarity_score * 100).toFixed(1)}% match
                      </span>
                    </div>
                    <h3 className="text-lg font-bold mb-2">{rec.title}</h3>
                    <p className="text-gray-600 mb-3">{rec.short_description}</p>
                    <p className="text-sm text-gray-500 mb-3">By {rec.authors}</p>
                  </div>
                </div>
                {rec.link && (
                  <a
                    href={rec.link}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-blue-600 hover:underline font-medium"
                  >
                    Read Full Article →
                  </a>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
