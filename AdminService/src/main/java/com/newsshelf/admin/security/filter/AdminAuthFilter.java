package com.newsshelf.admin.security.filter;

import com.newsshelf.admin.security.model.AdminAuthentication;
import com.newsshelf.admin.security.model.AdminPrincipal;
import com.newsshelf.admin.security.token.validation.TokenValidationService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

@RequiredArgsConstructor
public class AdminAuthFilter extends OncePerRequestFilter {

    private final TokenValidationService tokenValidationService;

    @Override
    protected void doFilterInternal(
            HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain
    ) throws ServletException, IOException {

        String token = extractBearer(request.getHeader(HttpHeaders.AUTHORIZATION));

        if (token != null && !token.isBlank()) {
            AdminPrincipal principal = tokenValidationService.validate(token);
            SecurityContextHolder
                    .getContext()
                    .setAuthentication(new AdminAuthentication(principal));
        }

        filterChain.doFilter(request, response);
    }

    private String extractBearer(String header) {
        if (header == null || header.isBlank()) return null;
        String prefix = "Bearer ";
        return header.startsWith(prefix) ? header.substring(prefix.length()) : header;
    }
}