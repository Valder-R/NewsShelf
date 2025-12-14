package com.newsshelf.admin.security.token.parse;

import com.newsshelf.admin.security.model.AdminPrincipal;


public interface TokenParser {
    AdminPrincipal parse(String token);
}
