package com.newsshelf.admin.service.user;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;


public interface UserAdminService {
    void assignRole(String userId, AssignRoleRequest request);

    ListUsersResponse listUsers(String role, String status);

    void deleteUser(String userId);
}
