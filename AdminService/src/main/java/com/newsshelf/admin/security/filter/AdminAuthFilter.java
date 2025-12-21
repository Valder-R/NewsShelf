package com.newsshelf.admin.security.filter;

import com.newsshelf.admin.security.principal.AdminPrincipal;
import com.newsshelf.admin.security.token.AuthPayload;
import com.newsshelf.admin.security.token.TokenAuthService;
import com.newsshelf.admin.security.token.TokenExtractor;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.jspecify.annotations.NonNull;
import org.springframework.security.authentication.AnonymousAuthenticationToken;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
public class AdminAuthFilter extends OncePerRequestFilter {

    private final TokenAuthService tokenAuthService;

    @Override
    protected void doFilterInternal(
            @NonNull HttpServletRequest request,
            @NonNull HttpServletResponse response,
            @NonNull FilterChain filterChain
    ) throws ServletException, IOException {

        String path = request.getRequestURI();
        String method = request.getMethod();

        var existing = SecurityContextHolder.getContext().getAuthentication();

        if (existing != null
                && existing.isAuthenticated()
                && !(existing instanceof AnonymousAuthenticationToken)) {
            log.debug("Auth already present (non-anonymous), skipping token auth. {} {}", method, path);
            filterChain.doFilter(request, response);
            return;
        }

        var tokenOpt = TokenExtractor.extractBearer(request);
        if (tokenOpt.isEmpty()) {
            log.debug("No Bearer token found. {} {}", method, path);
            filterChain.doFilter(request, response);
            return;
        }

        String rawToken = tokenOpt.get();
        log.debug("Bearer token found (len={}). {} {}", rawToken.length(), method, path);

        try {
            AuthPayload payload = tokenAuthService.authenticate(rawToken);

            var authorities = payload.roles().stream()
                    .map(r -> "ROLE_" + r)
                    .map(SimpleGrantedAuthority::new)
                    .collect(Collectors.toSet());

            var principal = new AdminPrincipal(payload.userId(), payload.roles());

            var auth = UsernamePasswordAuthenticationToken.authenticated(principal, null, authorities);

            auth.setDetails(rawToken);

            SecurityContextHolder.getContext().setAuthentication(auth);

            log.info("Authenticated admin userId={} roles={} authorities={}",
                    payload.userId(), payload.roles(), authorities);

            filterChain.doFilter(request, response);

        } catch (RuntimeException ex) {
            log.warn("Authentication failed. {} {} -> {}", method, path, ex.getMessage());
            throw ex;
        }
    }
}
