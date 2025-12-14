package com.newsshelf.admin.service.comment;

import com.newsshelf.admin.dto.response.DeleteCommentResponse;

public interface CommentAdminOperations {
    DeleteCommentResponse deleteComment(String commentId);
}
