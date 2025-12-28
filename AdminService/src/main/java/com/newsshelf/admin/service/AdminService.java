package com.newsshelf.admin.service;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;


public interface AdminService {

    void assignRole(String userId, AssignRoleRequest request);

    ListUsersResponse listUsers(String role, String status);

    void deleteUser(String userId);

    void deleteComment(String commentId);

    void deletePost(String postId);
}
