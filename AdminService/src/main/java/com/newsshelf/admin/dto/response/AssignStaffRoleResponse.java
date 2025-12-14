package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record AssignStaffRoleResponse(
        boolean success,
        String message,
        String userId,
        String oldRole,
        String newRole,
        Instant timestamp
) {
}