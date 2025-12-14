package com.newsshelf.admin.dto.response;

import lombok.Builder;

import java.time.Instant;

@Builder
public record DeleteCommentResponse(
        boolean success,
        String message,
        String commentId,
        Instant timestamp
) {
}
