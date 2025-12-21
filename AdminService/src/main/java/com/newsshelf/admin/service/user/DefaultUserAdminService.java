package com.newsshelf.admin.service.user;

import com.newsshelf.admin.dto.request.AssignRoleRequest;
import com.newsshelf.admin.dto.response.ListUsersResponse;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;
import org.springframework.web.server.ResponseStatusException;

import java.util.Optional;

@Service
public class DefaultUserAdminService implements UserAdminService {

    private final RestClient userServiceClient;

    public DefaultUserAdminService(
            @Qualifier("userServiceClient") RestClient userServiceClient
    ) {
        this.userServiceClient = userServiceClient;
    }

    @Override
    public void assignRole(String userId, AssignRoleRequest request) {
        if (request == null || request.role() == null || request.role().isBlank()) {
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

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public ListUsersResponse listUsers(String role, String status) {
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

            return body;

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }

    @Override
    public void deleteUser(String userId) {
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

        } catch (ResponseStatusException e) {
            throw e;
        } catch (Exception e) {
            throw new ResponseStatusException(HttpStatus.BAD_GATEWAY, "Failed to call UserService", e);
        }
    }
}
