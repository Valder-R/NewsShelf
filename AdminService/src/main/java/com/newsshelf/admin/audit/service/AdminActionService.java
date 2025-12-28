package com.newsshelf.admin.audit.service;

import com.newsshelf.admin.audit.model.ActionStatus;
import com.newsshelf.admin.audit.model.ActionType;
import com.newsshelf.admin.audit.model.TargetType;

public interface AdminActionService {

    void log(ActionType action,
             TargetType targetType,
             String targetId,
             ActionStatus status);
}
