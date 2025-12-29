using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> NewsItems => Set<News>();
        public DbSet<NewsImage> NewsImages => Set<NewsImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store enum Category as string
            modelBuilder.Entity<News>()
                .Property(n => n.Category)
                .HasConversion<string>();

            modelBuilder.Entity<News>().HasData(
        new News
        {
            Id = 1,
            Title = "Ринок ШІ у 2025: головні напрями зростання",
            Content = "Аналітики відзначають зміщення фокусу з експериментів на промислові сценарії: підтримка клієнтів, пошук, автоматизація бек-офісу.",
            PublishedAt = new DateTime(2025, 12, 20, 9, 15, 0, DateTimeKind.Utc),
            Author = "Редакція",
            Category = NewsCategory.Technology
        },
        new News
        {
            Id = 2,
            Title = "Оновлення підходів до кібербезпеки у бізнесі",
            Content = "Компанії частіше впроваджують Zero Trust, посилюють контроль доступів і аудит привілеїв, а також тренують персонал.",
            PublishedAt = new DateTime(2025, 12, 19, 14, 0, 0, DateTimeKind.Utc),
            Author = "Ірина Мельник",
            Category = NewsCategory.Technology
        },
        new News
        {
            Id = 3,
            Title = "Економічний огляд: ключові показники тижня",
            Content = "Інвестори уважно стежать за інфляційними очікуваннями та динамікою ринку праці; на ринках зберігається помірна волатильність.",
            PublishedAt = new DateTime(2025, 12, 18, 8, 30, 0, DateTimeKind.Utc),
            Author = "Олег Савчук",
            Category = NewsCategory.Economy
        },
        new News
        {
            Id = 4,
            Title = "Наука: нові результати у дослідженнях матеріалів",
            Content = "Дослідники повідомляють про поліпшення характеристик композитів, що може вплинути на енергоефективність та виробництво.",
            PublishedAt = new DateTime(2025, 12, 17, 11, 10, 0, DateTimeKind.Utc),
            Author = "Лабораторний дайджест",
            Category = NewsCategory.Science
        },
        new News
        {
            Id = 5,
            Title = "Здоров'я: як підтримувати режим у сезон застуд",
            Content = "Лікарі нагадують про важливість сну, питного режиму та збалансованого харчування; профілактика починається з рутини.",
            PublishedAt = new DateTime(2025, 12, 16, 7, 45, 0, DateTimeKind.Utc),
            Author = "Марія Коваль",
            Category = NewsCategory.Health
        },
        new News
        {
            Id = 6,
            Title = "Світ: підсумки ключових міжнародних подій",
            Content = "Тиждень відзначився активізацією переговорів у кількох регіонах та новими заявами щодо торговельних обмежень.",
            PublishedAt = new DateTime(2025, 12, 15, 16, 20, 0, DateTimeKind.Utc),
            Author = "International Desk",
            Category = NewsCategory.World
        },
        new News
        {
            Id = 7,
            Title = "Суспільство: волонтерські ініціативи набирають обертів",
            Content = "Громадські проєкти частіше залучають бізнес і локальні спільноти, формуючи сталі програми підтримки.",
            PublishedAt = new DateTime(2025, 12, 14, 10, 5, 0, DateTimeKind.Utc),
            Author = "Анна Гринь",
            Category = NewsCategory.Society
        },
        new News
        {
            Id = 8,
            Title = "Екологія: міста розширюють програми сортування",
            Content = "У громадах з’являються нові пункти збору вторсировини; паралельно запускають інформаційні кампанії для мешканців.",
            PublishedAt = new DateTime(2025, 12, 13, 12, 0, 0, DateTimeKind.Utc),
            Author = "EcoWatch",
            Category = NewsCategory.Environment
        },
        new News
        {
            Id = 9,
            Title = "Подорожі: що врахувати при зимових поїздках",
            Content = "Поради стосуються безпеки в дорозі, страхування, а також планування маршрутів із запасом на погоду та затримки.",
            PublishedAt = new DateTime(2025, 12, 12, 18, 35, 0, DateTimeKind.Utc),
            Author = "Travel редакція",
            Category = NewsCategory.Travel
        },
        new News
        {
            Id = 10,
            Title = "Культура: прем’єри грудня та очікування критиків",
            Content = "Кінорелізи цього місяця роблять ставку на жанрове різноманіття — від драми до наукової фантастики.",
            PublishedAt = new DateTime(2025, 12, 11, 13, 25, 0, DateTimeKind.Utc),
            Author = "Кіноколонка",
            Category = NewsCategory.Culture
        },
        new News
        {
            Id = 11,
            Title = "Освіта: цифрові інструменти для навчання",
            Content = "Платформи для тестування, трекінгу прогресу і спільної роботи стають стандартом навіть у невеликих навчальних групах.",
            PublishedAt = new DateTime(2025, 12, 10, 9, 0, 0, DateTimeKind.Utc),
            Author = "EdTech огляд",
            Category = NewsCategory.Education
        },
        new News
        {
            Id = 12,
            Title = "Авто: тренди у системах допомоги водієві",
            Content = "ADAS-функції поширюються у масовому сегменті; виробники зосереджуються на точності сенсорів і надійності сценаріїв.",
            PublishedAt = new DateTime(2025, 12, 9, 15, 40, 0, DateTimeKind.Utc),
            Author = "AutoDesk",
            Category = NewsCategory.Automotive
        },
        new News
        {
            Id = 13,
            Title = "Спорт: підсумки ігрового тижня",
            Content = "Команди демонструють щільну конкуренцію; ключовим фактором стає глибина складу та дисципліна у захисті.",
            PublishedAt = new DateTime(2025, 12, 8, 20, 10, 0, DateTimeKind.Utc),
            Author = "Sports редакція",
            Category = NewsCategory.Sport
        },
        new News
        {
            Id = 14,
            Title = "Політика: пріоритети порядку денного на місяць",
            Content = "У фокусі — бюджетні рішення, соціальні програми та регуляторні зміни, що можуть вплинути на бізнес-процеси.",
            PublishedAt = new DateTime(2025, 12, 7, 10, 30, 0, DateTimeKind.Utc),
            Author = "Політичний огляд",
            Category = NewsCategory.Politics
        },
        new News
        {
            Id = 15,
            Title = "Лайфстайл: як планувати цілі без вигорання",
            Content = "Експерти радять дробити цілі на короткі спринти, вимірювати прогрес та залишати простір для відновлення.",
            PublishedAt = new DateTime(2025, 12, 6, 9, 50, 0, DateTimeKind.Utc),
            Author = "Lifestyle Desk",
            Category = NewsCategory.Lifestyle
        },
        new News
        {
            Id = 16,
            Title = "Технології: огляд популярних стеків для веб",
            Content = "Найчастіше обирають поєднання .NET/Java/Node для бекенду та React/Vue/Angular для фронтенду — залежно від команди і домену.",
            PublishedAt = new DateTime(2025, 12, 5, 12, 15, 0, DateTimeKind.Utc),
            Author = "Tech огляд",
            Category = NewsCategory.Technology
        },
        new News
        {
            Id = 17,
            Title = "Економіка: як бізнес оптимізує витрати у 2025",
            Content = "Серед підходів — перегляд портфеля проєктів, автоматизація операцій та перехід на більш прогнозовані контракти.",
            PublishedAt = new DateTime(2025, 12, 4, 8, 5, 0, DateTimeKind.Utc),
            Author = "Фінансовий відділ",
            Category = NewsCategory.Economy
        },
        new News
        {
            Id = 18,
            Title = "Наука: космічні місії та нові відкриття",
            Content = "Команди аналізують дані спостережень і уточнюють моделі, що пояснюють еволюцію об’єктів у далеких системах.",
            PublishedAt = new DateTime(2025, 12, 3, 17, 0, 0, DateTimeKind.Utc),
            Author = "Space Digest",
            Category = NewsCategory.Science
        },
        new News
        {
            Id = 19,
            Title = "Здоров'я: звички, що підтримують енергію протягом дня",
            Content = "Короткі перерви, контроль кофеїну та регулярний прийом їжі без переїдання — прості правила, які працюють у більшості випадків.",
            PublishedAt = new DateTime(2025, 12, 2, 7, 20, 0, DateTimeKind.Utc),
            Author = "Марія Коваль",
            Category = NewsCategory.Health
        },
        new News
        {
            Id = 20,
            Title = "Суспільство: локальні ініціативи з благоустрою",
            Content = "Міські команди запускають проєкти з оновлення просторів, освітлення та доступності інфраструктури.",
            PublishedAt = new DateTime(2025, 12, 1, 13, 10, 0, DateTimeKind.Utc),
            Author = "Анна Гринь",
            Category = NewsCategory.Society
        },
        new News
        {
            Id = 21,
            Title = "Екологія: як бізнес звітує про сталий розвиток",
            Content = "Більше компаній переходять на структуровані звіти, збирають дані про викиди та оцінюють ланцюги постачання.",
            PublishedAt = new DateTime(2025, 11, 30, 9, 0, 0, DateTimeKind.Utc),
            Author = "EcoWatch",
            Category = NewsCategory.Environment
        },
        new News
        {
            Id = 22,
            Title = "Подорожі: як економити на бронюваннях без ризику",
            Content = "Рекомендації включають гнучкі тарифи, порівняння умов скасування та планування поза піковими датами.",
            PublishedAt = new DateTime(2025, 11, 29, 18, 0, 0, DateTimeKind.Utc),
            Author = "Travel редакція",
            Category = NewsCategory.Travel
        },
        new News
        {
            Id = 23,
            Title = "Культура: книжкові підсумки року",
            Content = "У добірках критиків домінують нон-фікшн і сучасна проза, а також переклади, що відкривають нові імена.",
            PublishedAt = new DateTime(2025, 11, 28, 11, 30, 0, DateTimeKind.Utc),
            Author = "Літогляд",
            Category = NewsCategory.Culture
        },
        new News
        {
            Id = 24,
            Title = "Освіта: як підготуватись до сесії системно",
            Content = "Планування по темах, регулярні повторення й короткі тести допомагають зменшити стрес і підвищити якість запам’ятовування.",
            PublishedAt = new DateTime(2025, 11, 27, 9, 45, 0, DateTimeKind.Utc),
            Author = "EdTech огляд",
            Category = NewsCategory.Education
        },
        new News
        {
            Id = 25,
            Title = "Авто: електрифікація і інфраструктура зарядок",
            Content = "Ринок зростає разом із кількістю зарядних станцій; користувачі очікують прозорих тарифів і стабільної доступності.",
            PublishedAt = new DateTime(2025, 11, 26, 16, 10, 0, DateTimeKind.Utc),
            Author = "AutoDesk",
            Category = NewsCategory.Automotive
        },
        new News
        {
            Id = 26,
            Title = "Спорт: аналітика та дані в тренувальному процесі",
            Content = "Команди активніше використовують трекінг навантажень, відеоаналіз і метрики відновлення, щоб зменшити травматизм.",
            PublishedAt = new DateTime(2025, 11, 25, 19, 0, 0, DateTimeKind.Utc),
            Author = "Sports редакція",
            Category = NewsCategory.Sport
        },
        new News
        {
            Id = 27,
            Title = "Політика: комунікації та прозорість рішень",
            Content = "Публічні звіти та пояснення змін у регуляціях знижують напругу і підвищують довіру до процесів.",
            PublishedAt = new DateTime(2025, 11, 24, 10, 0, 0, DateTimeKind.Utc),
            Author = "Політичний огляд",
            Category = NewsCategory.Politics
        },
        new News
        {
            Id = 28,
            Title = "Лайфстайл: як сформувати корисну звичку за 30 днів",
            Content = "Найкраще працюють маленькі кроки, прив’язка до існуючого ритуалу та простий трекінг без перфекціонізму.",
            PublishedAt = new DateTime(2025, 11, 23, 8, 10, 0, DateTimeKind.Utc),
            Author = "Lifestyle Desk",
            Category = NewsCategory.Lifestyle
        },
        new News
        {
            Id = 29,
            Title = "Світ: огляд економічних трендів у різних регіонах",
            Content = "Ринки по-різному реагують на монетарну політику та логістичні ризики; бізнес диверсифікує постачання.",
            PublishedAt = new DateTime(2025, 11, 22, 15, 30, 0, DateTimeKind.Utc),
            Author = "International Desk",
            Category = NewsCategory.World
        },
        new News
        {
            Id = 30,
            Title = "Інше: короткий дайджест дня",
            Content = "Зібрали найпомітніші новини та оновлення з різних сфер, щоб швидко зорієнтуватися в подіях.",
            PublishedAt = new DateTime(2025, 11, 21, 9, 0, 0, DateTimeKind.Utc),
            Author = null,
            Category = NewsCategory.Other
        }
    );

            // -------- SEED: NewsImage (30) --------
            // Прості стабільні URL-адреси-заглушки (можеш замінити на свої)
            modelBuilder.Entity<NewsImage>().HasData(
                new NewsImage { Id = 1, Url = "https://picsum.photos/seed/news-1/800/450", NewsId = 1 },
                new NewsImage { Id = 2, Url = "https://picsum.photos/seed/news-2/800/450", NewsId = 2 },
                new NewsImage { Id = 3, Url = "https://picsum.photos/seed/news-3/800/450", NewsId = 3 },
                new NewsImage { Id = 4, Url = "https://picsum.photos/seed/news-4/800/450", NewsId = 4 },
                new NewsImage { Id = 5, Url = "https://picsum.photos/seed/news-5/800/450", NewsId = 5 },
                new NewsImage { Id = 6, Url = "https://picsum.photos/seed/news-6/800/450", NewsId = 6 },
                new NewsImage { Id = 7, Url = "https://picsum.photos/seed/news-7/800/450", NewsId = 7 },
                new NewsImage { Id = 8, Url = "https://picsum.photos/seed/news-8/800/450", NewsId = 8 },
                new NewsImage { Id = 9, Url = "https://picsum.photos/seed/news-9/800/450", NewsId = 9 },
                new NewsImage { Id = 10, Url = "https://picsum.photos/seed/news-10/800/450", NewsId = 10 },
                new NewsImage { Id = 11, Url = "https://picsum.photos/seed/news-11/800/450", NewsId = 11 },
                new NewsImage { Id = 12, Url = "https://picsum.photos/seed/news-12/800/450", NewsId = 12 },
                new NewsImage { Id = 13, Url = "https://picsum.photos/seed/news-13/800/450", NewsId = 13 },
                new NewsImage { Id = 14, Url = "https://picsum.photos/seed/news-14/800/450", NewsId = 14 },
                new NewsImage { Id = 15, Url = "https://picsum.photos/seed/news-15/800/450", NewsId = 15 },
                new NewsImage { Id = 16, Url = "https://picsum.photos/seed/news-16/800/450", NewsId = 16 },
                new NewsImage { Id = 17, Url = "https://picsum.photos/seed/news-17/800/450", NewsId = 17 },
                new NewsImage { Id = 18, Url = "https://picsum.photos/seed/news-18/800/450", NewsId = 18 },
                new NewsImage { Id = 19, Url = "https://picsum.photos/seed/news-19/800/450", NewsId = 19 },
                new NewsImage { Id = 20, Url = "https://picsum.photos/seed/news-20/800/450", NewsId = 20 },
                new NewsImage { Id = 21, Url = "https://picsum.photos/seed/news-21/800/450", NewsId = 21 },
                new NewsImage { Id = 22, Url = "https://picsum.photos/seed/news-22/800/450", NewsId = 22 },
                new NewsImage { Id = 23, Url = "https://picsum.photos/seed/news-23/800/450", NewsId = 23 },
                new NewsImage { Id = 24, Url = "https://picsum.photos/seed/news-24/800/450", NewsId = 24 },
                new NewsImage { Id = 25, Url = "https://picsum.photos/seed/news-25/800/450", NewsId = 25 },
                new NewsImage { Id = 26, Url = "https://picsum.photos/seed/news-26/800/450", NewsId = 26 },
                new NewsImage { Id = 27, Url = "https://picsum.photos/seed/news-27/800/450", NewsId = 27 },
                new NewsImage { Id = 28, Url = "https://picsum.photos/seed/news-28/800/450", NewsId = 28 },
                new NewsImage { Id = 29, Url = "https://picsum.photos/seed/news-29/800/450", NewsId = 29 },
                new NewsImage { Id = 30, Url = "https://picsum.photos/seed/news-30/800/450", NewsId = 30 }
            );
        }
}
}
