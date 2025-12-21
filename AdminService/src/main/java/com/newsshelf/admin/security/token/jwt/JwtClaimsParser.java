package com.newsshelf.admin.security.token.jwt;

import io.jsonwebtoken.Claims;

import java.util.Collection;
import java.util.HashSet;
import java.util.Locale;
import java.util.Set;


public class JwtClaimsParser {

    private final String rolesClaimName;

    public JwtClaimsParser(String rolesClaimName) {
        this.rolesClaimName = (rolesClaimName == null || rolesClaimName.isBlank())
                ? "roles"
                : rolesClaimName;
    }

    public String extractUserId(Claims claims) {
        String sub = claims.getSubject();
        if (sub != null && !sub.isBlank()) return sub;

        Object uid = claims.get("userId");
        if (uid != null) return String.valueOf(uid);

        return null;
    }

    public Set<String> extractRoles(Claims claims) {
        Object raw = claims.get(rolesClaimName);
        if (raw == null) raw = claims.get("role");

        if (raw == null) return Set.of();

        if (raw instanceof Collection<?> col) {
            Set<String> roles = new HashSet<>();
            for (Object v : col) {
                if (v != null) roles.add(String.valueOf(v));
            }
            return normalizeRoles(roles);
        }

        String s = String.valueOf(raw).trim();
        if (s.isEmpty()) return Set.of();

        String[] parts = s.contains(",") ? s.split(",") : s.split("\\s+");

        Set<String> roles = new HashSet<>();
        for (String p : parts) {
            String r = p.trim();
            if (!r.isEmpty()) roles.add(r);
        }
        return normalizeRoles(roles);
    }

    private Set<String> normalizeRoles(Set<String> roles) {
        Set<String> out = new HashSet<>();
        for (String r : roles) {
            if (r == null) continue;

            String x = r.trim();
            if (x.startsWith("ROLE_")) x = x.substring("ROLE_".length());
            if (!x.isBlank()) out.add(x.toUpperCase(Locale.ROOT));
        }
        return out;
    }
}
