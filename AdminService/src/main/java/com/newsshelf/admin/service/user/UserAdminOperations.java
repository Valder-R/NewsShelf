package com.newsshelf.admin.service.user;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.request.BlockUserRequest;
import com.newsshelf.admin.dto.response.*;

public interface UserAdminOperations {
    AssignStaffRoleResponse assignRole(String userId, AssignRoleRequest request);

    ListUsersResponse listUsers(String role, String status);

    BlockUserResponse blockUser(String userId, BlockUserRequest request);

    UnblockUserResponse unblockUser(String userId);

    DeleteUserResponse deleteUser(String userId);
}
