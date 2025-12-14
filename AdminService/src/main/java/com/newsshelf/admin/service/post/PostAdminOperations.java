package com.newsshelf.admin.service.post;

import com.newsshelf.admin.dto.response.DeletePostResponse;

public interface PostAdminOperations {
    DeletePostResponse deletePost(String postId);
}
