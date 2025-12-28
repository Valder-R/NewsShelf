package com.newsshelf.admin.security.token;


public interface TokenAuthService {
    AuthPayload authenticate(String rawToken);
}
