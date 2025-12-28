package com.newsshelf.admin.audit.service;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.AdminAction;
import com.newsshelf.admin.audit.model.TargetType;
import com.newsshelf.admin.audit.repository.AdminActionRepository;
import com.newsshelf.admin.security.principal.AdminPrincipal;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

import java.time.Instant;

@Service
@RequiredArgsConstructor
public class DefaultAdminActionService implements AdminActionService {

    private final AdminActionRepository actionRepository;

    @Override
    public void log(ActionType action, TargetType targetType, String targetId, ActionStatus status) {
        actionRepository.save(AdminAction.builder()
                .actorUserId(currentUserId())
                .action(action.name())
                .targetType(targetType.name())
                .targetId(targetId)
                .status(status.name())
                .createdAt(Instant.now())
                .build());
    }

    private String currentUserId() {
        var auth = SecurityContextHolder.getContext().getAuthentication();
        if (auth == null) return "unknown";

        var principal = auth.getPrincipal();
        if (principal instanceof AdminPrincipal p) {
            return p.userId();
        }
        return "unknown";
    }
}
