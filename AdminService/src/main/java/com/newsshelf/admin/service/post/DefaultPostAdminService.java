package com.newsshelf.admin.service.post;

import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;
import org.springframework.web.server.ResponseStatusException;

@Slf4j
@Service
public class DefaultPostAdminService implements PostAdminService {

    private final RestClient newsServiceClient;

    public DefaultPostAdminService(
            @Qualifier("newsServiceClient") RestClient newsServiceClient
    ) {
        this.newsServiceClient = newsServiceClient;
    }


    @Override
    public void deletePost(String postId) {
        log.info("Deleting post via NewsService. postId={}", postId);

        final int id;
        try {
            id = Integer.parseInt(postId);
        } catch (NumberFormatException ex) {
            log.warn("Invalid postId (not int): {}", postId);
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "postId must be integer");
        }

        try {
            newsServiceClient.delete()
                    .uri("/api/news/{id}", id)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        log.info("Post not found in NewsService. postId={}", id);
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "News not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        log.warn("NewsService 4xx on deletePost. postId={} status={}", id, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "NewsApi rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        log.error("NewsService 5xx on deletePost. postId={} status={}", id, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "NewsApi unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            log.info("Post deleted successfully. postId={}", id);

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            log.error("Failed to call NewsService for deletePost. postId={}", id, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call NewsApi", e);
        }
    }
}
