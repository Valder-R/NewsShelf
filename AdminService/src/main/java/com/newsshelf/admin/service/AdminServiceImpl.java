package com.newsshelf.admin.service;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.request.BlockUserRequest;
import com.newsshelf.admin.dto.response.*;
import com.newsshelf.admin.service.comment.CommentAdminOperations;
import com.newsshelf.admin.service.post.PostAdminOperations;
import com.newsshelf.admin.service.user.UserAdminOperations;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class AdminServiceImpl implements AdminService {

    private final UserAdminOperations userOps;
    private final CommentAdminOperations commentOps;
    private final PostAdminOperations postOps;


    @Override
    public AssignStaffRoleResponse assignRole(String userId, AssignRoleRequest request) {
        return userOps.assignRole(userId, request);
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
        return userOps.listUsers(role, status);
    }

    @Override
    public BlockUserResponse blockUser(String userId, BlockUserRequest request) {
        return userOps.blockUser(userId, request);
    }

    @Override
    public UnblockUserResponse unblockUser(String userId) {
        return userOps.unblockUser(userId);
    }

    @Override
    public DeleteUserResponse deleteUser(String userId) {
        return userOps.deleteUser(userId);
    }

    @Override
    public DeleteCommentResponse deleteComment(String commentId) {
        return commentOps.deleteComment(commentId);
    }

    @Override
    public DeletePostResponse deletePost(String postId) {
        return postOps.deletePost(postId);
    }
}
