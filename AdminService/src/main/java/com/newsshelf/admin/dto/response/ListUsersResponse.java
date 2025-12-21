package com.newsshelf.admin.dto.response;

import java.util.List;


public record ListUsersResponse(
        List<UserItem> users,
        int total
) {
    public record UserItem(
            String id,
            String email,
            String role,
            String status
    ) {
    }
}
