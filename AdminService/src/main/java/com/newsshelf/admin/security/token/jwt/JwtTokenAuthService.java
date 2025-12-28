package com.newsshelf.admin.security.token.jwt;

import com.newsshelf.admin.security.token.AuthPayload;
import com.newsshelf.admin.security.token.TokenAuthService;
import io.jsonwebtoken.Claims;
import lombok.RequiredArgsConstructor;

@RequiredArgsConstructor
public class JwtTokenAuthService implements TokenAuthService {

    private final JwtVerifier verifier;
    private final JwtClaimsParser parser;

    @Override
    public AuthPayload authenticate(String rawToken) {
        Claims claims = verifier.verify(rawToken);

        String userId = parser.userId(claims);
        var roles = parser.roles(claims);

        return new AuthPayload(userId, roles);
    }
}
