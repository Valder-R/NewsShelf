package com.newsshelf.admin.security.token;

import java.util.Set;


public record AuthPayload(String userId, Set<String> roles) {
}
