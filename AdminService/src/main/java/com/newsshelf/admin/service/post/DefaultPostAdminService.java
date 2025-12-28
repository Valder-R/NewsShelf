package com.newsshelf.admin.service.post;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.TargetType;
import com.newsshelf.admin.audit.service.AdminActionService;
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
    private final AdminActionService adminActionService;

    public DefaultPostAdminService(
            @Qualifier("newsServiceClient") RestClient newsServiceClient,
            AdminActionService adminActionService
    ) {
        this.newsServiceClient = newsServiceClient;
        this.adminActionService = adminActionService;
    }


    @Override
    public void deletePost(String postId) {
        log.info("Deleting post via NewsService. postId={}", postId);

        final int id;
        try {
            id = Integer.parseInt(postId);
        } catch (NumberFormatException ex) {
            log.warn("Invalid postId (not int): {}", postId);
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
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

            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.SUCCESS);
            log.info("Post deleted successfully. postId={}", postId);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
            throw e;

        } catch (Exception e) {
            log.error("Failed to call NewsService for deletePost. postId={}", id, e);
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call NewsApi", e);
        }
    }

}
