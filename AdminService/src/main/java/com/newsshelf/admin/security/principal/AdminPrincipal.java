package com.newsshelf.admin.security.principal;

import com.newsshelf.admin.security.role.Role;

import java.util.Set;

public record AdminPrincipal(String userId, Set<Role> roles) {
}
