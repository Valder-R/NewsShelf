export interface User {
  id: string;
  email: string;
  displayName: string;
  bio?: string;
}

export interface AuthResponse {
  accessToken: string;
  user: User;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
  favoriteTopics?: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface News {
  id: number;
  title: string;
  content: string;
  category: string;
  author?: string;
  imageUrls: string[];
  createdAt: string;
}

export interface Recommendation {
  news_id: number;
  title: string;
  category: string;
  short_description: string;
  authors: string;
  similarity_score: number;
  link: string;
}

export interface RecommendationsResponse {
  user_id: number;
  recommendations: Recommendation[];
  total_count: number;
}

export interface UserInterests {
  user_id: number;
  interests: Record<string, number>;
  total_activities: number;
}
