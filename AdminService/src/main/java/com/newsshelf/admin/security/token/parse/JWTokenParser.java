package com.newsshelf.admin.security.token.parse;

import com.newsshelf.admin.security.model.AdminPrincipal;
import org.springframework.stereotype.Service;

import java.util.Set;

@Service
public class JWTokenParser implements TokenParser {
    @Override
    public AdminPrincipal parse(String token) {
        return AdminPrincipal.builder()
                .userId("1")
                .role("ADMIN")
                .permissions(Set.of("USER_BLOCK", "USER_DELETE", "POST_DELETE", "COMMENT_DELETE"))
                .build();
    }
}
