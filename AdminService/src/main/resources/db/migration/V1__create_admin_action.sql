CREATE TABLE admin_action
(
    id             BIGSERIAL PRIMARY KEY,

    correlation_id UUID         NOT NULL,
    action_type    VARCHAR(64)  NOT NULL,
    status         VARCHAR(24)  NOT NULL,

    actor_user_id  VARCHAR(64)  NOT NULL,
    actor_roles    VARCHAR(255) NOT NULL,

    target_type    VARCHAR(32)  NOT NULL,
    target_id      VARCHAR(64),

    started_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    finished_at    TIMESTAMPTZ,
    duration_ms    BIGINT,

    created_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX uq_admin_action_correlation ON admin_action (correlation_id);
CREATE INDEX ix_admin_action_actor_time ON admin_action (actor_user_id, started_at);
CREATE INDEX ix_admin_action_status_time ON admin_action (status, started_at);
CREATE INDEX ix_admin_action_type_time ON admin_action (action_type, started_at);
CREATE INDEX ix_admin_action_target ON admin_action (target_type, target_id);
