"""
Logger configuration and utilities
"""

from loguru import logger
import sys
import os
from src.config import get_settings

settings = get_settings()


def setup_logging():
    """Configure loguru logger"""
    
    # Remove default handler
    logger.remove()
    
    # Add console handler
    logger.add(
        sys.stdout,
        format=settings.log_format,
        level=settings.log_level,
        colorize=True
    )
    
    # Add file handler for errors
    log_dir = "logs"
    if not os.path.exists(log_dir):
        os.makedirs(log_dir)
    
    logger.add(
        f"{log_dir}/error.log",
        format=settings.log_format,
        level="ERROR",
        rotation="500 MB",
        retention="7 days"
    )
    
    # Add file handler for all logs
    logger.add(
        f"{log_dir}/app.log",
        format=settings.log_format,
        level=settings.log_level,
        rotation="500 MB",
        retention="7 days"
    )
    
    logger.info("✅ Logging configured successfully")


# Setup logging on import
setup_logging()

__all__ = ["logger", "setup_logging"]
