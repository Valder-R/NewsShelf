
namespace DataAccess.Entities
{
    public enum NewsCategory
    {
        /// <summary>
        /// Default category for articles that do not fit into any specific group.
        /// Used as a safe fallback when no category is provided.
        /// </summary>
        Other = 0,

        /// <summary>
        /// News related to politics, government, elections, and public policy.
        /// </summary>
        Politics = 1,

        /// <summary>
        /// Business, finance, markets, and economic developments.
        /// </summary>
        Economy = 2,

        /// <summary>
        /// Technology, IT, software, hardware, gadgets, and artificial intelligence.
        /// </summary>
        Technology = 3,

        /// <summary>
        /// Scientific discoveries, research, space, and academic studies.
        /// </summary>
        Science = 4,

        /// <summary>
        /// Sports events, competitions, athletes, and sports-related news.
        /// </summary>
        Sport = 5,

        /// <summary>
        /// Culture, art, movies, music, literature, and entertainment.
        /// </summary>
        Culture = 6,

        /// <summary>
        /// Health, medicine, medical research, wellness, and healthcare system.
        /// </summary>
        Health = 7,

        /// <summary>
        /// International news and global events.
        /// </summary>
        World = 8,

        /// <summary>
        /// Social issues, community events, public incidents, and societal topics.
        /// </summary>
        Society = 9,

        /// <summary>
        /// Ecology, climate change, nature, and environmental topics.
        /// </summary>
        Environment = 10,

        /// <summary>
        /// Tourism, travel destinations, transportation, and adventures.
        /// </summary>
        Travel = 11,

        /// <summary>
        /// Lifestyle, daily life, trends, beauty, and personal well-being.
        /// </summary>
        Lifestyle = 12,

        /// <summary>
        /// Educational system, schools, universities, and learning materials.
        /// </summary>
        Education = 13,

        /// <summary>
        /// Automotive news, vehicles, transportation technologies, and car industry.
        /// </summary>
        Automotive = 14
    }
}
