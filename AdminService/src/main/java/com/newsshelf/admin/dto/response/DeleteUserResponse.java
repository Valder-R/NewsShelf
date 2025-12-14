package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record DeleteUserResponse(
        boolean success,
        String message,
        String userId,
        Instant timestamp
) {
}
