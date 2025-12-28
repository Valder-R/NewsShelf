package com.newsshelf.admin.dto.request;

import com.newsshelf.admin.security.role.Role;
import jakarta.validation.constraints.NotNull;

public record AssignRoleRequest(@NotNull Role role) {
}
