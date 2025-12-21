package com.newsshelf.admin.security.principal;

import java.util.Set;


public record AdminPrincipal(String userId, Set<String> roles) {
}
