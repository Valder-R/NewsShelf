package com.newsshelf.admin.security.model;

import lombok.Builder;

import java.util.Set;

@Builder
public record AdminPrincipal(
        String userId,
        String role,
        Set<String> permissions
) {
}
