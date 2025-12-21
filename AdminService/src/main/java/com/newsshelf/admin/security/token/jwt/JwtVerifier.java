package com.newsshelf.admin.security.token.jwt;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.JwtParserBuilder;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;


public class JwtVerifier {

    private final SecretKey key;
    private final String issuer;

    public JwtVerifier(String secret, String issuer) {
        this.key = Keys.hmacShaKeyFor(secret.getBytes(StandardCharsets.UTF_8));
        this.issuer = issuer;
    }

    public Claims verifyAndGetClaims(String token) {
        JwtParserBuilder builder = Jwts.parserBuilder().setSigningKey(key);

        if (issuer != null && !issuer.isBlank()) {
            builder.requireIssuer(issuer);
        }

        return builder.build()
                .parseClaimsJws(token)
                .getBody();
    }
}
