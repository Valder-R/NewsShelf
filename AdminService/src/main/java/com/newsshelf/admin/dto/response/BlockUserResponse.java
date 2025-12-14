package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record BlockUserResponse(
        boolean success,
        String message,
        String userId,
        String status,
        String reason,
        Instant blockedUntil,
        Instant timestamp
) {
}
