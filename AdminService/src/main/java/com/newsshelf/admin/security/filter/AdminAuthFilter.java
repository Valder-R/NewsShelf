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

        if (SecurityContextHolder.getContext().getAuthentication() != null) {
            filterChain.doFilter(request, response);
            return;
        }

        var tokenOpt = TokenExtractor.extractBearer(request);
        if (tokenOpt.isEmpty()) {
            filterChain.doFilter(request, response);
            return;
        }

        String rawToken = tokenOpt.get();

        try {
            AuthPayload payload = tokenAuthService.authenticate(rawToken);

            var authorities = payload.roles().stream()
                    .map(r -> new SimpleGrantedAuthority("ROLE_" + r.name()))
                    .collect(Collectors.toSet());

            var principal = new AdminPrincipal(payload.userId(), payload.roles());

            var auth = new UsernamePasswordAuthenticationToken(principal, null, authorities);
            SecurityContextHolder.getContext().setAuthentication(auth);

            log.debug("AUTH OK userId={} roles={} method={} path={}",
                    payload.userId(), payload.roles(), method, path);

            filterChain.doFilter(request, response);

        } catch (RuntimeException ex) {
            log.warn("AUTH FAIL method={} path={} reason={}", method, path, ex.getMessage());
            throw ex;
        }
    }
}
