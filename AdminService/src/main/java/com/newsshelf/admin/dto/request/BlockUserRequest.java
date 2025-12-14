package com.newsshelf.admin.dto.request;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

import java.time.Instant;

@Data
public class BlockUserRequest {
    @NotBlank
    private String reason;

    private Instant until;
}
