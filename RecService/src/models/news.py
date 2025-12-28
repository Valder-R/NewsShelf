"""
News model with embedding support
"""

from sqlalchemy import Column, Integer, String, Date, Text, LargeBinary
from src.db import Base
from datetime import date


class News(Base):
    """News article model with embedding storage"""
    
    __tablename__ = "news"

    id = Column(Integer, primary_key=True, index=True)
    category = Column(String(100), index=True)
    title = Column(String(500), nullable=False)
    short_description = Column(Text)
    authors = Column(String(255))
    link = Column(String(500), unique=True)
    date = Column(Date, nullable=False, index=True)
    
    # Embedding fields
    title_embedding = Column(LargeBinary)  # Serialized numpy array
    description_embedding = Column(LargeBinary)  # Serialized numpy array
    
    # Metadata
    view_count = Column(Integer, default=0)
    embedding_generated = Column(Integer, default=0)  # Boolean: 0=False, 1=True
    
    def __repr__(self):
        return f"<News(id={self.id}, title='{self.title}', category='{self.category}')>"
