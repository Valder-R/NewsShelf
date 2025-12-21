package com.newsshelf.admin.service.comment;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;
import org.springframework.web.server.ResponseStatusException;


@Service
public class DefaultCommentAdminService implements CommentAdminService {

    private final RestClient commentServiceClient;

    public DefaultCommentAdminService(
            @Qualifier("commentServiceClient") RestClient commentServiceClient
    ) {
        this.commentServiceClient = commentServiceClient;
    }

    @Override
    public void deleteComment(String commentId) {
        final int id;
        try {
            id = Integer.parseInt(commentId);
        } catch (NumberFormatException ex) {
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

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call CommentService", e);
        }
    }
}
