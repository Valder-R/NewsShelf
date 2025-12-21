package com.newsshelf.admin.controller;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;
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

    /**
     * Призначення ролі.
     */
    @PatchMapping("/users/{userId}/role")
    public ResponseEntity<Void> assignRole(
            @PathVariable String userId,
            @Valid @RequestBody AssignRoleRequest request
    ) {
        adminService.assignRole(userId, request);
        return ResponseEntity.noContent().build();
    }

    /**
     * Список користувачів з опційними фільтрами: role та status.
     */
    @GetMapping("/users")
    public ResponseEntity<ListUsersResponse> listUsers(
            @RequestParam(required = false) String role,
            @RequestParam(required = false) String status
    ) {
        return ResponseEntity.ok(adminService.listUsers(role, status));
    }

    /**
     * Видалення користувача.
     */
    @DeleteMapping("/users/{userId}")
    public ResponseEntity<Void> deleteUser(
            @PathVariable String userId
    ) {
        adminService.deleteUser(userId);
        return ResponseEntity.noContent().build();
    }

    /**
     * Видалення коментаря.
     */
    @DeleteMapping("/comments/{commentId}")
    public ResponseEntity<Void> deleteComment(
            @PathVariable String commentId
    ) {
        adminService.deleteComment(commentId);
        return ResponseEntity.noContent().build();
    }

    /**
     * Видалення поста.
     */
    @DeleteMapping("/posts/{postId}")
    public ResponseEntity<Void> deletePost(
            @PathVariable String postId
    ) {
        adminService.deletePost(postId);
        return ResponseEntity.noContent().build();
    }
}
