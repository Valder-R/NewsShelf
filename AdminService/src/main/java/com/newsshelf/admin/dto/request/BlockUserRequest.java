package com.newsshelf.admin.dto.request;

import com.fasterxml.jackson.annotation.JsonFormat;
import jakarta.validation.constraints.Size;

import java.time.Instant;


public record BlockUserRequest(
        @Size(max = 255, message = "reason must be <= 255 characters")
        String reason,

        @JsonFormat(shape = JsonFormat.Shape.STRING)
        Instant until
) {
}
