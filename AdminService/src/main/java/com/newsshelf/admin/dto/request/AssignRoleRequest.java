package com.newsshelf.admin.dto.request;

import jakarta.validation.constraints.NotBlank;


public record AssignRoleRequest(@NotBlank(message = "role is required") String role) {
}
