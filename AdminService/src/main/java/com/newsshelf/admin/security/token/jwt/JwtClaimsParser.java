package com.newsshelf.admin.security.token.jwt;

import com.newsshelf.admin.security.role.Role;
import io.jsonwebtoken.Claims;

import java.util.*;

public class JwtClaimsParser {

    private final List<String> roleClaimKeys;

    public JwtClaimsParser(String rolesClaim, String aliasesCsv) {
        List<String> keys = new ArrayList<>();

        if (rolesClaim != null && !rolesClaim.isBlank()) {
            keys.add(rolesClaim.trim());
        }

        if (aliasesCsv != null && !aliasesCsv.isBlank()) {
            keys.addAll(Arrays.stream(aliasesCsv.split(","))
                    .map(String::trim)
                    .filter(s -> !s.isBlank())
                    .toList());
        }

        if (keys.isEmpty()) {
            keys.add("roles");
            keys.add("role");
        }

        this.roleClaimKeys = keys.stream().distinct().toList();
    }

    public String userId(Claims claims) {
        return claims.getSubject();
    }

    public Set<Role> roles(Claims claims) {
        Object value = null;

        for (String key : roleClaimKeys) {
            value = claims.get(key);
            if (value != null) break;
        }

        return toRoles(value);
    }

    private Set<Role> toRoles(Object value) {
        if (value == null) return Set.of();

        Set<Role> roles = new LinkedHashSet<>();

        if (value instanceof String s) {
            roles.add(Role.from(s));
            return roles;
        }

        if (value instanceof Collection<?> col) {
            for (Object it : col) {
                if (it != null) roles.add(Role.from(Objects.toString(it)));
            }
            return roles;
        }

        roles.add(Role.from(Objects.toString(value)));
        return roles;
    }
}
