package com.newsshelf.admin.service.user;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.TargetType;
import com.newsshelf.admin.audit.service.AdminActionService;
import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;
import org.springframework.web.server.ResponseStatusException;

import java.util.Optional;

@Slf4j
@Service
public class DefaultUserAdminService implements UserAdminService {

    private final RestClient userServiceClient;
    private final AdminActionService adminActionService;

    public DefaultUserAdminService(
            @Qualifier("userServiceClient") RestClient userServiceClient,
            AdminActionService adminActionService
    ) {
        this.userServiceClient = userServiceClient;
        this.adminActionService = adminActionService;
    }

    @Override
    public void assignRole(String userId, AssignRoleRequest request) {
        log.info("Assigning role via UserService. userId={} request={}", userId, request);

        if (request == null || request.role() == null) {
            log.warn("Role is missing for assignRole. userId={}", userId);
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "role is required");
        }

        try {
            userServiceClient.patch()
                    .uri("/api/users/{id}/role", userId)
                    .body(request)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        log.info("User not found in UserService. userId={}", userId);
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        log.warn("UserService 4xx on assignRole. userId={} status={}", userId, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        log.error("UserService 5xx on assignRole. userId={} status={}", userId, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.SUCCESS);
            log.info("Role assigned successfully. userId={}", userId);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            throw e;
        } catch (Exception e) {
            log.error("Failed to call UserService for assignRole. userId={}", userId, e);
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
        log.info("Listing users via UserService. role={} status={}", role, status);

        try {
            ListUsersResponse body = userServiceClient.get()
                    .uri(uriBuilder -> uriBuilder
                            .path("/api/users")
                            .queryParamIfPresent("role", Optional.ofNullable(role))
                            .queryParamIfPresent("status", Optional.ofNullable(status))
                            .build())
                    .retrieve()
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        log.warn("UserService 4xx on listUsers. role={} status={} http={}", role, status, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        log.error("UserService 5xx on listUsers. role={} status={} http={}", role, status, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .body(ListUsersResponse.class);

            if (body == null) {
                log.error("UserService returned empty body on listUsers. role={} status={}", role, status);
                throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "UserService returned empty body");
            }

            return body;

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            log.error("Failed to call UserService for listUsers. role={} status={}", role, status, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public void deleteUser(String userId) {
        log.info("Deleting user via UserService. userId={}", userId);

        try {
            userServiceClient.delete()
                    .uri("/api/users/{id}", userId)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        log.info("User not found in UserService. userId={}", userId);
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        log.warn("UserService 4xx on deleteUser. userId={} status={}", userId, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        log.error("UserService 5xx on deleteUser. userId={} status={}", userId, res.getStatusCode());
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.SUCCESS);
            log.info("User deleted successfully. userId={}", userId);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.FAILED);
            throw e;
        } catch (Exception e) {
            log.error("Failed to call UserService for deleteUser. userId={}", userId, e);
            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }
}
