
using System;
using DataAccess.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataAccess.Entities.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublishedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NewsItems");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "Редакція",
                            Category = "Technology",
                            Content = "Аналітики відзначають зміщення фокусу з експериментів на промислові сценарії: підтримка клієнтів, пошук, автоматизація бек-офісу.",
                            PublishedAt = new DateTime(2025, 12, 20, 9, 15, 0, 0, DateTimeKind.Utc),
                            Title = "Ринок ШІ у 2025: головні напрями зростання"
                        },
                        new
                        {
                            Id = 2,
                            Author = "Ірина Мельник",
                            Category = "Technology",
                            Content = "Компанії частіше впроваджують Zero Trust, посилюють контроль доступів і аудит привілеїв, а також тренують персонал.",
                            PublishedAt = new DateTime(2025, 12, 19, 14, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Оновлення підходів до кібербезпеки у бізнесі"
                        },
                        new
                        {
                            Id = 3,
                            Author = "Олег Савчук",
                            Category = "Economy",
                            Content = "Інвестори уважно стежать за інфляційними очікуваннями та динамікою ринку праці; на ринках зберігається помірна волатильність.",
                            PublishedAt = new DateTime(2025, 12, 18, 8, 30, 0, 0, DateTimeKind.Utc),
                            Title = "Економічний огляд: ключові показники тижня"
                        },
                        new
                        {
                            Id = 4,
                            Author = "Лабораторний дайджест",
                            Category = "Science",
                            Content = "Дослідники повідомляють про поліпшення характеристик композитів, що може вплинути на енергоефективність та виробництво.",
                            PublishedAt = new DateTime(2025, 12, 17, 11, 10, 0, 0, DateTimeKind.Utc),
                            Title = "Наука: нові результати у дослідженнях матеріалів"
                        },
                        new
                        {
                            Id = 5,
                            Author = "Марія Коваль",
                            Category = "Health",
                            Content = "Лікарі нагадують про важливість сну, питного режиму та збалансованого харчування; профілактика починається з рутини.",
                            PublishedAt = new DateTime(2025, 12, 16, 7, 45, 0, 0, DateTimeKind.Utc),
                            Title = "Здоров'я: як підтримувати режим у сезон застуд"
                        },
                        new
                        {
                            Id = 6,
                            Author = "International Desk",
                            Category = "World",
                            Content = "Тиждень відзначився активізацією переговорів у кількох регіонах та новими заявами щодо торговельних обмежень.",
                            PublishedAt = new DateTime(2025, 12, 15, 16, 20, 0, 0, DateTimeKind.Utc),
                            Title = "Світ: підсумки ключових міжнародних подій"
                        },
                        new
                        {
                            Id = 7,
                            Author = "Анна Гринь",
                            Category = "Society",
                            Content = "Громадські проєкти частіше залучають бізнес і локальні спільноти, формуючи сталі програми підтримки.",
                            PublishedAt = new DateTime(2025, 12, 14, 10, 5, 0, 0, DateTimeKind.Utc),
                            Title = "Суспільство: волонтерські ініціативи набирають обертів"
                        },
                        new
                        {
                            Id = 8,
                            Author = "EcoWatch",
                            Category = "Environment",
                            Content = "У громадах з’являються нові пункти збору вторсировини; паралельно запускають інформаційні кампанії для мешканців.",
                            PublishedAt = new DateTime(2025, 12, 13, 12, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Екологія: міста розширюють програми сортування"
                        },
                        new
                        {
                            Id = 9,
                            Author = "Travel редакція",
                            Category = "Travel",
                            Content = "Поради стосуються безпеки в дорозі, страхування, а також планування маршрутів із запасом на погоду та затримки.",
                            PublishedAt = new DateTime(2025, 12, 12, 18, 35, 0, 0, DateTimeKind.Utc),
                            Title = "Подорожі: що врахувати при зимових поїздках"
                        },
                        new
                        {
                            Id = 10,
                            Author = "Кіноколонка",
                            Category = "Culture",
                            Content = "Кінорелізи цього місяця роблять ставку на жанрове різноманіття — від драми до наукової фантастики.",
                            PublishedAt = new DateTime(2025, 12, 11, 13, 25, 0, 0, DateTimeKind.Utc),
                            Title = "Культура: прем’єри грудня та очікування критиків"
                        },
                        new
                        {
                            Id = 11,
                            Author = "EdTech огляд",
                            Category = "Education",
                            Content = "Платформи для тестування, трекінгу прогресу і спільної роботи стають стандартом навіть у невеликих навчальних групах.",
                            PublishedAt = new DateTime(2025, 12, 10, 9, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Освіта: цифрові інструменти для навчання"
                        },
                        new
                        {
                            Id = 12,
                            Author = "AutoDesk",
                            Category = "Automotive",
                            Content = "ADAS-функції поширюються у масовому сегменті; виробники зосереджуються на точності сенсорів і надійності сценаріїв.",
                            PublishedAt = new DateTime(2025, 12, 9, 15, 40, 0, 0, DateTimeKind.Utc),
                            Title = "Авто: тренди у системах допомоги водієві"
                        },
                        new
                        {
                            Id = 13,
                            Author = "Sports редакція",
                            Category = "Sport",
                            Content = "Команди демонструють щільну конкуренцію; ключовим фактором стає глибина складу та дисципліна у захисті.",
                            PublishedAt = new DateTime(2025, 12, 8, 20, 10, 0, 0, DateTimeKind.Utc),
                            Title = "Спорт: підсумки ігрового тижня"
                        },
                        new
                        {
                            Id = 14,
                            Author = "Політичний огляд",
                            Category = "Politics",
                            Content = "У фокусі — бюджетні рішення, соціальні програми та регуляторні зміни, що можуть вплинути на бізнес-процеси.",
                            PublishedAt = new DateTime(2025, 12, 7, 10, 30, 0, 0, DateTimeKind.Utc),
                            Title = "Політика: пріоритети порядку денного на місяць"
                        },
                        new
                        {
                            Id = 15,
                            Author = "Lifestyle Desk",
                            Category = "Lifestyle",
                            Content = "Експерти радять дробити цілі на короткі спринти, вимірювати прогрес та залишати простір для відновлення.",
                            PublishedAt = new DateTime(2025, 12, 6, 9, 50, 0, 0, DateTimeKind.Utc),
                            Title = "Лайфстайл: як планувати цілі без вигорання"
                        },
                        new
                        {
                            Id = 16,
                            Author = "Tech огляд",
                            Category = "Technology",
                            Content = "Найчастіше обирають поєднання .NET/Java/Node для бекенду та React/Vue/Angular для фронтенду — залежно від команди і домену.",
                            PublishedAt = new DateTime(2025, 12, 5, 12, 15, 0, 0, DateTimeKind.Utc),
                            Title = "Технології: огляд популярних стеків для веб"
                        },
                        new
                        {
                            Id = 17,
                            Author = "Фінансовий відділ",
                            Category = "Economy",
                            Content = "Серед підходів — перегляд портфеля проєктів, автоматизація операцій та перехід на більш прогнозовані контракти.",
                            PublishedAt = new DateTime(2025, 12, 4, 8, 5, 0, 0, DateTimeKind.Utc),
                            Title = "Економіка: як бізнес оптимізує витрати у 2025"
                        },
                        new
                        {
                            Id = 18,
                            Author = "Space Digest",
                            Category = "Science",
                            Content = "Команди аналізують дані спостережень і уточнюють моделі, що пояснюють еволюцію об’єктів у далеких системах.",
                            PublishedAt = new DateTime(2025, 12, 3, 17, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Наука: космічні місії та нові відкриття"
                        },
                        new
                        {
                            Id = 19,
                            Author = "Марія Коваль",
                            Category = "Health",
                            Content = "Короткі перерви, контроль кофеїну та регулярний прийом їжі без переїдання — прості правила, які працюють у більшості випадків.",
                            PublishedAt = new DateTime(2025, 12, 2, 7, 20, 0, 0, DateTimeKind.Utc),
                            Title = "Здоров'я: звички, що підтримують енергію протягом дня"
                        },
                        new
                        {
                            Id = 20,
                            Author = "Анна Гринь",
                            Category = "Society",
                            Content = "Міські команди запускають проєкти з оновлення просторів, освітлення та доступності інфраструктури.",
                            PublishedAt = new DateTime(2025, 12, 1, 13, 10, 0, 0, DateTimeKind.Utc),
                            Title = "Суспільство: локальні ініціативи з благоустрою"
                        },
                        new
                        {
                            Id = 21,
                            Author = "EcoWatch",
                            Category = "Environment",
                            Content = "Більше компаній переходять на структуровані звіти, збирають дані про викиди та оцінюють ланцюги постачання.",
                            PublishedAt = new DateTime(2025, 11, 30, 9, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Екологія: як бізнес звітує про сталий розвиток"
                        },
                        new
                        {
                            Id = 22,
                            Author = "Travel редакція",
                            Category = "Travel",
                            Content = "Рекомендації включають гнучкі тарифи, порівняння умов скасування та планування поза піковими датами.",
                            PublishedAt = new DateTime(2025, 11, 29, 18, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Подорожі: як економити на бронюваннях без ризику"
                        },
                        new
                        {
                            Id = 23,
                            Author = "Літогляд",
                            Category = "Culture",
                            Content = "У добірках критиків домінують нон-фікшн і сучасна проза, а також переклади, що відкривають нові імена.",
                            PublishedAt = new DateTime(2025, 11, 28, 11, 30, 0, 0, DateTimeKind.Utc),
                            Title = "Культура: книжкові підсумки року"
                        },
                        new
                        {
                            Id = 24,
                            Author = "EdTech огляд",
                            Category = "Education",
                            Content = "Планування по темах, регулярні повторення й короткі тести допомагають зменшити стрес і підвищити якість запам’ятовування.",
                            PublishedAt = new DateTime(2025, 11, 27, 9, 45, 0, 0, DateTimeKind.Utc),
                            Title = "Освіта: як підготуватись до сесії системно"
                        },
                        new
                        {
                            Id = 25,
                            Author = "AutoDesk",
                            Category = "Automotive",
                            Content = "Ринок зростає разом із кількістю зарядних станцій; користувачі очікують прозорих тарифів і стабільної доступності.",
                            PublishedAt = new DateTime(2025, 11, 26, 16, 10, 0, 0, DateTimeKind.Utc),
                            Title = "Авто: електрифікація і інфраструктура зарядок"
                        },
                        new
                        {
                            Id = 26,
                            Author = "Sports редакція",
                            Category = "Sport",
                            Content = "Команди активніше використовують трекінг навантажень, відеоаналіз і метрики відновлення, щоб зменшити травматизм.",
                            PublishedAt = new DateTime(2025, 11, 25, 19, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Спорт: аналітика та дані в тренувальному процесі"
                        },
                        new
                        {
                            Id = 27,
                            Author = "Політичний огляд",
                            Category = "Politics",
                            Content = "Публічні звіти та пояснення змін у регуляціях знижують напругу і підвищують довіру до процесів.",
                            PublishedAt = new DateTime(2025, 11, 24, 10, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Політика: комунікації та прозорість рішень"
                        },
                        new
                        {
                            Id = 28,
                            Author = "Lifestyle Desk",
                            Category = "Lifestyle",
                            Content = "Найкраще працюють маленькі кроки, прив’язка до існуючого ритуалу та простий трекінг без перфекціонізму.",
                            PublishedAt = new DateTime(2025, 11, 23, 8, 10, 0, 0, DateTimeKind.Utc),
                            Title = "Лайфстайл: як сформувати корисну звичку за 30 днів"
                        },
                        new
                        {
                            Id = 29,
                            Author = "International Desk",
                            Category = "World",
                            Content = "Ринки по-різному реагують на монетарну політику та логістичні ризики; бізнес диверсифікує постачання.",
                            PublishedAt = new DateTime(2025, 11, 22, 15, 30, 0, 0, DateTimeKind.Utc),
                            Title = "Світ: огляд економічних трендів у різних регіонах"
                        },
                        new
                        {
                            Id = 30,
                            Category = "Other",
                            Content = "Зібрали найпомітніші новини та оновлення з різних сфер, щоб швидко зорієнтуватися в подіях.",
                            PublishedAt = new DateTime(2025, 11, 21, 9, 0, 0, 0, DateTimeKind.Utc),
                            Title = "Інше: короткий дайджест дня"
                        });
                });

            modelBuilder.Entity("DataAccess.Entities.NewsImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("NewsId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NewsId");

                    b.ToTable("NewsImages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            NewsId = 1,
                            Url = "https://picsum.photos/seed/news-1/800/450"
                        },
                        new
                        {
                            Id = 2,
                            NewsId = 2,
                            Url = "https://picsum.photos/seed/news-2/800/450"
                        },
                        new
                        {
                            Id = 3,
                            NewsId = 3,
                            Url = "https://picsum.photos/seed/news-3/800/450"
                        },
                        new
                        {
                            Id = 4,
                            NewsId = 4,
                            Url = "https://picsum.photos/seed/news-4/800/450"
                        },
                        new
                        {
                            Id = 5,
                            NewsId = 5,
                            Url = "https://picsum.photos/seed/news-5/800/450"
                        },
                        new
                        {
                            Id = 6,
                            NewsId = 6,
                            Url = "https://picsum.photos/seed/news-6/800/450"
                        },
                        new
                        {
                            Id = 7,
                            NewsId = 7,
                            Url = "https://picsum.photos/seed/news-7/800/450"
                        },
                        new
                        {
                            Id = 8,
                            NewsId = 8,
                            Url = "https://picsum.photos/seed/news-8/800/450"
                        },
                        new
                        {
                            Id = 9,
                            NewsId = 9,
                            Url = "https://picsum.photos/seed/news-9/800/450"
                        },
                        new
                        {
                            Id = 10,
                            NewsId = 10,
                            Url = "https://picsum.photos/seed/news-10/800/450"
                        },
                        new
                        {
                            Id = 11,
                            NewsId = 11,
                            Url = "https://picsum.photos/seed/news-11/800/450"
                        },
                        new
                        {
                            Id = 12,
                            NewsId = 12,
                            Url = "https://picsum.photos/seed/news-12/800/450"
                        },
                        new
                        {
                            Id = 13,
                            NewsId = 13,
                            Url = "https://picsum.photos/seed/news-13/800/450"
                        },
                        new
                        {
                            Id = 14,
                            NewsId = 14,
                            Url = "https://picsum.photos/seed/news-14/800/450"
                        },
                        new
                        {
                            Id = 15,
                            NewsId = 15,
                            Url = "https://picsum.photos/seed/news-15/800/450"
                        },
                        new
                        {
                            Id = 16,
                            NewsId = 16,
                            Url = "https://picsum.photos/seed/news-16/800/450"
                        },
                        new
                        {
                            Id = 17,
                            NewsId = 17,
                            Url = "https://picsum.photos/seed/news-17/800/450"
                        },
                        new
                        {
                            Id = 18,
                            NewsId = 18,
                            Url = "https://picsum.photos/seed/news-18/800/450"
                        },
                        new
                        {
                            Id = 19,
                            NewsId = 19,
                            Url = "https://picsum.photos/seed/news-19/800/450"
                        },
                        new
                        {
                            Id = 20,
                            NewsId = 20,
                            Url = "https://picsum.photos/seed/news-20/800/450"
                        },
                        new
                        {
                            Id = 21,
                            NewsId = 21,
                            Url = "https://picsum.photos/seed/news-21/800/450"
                        },
                        new
                        {
                            Id = 22,
                            NewsId = 22,
                            Url = "https://picsum.photos/seed/news-22/800/450"
                        },
                        new
                        {
                            Id = 23,
                            NewsId = 23,
                            Url = "https://picsum.photos/seed/news-23/800/450"
                        },
                        new
                        {
                            Id = 24,
                            NewsId = 24,
                            Url = "https://picsum.photos/seed/news-24/800/450"
                        },
                        new
                        {
                            Id = 25,
                            NewsId = 25,
                            Url = "https://picsum.photos/seed/news-25/800/450"
                        },
                        new
                        {
                            Id = 26,
                            NewsId = 26,
                            Url = "https://picsum.photos/seed/news-26/800/450"
                        },
                        new
                        {
                            Id = 27,
                            NewsId = 27,
                            Url = "https://picsum.photos/seed/news-27/800/450"
                        },
                        new
                        {
                            Id = 28,
                            NewsId = 28,
                            Url = "https://picsum.photos/seed/news-28/800/450"
                        },
                        new
                        {
                            Id = 29,
                            NewsId = 29,
                            Url = "https://picsum.photos/seed/news-29/800/450"
                        },
                        new
                        {
                            Id = 30,
                            NewsId = 30,
                            Url = "https://picsum.photos/seed/news-30/800/450"
                        });
                });

            modelBuilder.Entity("DataAccess.Entities.NewsImage", b =>
                {
                    b.HasOne("DataAccess.Entities.News", "News")
                        .WithMany("Images")
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("News");
                });

            modelBuilder.Entity("DataAccess.Entities.News", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
