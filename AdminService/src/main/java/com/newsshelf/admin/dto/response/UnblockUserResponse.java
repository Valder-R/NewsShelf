package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record UnblockUserResponse(
        boolean success,
        String message,
        String userId,
        String status,
        Instant timestamp
) {
}
