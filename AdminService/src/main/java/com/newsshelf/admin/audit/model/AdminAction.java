package com.newsshelf.admin.audit.model;

import jakarta.persistence.*;
import lombok.*;

import java.time.OffsetDateTime;
import java.util.UUID;

@Entity
@Table(name = "admin_action")
@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class AdminAction {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @Column(name = "correlation_id", nullable = false)
    private UUID correlationId;

    @Column(name = "action_type", nullable = false, length = 64)
    private String actionType;

    @Column(name = "target_user_id")
    private UUID targetUserId;

    @Column(name = "actor", nullable = false, length = 128)
    private String actor;

    @Column(name = "actor_roles", nullable = false, length = 255)
    private String actorRoles;

    @Column(name = "status", nullable = false, length = 32)
    private String status;

    @Column(name = "error_message")
    private String errorMessage;

    @Column(name = "started_at", nullable = false)
    private OffsetDateTime startedAt;

    @Column(name = "finished_at")
    private OffsetDateTime finishedAt;
}
