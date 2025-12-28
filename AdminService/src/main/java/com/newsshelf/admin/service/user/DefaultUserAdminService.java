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
        String role = request == null ? null : String.valueOf(request.role());
        log.info("assignRole start userId={} role={}", userId, role);

        if (role == null || role.isBlank()) {
            log.warn("assignRole invalid role userId={} role={}", userId, role);
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "role is required");
        }

        try {
            userServiceClient.patch()
                    .uri("/api/users/{id}/role", userId)
                    .body(request)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.SUCCESS);
            log.info("assignRole success userId={} role={}", userId, role);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            log.warn("assignRole fail userId={} role={} status={} reason={}",
                    userId, role, e.getStatusCode(), e.getReason());
            throw e;

        } catch (Exception e) {
            adminActionService.log(ActionType.ASSIGN_ROLE, TargetType.USER, userId, ActionStatus.FAILED);
            log.error("assignRole error userId={} role={}", userId, role, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
        log.info("listUsers start role={} status={}", role, status);

        try {
            ListUsersResponse body = userServiceClient.get()
                    .uri(uriBuilder -> uriBuilder
                            .path("/api/users")
                            .queryParamIfPresent("role", Optional.ofNullable(role))
                            .queryParamIfPresent("status", Optional.ofNullable(status))
                            .build())
                    .retrieve()
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .body(ListUsersResponse.class);

            if (body == null) {
                throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "UserService returned empty body");
            }

            log.info("listUsers success role={} status={}", role, status);
            return body;

        } catch (ResponseStatusException e) {
            log.warn("listUsers fail role={} status={} statusCode={} reason={}",
                    role, status, e.getStatusCode(), e.getReason());
            throw e;

        } catch (Exception e) {
            log.error("listUsers error role={} status={}", role, status, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public void deleteUser(String userId) {
        log.info("deleteUser start userId={}", userId);

        try {
            userServiceClient.delete()
                    .uri("/api/users/{id}", userId)
                    .retrieve()
                    .onStatus(s -> s.value() == 404, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found");
                    })
                    .onStatus(HttpStatusCode::is4xxClientError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                                "UserService rejected request: " + res.getStatusCode());
                    })
                    .onStatus(HttpStatusCode::is5xxServerError, (req, res) -> {
                        throw new ResponseStatusException(HttpStatus.BAD_GATEWAY,
                                "UserService unavailable: " + res.getStatusCode());
                    })
                    .toBodilessEntity();

            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.SUCCESS);
            log.info("deleteUser success userId={}", userId);

        } catch (ResponseStatusException e) {
            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.FAILED);
            log.warn("deleteUser fail userId={} status={} reason={}",
                    userId, e.getStatusCode(), e.getReason());
            throw e;

        } catch (Exception e) {
            adminActionService.log(ActionType.DELETE_USER, TargetType.USER, userId, ActionStatus.FAILED);
            log.error("deleteUser error userId={}", userId, e);
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }
}
