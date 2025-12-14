package com.newsshelf.admin.controller;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.request.BlockUserRequest;
import com.newsshelf.admin.dto.response.*;
import com.newsshelf.admin.service.AdminService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/admin")
@RequiredArgsConstructor
public class AdminController {

    private final AdminService adminService;

    @PostMapping("/users/{userId}/role")
    public ResponseEntity<AssignStaffRoleResponse> assignRole(
            @PathVariable String userId,
            @Valid @RequestBody AssignRoleRequest request
    ) {
        AssignStaffRoleResponse response = adminService.assignRole(userId, request);

        return ResponseEntity.ok(response);
    }

    @GetMapping("/users")
    public ResponseEntity<ListUsersResponse> listUsers(
            @RequestParam(required = false) String role,
            @RequestParam(required = false) String status
    ) {
        ListUsersResponse response = adminService.listUsers(role, status);

        return ResponseEntity.ok(response);
    }

    @PostMapping("/users/{userId}/block")
    public ResponseEntity<BlockUserResponse> blockUser(
            @PathVariable String userId,
            @Valid @RequestBody BlockUserRequest request
    ) {
        BlockUserResponse response = adminService.blockUser(userId, request);

        return ResponseEntity.ok(response);
    }

    @PostMapping("/users/{userId}/unblock")
    public ResponseEntity<UnblockUserResponse> unblockUser(
            @PathVariable String userId
    ) {
        UnblockUserResponse response = adminService.unblockUser(userId);

        return ResponseEntity.ok(response);
    }

    @DeleteMapping("/users/{userId}")
    public ResponseEntity<DeleteUserResponse> deleteUser(
            @PathVariable String userId
    ) {
        DeleteUserResponse response = adminService.deleteUser(userId);

        return ResponseEntity.ok(response);
    }

    @DeleteMapping("/comments/{commentId}")
    public ResponseEntity<DeleteCommentResponse> deleteComment(
            @PathVariable String commentId
    ) {
        DeleteCommentResponse response = adminService.deleteComment(commentId);

        return ResponseEntity.ok(response);
    }

    @DeleteMapping("/posts/{postId}")
    public ResponseEntity<DeletePostResponse> deletePost(
            @PathVariable String postId
    ) {
        DeletePostResponse response = adminService.deletePost(postId);

        return ResponseEntity.ok(response);
    }
}
