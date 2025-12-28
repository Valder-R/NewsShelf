package com.newsshelf.admin.security.token;

import com.newsshelf.admin.security.role.Role;

import java.util.Set;

public record AuthPayload(String userId, Set<Role> roles) { }
