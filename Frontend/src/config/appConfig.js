
const env = import.meta.env;

const toBool = (v, def = false) => {
    if (v === undefined || v === null || v === '') return def;
    return String(v).trim().toLowerCase() === 'true';
};

const normalizeBase = (url) => (url ? String(url).replace(/\/+$/, '') : '');
const ensureLeadingSlash = (p) => (p?.startsWith('/') ? p : `/${p || ''}`);
const joinUrl = (base, path) => {
    const b = normalizeBase(base);
    const p = ensureLeadingSlash(path);
    return b ? `${b}${p}` : p;
};

const gateway = normalizeBase(env.VITE_API_GATEWAY) || 'http://localhost:5000';

const defaults = {
    user: joinUrl(gateway, '/api/user'),
    news: joinUrl(gateway, '/api/search'),
    search: joinUrl(gateway, '/api/search'),
    admin: joinUrl(gateway, '/api/admin'),
    rec: joinUrl(gateway, '/api/recommendations')
};

const services = {
    user: normalizeBase(env.VITE_USER_API) || defaults.user,
    news: normalizeBase(env.VITE_NEWS_API) || defaults.news,
    search: normalizeBase(env.VITE_SEARCH_API) || defaults.search,
    admin: normalizeBase(env.VITE_ADMIN_API) || defaults.admin,
    rec: normalizeBase(env.VITE_REC_API) || defaults.rec
};

export const AppConfig = {
    useMock: toBool(env.VITE_USE_MOCK, false),
    gatewayBaseUrl: gateway,
    services,

    url: {
        user: (path) => joinUrl(services.user, path),
        news: (path) => joinUrl(services.news, path),
        search: (path) => joinUrl(services.search, path),
        admin: (path) => joinUrl(services.admin, path),
        rec: (path) => joinUrl(services.rec, path)
    },

    endpoints: {
        auth: {
            register: '/auth/register',
            login: '/auth/login',
            external: '/auth/external'
        },
        profile: {
            me: '/profiles/me',
            update: '/profiles'
        },

        news: {
            list: '/News',
            details: (id) => `/News/${id}`,
            categories: '/News/categories',
            search: '/News/search',
            searchByText: '/News/search/by-text',
            searchByAuthor: '/News/search/by-author',
            searchByCategory: '/News/search/by-category',
            searchByDateRange: '/News/search/by-date-range'
        },

        rec: {
            trending: '/recommendations/popular/news',
            recommended: (userId) => `/recommendations/${userId}`
        },

        admin: {
            listUsers: '/users',
            assignRole: (userId) => `/users/${userId}/role`,
            deleteUser: (userId) => `/users/${userId}`,
            deleteComment: (commentId) => `/comments/${commentId}`,
            deletePost: (postId) => `/posts/${postId}`
        }
    }
};
