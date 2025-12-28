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
        log.info("deletePost start postId={}", postId);

        final int id;
        try {
            id = Integer.parseInt(postId);
        } catch (NumberFormatException ex) {
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
            log.warn("deletePost invalid postId postId={}", postId);
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "postId must be integer");
        }

        try {
            newsServiceClient.delete()
                    .uri("/api/news/{id}", id)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "News not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "NewsApi rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "NewsApi unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.SUCCESS);
            log.info("deletePost success postId={} id={}", postId, id);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
            log.warn("deletePost fail postId={} id={} status={} reason={}",
                    postId, id, e.getStatusCode(), e.getReason());
            throw e;

        } catch (Exception e) {
            adminActionService.log(ActionType.DELETE_POST, TargetType.POST, postId, ActionStatus.FAILED);
            log.error("deletePost error postId={} id={}", postId, id, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call NewsApi", e);
        }
    }
}

