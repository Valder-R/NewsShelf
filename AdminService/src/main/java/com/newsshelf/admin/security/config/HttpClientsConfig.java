package com.newsshelf.admin.security.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpHeaders;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.client.RestClient;


@Configuration
public class HttpClientsConfig {

    @Bean
    public RestClient newsServiceClient(
            @Value("${services.news.base-url}") String baseUrl
    ) {
        return buildClient(baseUrl);
    }

    @Bean
    public RestClient commentServiceClient(
            @Value("${services.comments.base-url}") String baseUrl
    ) {
        return buildClient(baseUrl);
    }

    @Bean
    public RestClient userServiceClient(
            @Value("${services.user.base-url}") String baseUrl
    ) {
        return buildClient(baseUrl);
    }

    private RestClient buildClient(String baseUrl) {
        return RestClient.builder()
                .baseUrl(baseUrl)
                .requestInterceptor((request, body, execution) -> {
                    var auth = SecurityContextHolder.getContext().getAuthentication();
                    if (auth != null && auth.getDetails() instanceof String token && !token.isBlank()) {
                        request.getHeaders().set(HttpHeaders.AUTHORIZATION, "Bearer " + token);
                    }
                    return execution.execute(request, body);
                })
                .build();
    }

}
