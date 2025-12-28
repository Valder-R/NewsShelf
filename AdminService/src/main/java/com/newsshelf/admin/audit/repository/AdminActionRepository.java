package com.newsshelf.admin.audit.repository;

import com.newsshelf.admin.audit.model.AdminAction;
import org.springframework.data.jpa.repository.JpaRepository;

public interface AdminActionRepository extends JpaRepository<AdminAction, Long> {
}
