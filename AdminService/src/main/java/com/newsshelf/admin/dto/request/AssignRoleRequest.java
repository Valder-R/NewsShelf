package com.newsshelf.admin.dto.request;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import lombok.Data;

@Data
public class AssignRoleRequest {
    @NotBlank
    @Pattern(regexp = "READER|MODERATOR|ADMIN", message = "role must be one of: READER, MODERATOR, ADMIN")
    private String role;

    private String reason;
}
