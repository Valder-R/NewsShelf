package com.newsshelf.admin.audit.service;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.AdminAction;
import com.newsshelf.admin.audit.model.TargetType;
import com.newsshelf.admin.audit.repository.AdminActionRepository;
import com.newsshelf.admin.security.principal.AdminPrincipal;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

import java.time.OffsetDateTime;
import java.util.Collection;
import java.util.UUID;
import java.util.stream.Collectors;

@Slf4j
@Service
@RequiredArgsConstructor
public class DefaultAdminActionService implements AdminActionService {

    private final AdminActionRepository actionRepository;

    @Override
    public void log(ActionType action, TargetType targetType, String targetId, ActionStatus status) {
        try {
            Authentication auth = SecurityContextHolder.getContext().getAuthentication();

            String actor = currentActor(auth);
            String actorRoles = currentActorRoles(auth);

            UUID correlationId = UUID.randomUUID();
            OffsetDateTime now = OffsetDateTime.now();

            UUID targetUserId = null;
            if (targetType == TargetType.USER && targetId != null && !targetId.isBlank()) {
                try {
                    targetUserId = UUID.fromString(targetId);
                } catch (IllegalArgumentException ignored) {
                    targetUserId = null;
                }
            }

            actionRepository.save(
                    AdminAction.builder()
                            .correlationId(correlationId)
                            .actionType(action.name())
                            .targetUserId(targetUserId)
                            .actor(actor)
                            .actorRoles(actorRoles)
                            .status(status.name())
                            .errorMessage(status == ActionStatus.FAILED ? "FAILED" : null)
                            .startedAt(now)
                            .finishedAt(now)
                            .build()
            );

            log.debug("audit saved actionType={} targetType={} targetId={} status={}",
                    action, targetType, targetId, status);

        } catch (Exception e) {
            log.warn("audit save failed actionType={} targetType={} targetId={} status={} reason={}",
                    action, targetType, targetId, status, e.getMessage(), e);
        }
    }

    private String currentActor(Authentication auth) {
        if (auth == null) return "unknown";

        Object principal = auth.getPrincipal();
        if (principal instanceof AdminPrincipal p) {
            return p.userId();
        }

        return auth.getName() != null ? auth.getName() : "unknown";
    }

    private String currentActorRoles(Authentication auth) {
        if (auth == null) return "UNKNOWN";

        Collection<? extends GrantedAuthority> authorities = auth.getAuthorities();
        if (authorities.isEmpty()) return "UNKNOWN";

        return authorities.stream()
                .map(GrantedAuthority::getAuthority)
                .distinct()
                .collect(Collectors.joining(","));
    }
}
