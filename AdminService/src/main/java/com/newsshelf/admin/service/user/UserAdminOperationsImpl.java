package com.newsshelf.admin.service.user;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.request.BlockUserRequest;
import com.newsshelf.admin.dto.response.*;
import org.springframework.stereotype.Service;

@Service
public class UserAdminOperationsImpl implements UserAdminOperations {
    //TODO: Реалізувати операції
    @Override
    public AssignStaffRoleResponse assignRole(String userId, AssignRoleRequest request) {
        return null;
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
        return null;
    }

    @Override
    public BlockUserResponse blockUser(String userId, BlockUserRequest request) {
        return null;
    }

    @Override
    public UnblockUserResponse unblockUser(String userId) {
        return null;
    }

    @Override
    public DeleteUserResponse deleteUser(String userId) {
        return null;
    }
}
