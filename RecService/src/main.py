"""
FastAPI application for Recommendation Service
"""

from fastapi import FastAPI, Response
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from sqlalchemy import text

from src.api.recommend import router
from src.config import get_settings
from src.logger import logger, setup_logging
from src.db import SessionLocal, engine, Base

# Setup logging
setup_logging()

# Get settings
settings = get_settings()

# Create FastAPI app
app = FastAPI(
    title=settings.api_title,
    description=settings.api_description,
    version=settings.api_version,
    openapi_url="/api/v1/openapi.json",
    docs_url="/api/v1/docs",
    redoc_url="/api/v1/redoc"
)

# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# Create database tables
@app.on_event("startup")
async def startup_event():
    """Initialize database on startup"""
    logger.info("🚀 Starting Recommendation Service...")
    try:
        Base.metadata.create_all(bind=engine)
        logger.info("✅ Database tables created/verified")
    except Exception as e:
        logger.error(f"❌ Failed to create tables: {e}")


@app.on_event("shutdown")
async def shutdown_event():
    """Cleanup on shutdown"""
    logger.info("🛑 Shutting down Recommendation Service...")


# Health check endpoint
@app.get("/health", tags=["Health"])
async def health_check():
    """Check if service is running"""
    try:
        db = SessionLocal()
        db.execute(text("SELECT 1"))
        db.close()
        
        return JSONResponse(
            status_code=200,
            content={
                "status": "healthy",
                "database": "connected",
                "message": "Recommendation Service is running"
            }
        )
    except Exception as e:
        logger.error(f"❌ Health check failed: {e}")
        return JSONResponse(
            status_code=503,
            content={
                "status": "unhealthy",
                "database": "disconnected",
                "message": str(e)
            }
        )


# Include routers
app.include_router(router)


# Root endpoint
@app.get("/", tags=["Root"])
async def root():
    """API root endpoint"""
    return {
        "name": "Recommendation Service API",
        "version": settings.api_version,
        "docs": "/api/v1/docs"
    }


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8001)
