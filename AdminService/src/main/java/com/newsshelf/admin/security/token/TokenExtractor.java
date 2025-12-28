package com.newsshelf.admin.security.token;

import jakarta.servlet.http.HttpServletRequest;

import java.util.Optional;


public final class TokenExtractor {

    private static final String AUTH_HEADER = "Authorization";
    private static final String BEARER = "Bearer ";

    private TokenExtractor() {
    }

    public static Optional<String> extractBearer(HttpServletRequest request) {
        String header = request.getHeader(AUTH_HEADER);
        if (header == null || header.isBlank()) return Optional.empty();

        if (!header.startsWith(BEARER)) return Optional.empty();

        String token = header.substring(BEARER.length()).trim();
        return token.isEmpty() ? Optional.empty() : Optional.of(token);
    }
}
