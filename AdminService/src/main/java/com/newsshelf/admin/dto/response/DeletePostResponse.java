package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record DeletePostResponse(
        boolean success,
        String message,
        String postId,
        Instant timestamp
) {
}
