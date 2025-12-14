package com.newsshelf.admin.service;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.request.BlockUserRequest;
import com.newsshelf.admin.dto.response.*;

public interface AdminService {
    AssignStaffRoleResponse assignRole(String userId, AssignRoleRequest request);

    ListUsersResponse listUsers(String role, String status);

    BlockUserResponse blockUser(String userId, BlockUserRequest request);

    UnblockUserResponse unblockUser(String userId);

    DeleteUserResponse deleteUser(String userId);

    DeleteCommentResponse deleteComment(String commentId);

    DeletePostResponse deletePost(String postId);
}
