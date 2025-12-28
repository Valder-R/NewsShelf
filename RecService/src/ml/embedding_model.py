"""
Embedding model for generating text embeddings
"""

from sentence_transformers import SentenceTransformer
import numpy as np
from typing import Union, List
from src.logger import logger
from src.config import get_settings


class EmbeddingModel:
    """Wrapper around SentenceTransformer for generating embeddings"""
    
    def __init__(self, model_name: str = None):
        """Initialize embedding model
        
        Args:
            model_name: Name of the sentence-transformer model
        """
        settings = get_settings()
        self.model_name = model_name or settings.embedding_model_name
        
        try:
            logger.info(f"Loading embedding model: {self.model_name}")
            self.model = SentenceTransformer(self.model_name)
            logger.info("✅ Embedding model loaded successfully")
        except Exception as e:
            logger.error(f"❌ Failed to load embedding model: {e}")
            raise

    def encode(self, text: Union[str, List[str]]) -> Union[np.ndarray, List[np.ndarray]]:
        """Encode text(s) to embedding(s)
        
        Args:
            text: Single string or list of strings to encode
            
        Returns:
            numpy array or list of arrays with embeddings
        """
        try:
            if isinstance(text, str):
                embedding = self.model.encode(text, convert_to_numpy=True)
                return embedding
            else:
                embeddings = self.model.encode(text, convert_to_numpy=True)
                return embeddings
        except Exception as e:
            logger.error(f"❌ Error encoding text: {e}")
            raise
    
    def similarity(self, embedding1: np.ndarray, embedding2: np.ndarray) -> float:
        """Calculate cosine similarity between two embeddings
        
        Args:
            embedding1: First embedding vector
            embedding2: Second embedding vector
            
        Returns:
            Similarity score between 0 and 1
        """
        from sklearn.metrics.pairwise import cosine_similarity
        similarity = cosine_similarity([embedding1], [embedding2])[0][0]
        return float(similarity)


# Global instance
_embedding_model = None


def get_embedding_model() -> EmbeddingModel:
    """Get or create embedding model singleton"""
    global _embedding_model
    if _embedding_model is None:
        _embedding_model = EmbeddingModel()
    return _embedding_model
