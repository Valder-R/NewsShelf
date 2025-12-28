package com.newsshelf.admin.service.comment;

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
public class DefaultCommentAdminService implements CommentAdminService {

    private final RestClient commentServiceClient;
    private final AdminActionService adminActionService;

    public DefaultCommentAdminService(
            @Qualifier("commentServiceClient") RestClient commentServiceClient,
            AdminActionService adminActionService
    ) {
        this.commentServiceClient = commentServiceClient;
        this.adminActionService = adminActionService;
    }

    @Override
    public void deleteComment(String commentId) {
        log.info("deleteComment start commentId={}", commentId);

        final int id;
        try {
            id = Integer.parseInt(commentId);
        } catch (NumberFormatException ex) {
            log.warn("deleteComment invalid commentId (not int) commentId={}", commentId);
            adminActionService.log(ActionType.DELETE_COMMENT, TargetType.COMMENT, commentId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "commentId must be integer");
        }

        try {
            commentServiceClient.delete()
                    .uri("/api/comments/{id}", id)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Comment not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "CommentService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "CommentService unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.DELETE_COMMENT, TargetType.COMMENT, commentId, ActionStatus.SUCCESS);
            log.info("deleteComment success commentId={} id={}", commentId, id);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.DELETE_COMMENT, TargetType.COMMENT, commentId, ActionStatus.FAILED);
            log.warn("deleteComment fail commentId={} id={} status={} reason={}",
                    commentId, id, e.getStatusCode(), e.getReason());
            throw e;

        } catch (Exception e) {
            adminActionService.log(ActionType.DELETE_COMMENT, TargetType.COMMENT, commentId, ActionStatus.FAILED);
            log.error("deleteComment error commentId={} id={}", commentId, id, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call CommentService", e);
        }
    }
}
