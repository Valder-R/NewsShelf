package com.newsshelf.admin.service;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;
import com.newsshelf.admin.service.comment.CommentAdminService;
import com.newsshelf.admin.service.post.PostAdminService;
import com.newsshelf.admin.service.user.UserAdminService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;


@Service
@RequiredArgsConstructor
public class AdminFacade implements AdminService {

    private final UserAdminService userAdminService;
    private final PostAdminService postAdminService;
    private final CommentAdminService commentAdminService;

    @Override
    public void assignRole(String userId, AssignRoleRequest request) {
        userAdminService.assignRole(userId, request);
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
        return userAdminService.listUsers(role, status);
    }

    @Override
    public void deleteUser(String userId) {
        userAdminService.deleteUser(userId);
    }

    @Override
    public void deleteComment(String commentId) {
        commentAdminService.deleteComment(commentId);
    }

    @Override
    public void deletePost(String postId) {
        postAdminService.deletePost(postId);
    }
}
