package com.newsshelf.admin.dto.response;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;
import java.util.List;

@Builder
public record ListUsersResponse(
        boolean success,
        String message,
        List<UserItem> users,
        Integer count,
        Instant timestamp
) {
    @Data
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor
    public static class UserItem {
        private String userId;
        private String email;
        private String role;
        private String status;

    }
}
