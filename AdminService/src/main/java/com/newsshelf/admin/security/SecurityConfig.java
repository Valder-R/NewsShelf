package com.newsshelf.admin.security;

import com.newsshelf.admin.security.filter.AdminAuthFilter;
import com.newsshelf.admin.security.token.validation.TokenValidationService;
import lombok.RequiredArgsConstructor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

@Configuration
@RequiredArgsConstructor
public class SecurityConfig {

    private final TokenValidationService tokenValidationService;

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) {
        AdminAuthFilter adminAuthFilter = new AdminAuthFilter(tokenValidationService);

        return http
                .csrf(AbstractHttpConfigurer::disable)
                .sessionManagement(sm -> sm.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers("/api/v1/admin/health").permitAll()
                        .requestMatchers("/api/v1/admin/**").authenticated()
                        .anyRequest().permitAll()
                )
                .addFilterBefore(adminAuthFilter, UsernamePasswordAuthenticationFilter.class)
                .httpBasic(Customizer.withDefaults())
                .build();
    }
}
