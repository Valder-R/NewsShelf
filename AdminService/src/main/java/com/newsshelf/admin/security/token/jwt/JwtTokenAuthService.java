package com.newsshelf.admin.security.token.jwt;

import com.newsshelf.admin.security.token.AuthPayload;
import com.newsshelf.admin.security.token.TokenAuthService;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.JwtException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.web.server.ResponseStatusException;

import java.util.Set;

@Slf4j
@RequiredArgsConstructor
public class JwtTokenAuthService implements TokenAuthService {

    private final JwtVerifier verifier;
    private final JwtClaimsParser claimsParser;

    @Override
    public AuthPayload authenticate(String rawToken) {
        try {
            Claims claims = verifier.verifyAndGetClaims(rawToken);

            String userId = claimsParser.extractUserId(claims);
            if (userId == null || userId.isBlank()) {
                log.debug("JWT missing subject (sub)");
                throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "JWT missing subject (sub)");
            }

            Set<String> roles = claimsParser.extractRoles(claims);

            log.debug("JWT verified. sub={} roles={}", userId, roles);
            return new AuthPayload(userId, roles);

        } catch (ExpiredJwtException e) {
            log.info("JWT expired");
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "JWT expired");
        } catch (JwtException e) {
            log.info("JWT invalid: {}", e.getMessage());
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "JWT invalid");
        }
    }
}
