package com.newsshelf.admin.security.config;

import com.newsshelf.admin.security.filter.AdminAuthFilter;
import com.newsshelf.admin.security.token.TokenAuthService;
import com.newsshelf.admin.security.token.jwt.JwtClaimsParser;
import com.newsshelf.admin.security.token.jwt.JwtTokenAuthService;
import com.newsshelf.admin.security.token.jwt.JwtVerifier;
import jakarta.servlet.DispatcherType;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;


@Slf4j
@Configuration
@EnableMethodSecurity
public class SecurityConfig {

    @Value("${security.jwt.secret}")
    private String secret;

    @Value("${security.jwt.issuer:}")
    private String issuer;

    @Value("${security.jwt.roles-claim:roles}")
    private String rolesClaim;

    @Value("${security.jwt.roles-claim-aliases:}")
    private String rolesClaimAliases;

    @Bean
    public TokenAuthService tokenAuthService() {
        var verifier = new JwtVerifier(secret, issuer);
        var parser = new JwtClaimsParser(rolesClaim, rolesClaimAliases);
        return new JwtTokenAuthService(verifier, parser);
    }

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http, TokenAuthService tokenAuthService) throws Exception {
        return http
                .csrf(AbstractHttpConfigurer::disable)
                .httpBasic(AbstractHttpConfigurer::disable)
                .formLogin(AbstractHttpConfigurer::disable)
                .sessionManagement(sm -> sm.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
                .authorizeHttpRequests(auth -> auth
                        .dispatcherTypeMatchers(DispatcherType.ERROR, DispatcherType.FORWARD).permitAll()
                        .requestMatchers("/error").permitAll()
                        .requestMatchers("/api/v1/admin/comments/**").hasAnyRole("ADMIN", "PUBLISHER")
                        .requestMatchers("/api/v1/admin/posts/**").hasAnyRole("ADMIN", "PUBLISHER")
                        .requestMatchers("/api/v1/admin/**").hasRole("ADMIN")
                        .anyRequest().authenticated()
                )
                .addFilterBefore(new AdminAuthFilter(tokenAuthService), UsernamePasswordAuthenticationFilter.class)
                .build();
    }
}
