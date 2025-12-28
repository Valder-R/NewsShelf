package com.newsshelf.admin.security.role;

public enum Role {
    PUBLISHER,
    ADMIN;

    public static Role from(String raw) {
        if (raw == null || raw.isBlank()) throw new IllegalArgumentException("Role is blank");
        return Role.valueOf(raw.trim().toUpperCase());
    }
}
