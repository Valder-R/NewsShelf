package com.newsshelf.admin.audit.service;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.AdminAction;
import com.newsshelf.admin.audit.model.TargetType;
import com.newsshelf.admin.audit.repository.AdminActionRepository;
import com.newsshelf.admin.security.principal.AdminPrincipal;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

import java.time.Instant;

@Slf4j
@Service
@RequiredArgsConstructor
public class DefaultAdminActionService implements AdminActionService {

    private final AdminActionRepository actionRepository;

    @Override
    public void log(ActionType action, TargetType targetType, String targetId, ActionStatus status) {
        try {
            actionRepository.save(AdminAction.builder()
                    .actorUserId(currentUserId())
                    .action(action.name())
                    .targetType(targetType.name())
                    .targetId(targetId)
                    .status(status.name())
                    .createdAt(Instant.now())
                    .build());

            log.debug("audit saved action={} targetType={} targetId={} status={}", action, targetType, targetId, status);

        } catch (Exception e) {
            log.warn("audit save failed action={} targetType={} targetId={} status={} reason={}",
                    action, targetType, targetId, status, e.getMessage());
        }
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
