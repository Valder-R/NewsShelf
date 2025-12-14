package com.newsshelf.admin.security.model;

import org.springframework.security.authentication.AbstractAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;

import java.util.stream.Collectors;

public class AdminAuthentication extends AbstractAuthenticationToken {

    private final AdminPrincipal principal;

    public AdminAuthentication(AdminPrincipal principal) {
        super(principal.permissions()
                .stream()
                .map(SimpleGrantedAuthority::new)
                .collect(Collectors.toSet()));
        this.principal = principal;
        setAuthenticated(true);
    }

    @Override
    public Object getCredentials() {
        return null;
    }

    @Override
    public AdminPrincipal getPrincipal() {
        return principal;
    }
}
