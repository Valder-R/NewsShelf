package com.newsshelf.admin.security.token.validation;

import com.newsshelf.admin.security.model.AdminPrincipal;

public interface TokenValidationService {
    AdminPrincipal validate(String token);
}
