package com.newsshelf.admin.security.token.validation;

import com.newsshelf.admin.security.model.AdminPrincipal;
import com.newsshelf.admin.security.token.parse.TokenParser;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class JWTokenValidation implements TokenValidationService {
    private final TokenParser tokenParser;

    @Override
    public AdminPrincipal validate(String token) {
        //TODO: Написати валідацію

        AdminPrincipal principal = tokenParser.parse(token);

        return principal;
    }
}
