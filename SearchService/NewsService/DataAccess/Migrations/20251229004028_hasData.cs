using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class hasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NewsItems",
                columns: new[] { "Id", "Author", "Category", "Content", "PublishedAt", "Title" },
                values: new object[,]
                {
                    { 1, "Редакція", "Technology", "Аналітики відзначають зміщення фокусу з експериментів на промислові сценарії: підтримка клієнтів, пошук, автоматизація бек-офісу.", new DateTime(2025, 12, 20, 9, 15, 0, 0, DateTimeKind.Utc), "Ринок ШІ у 2025: головні напрями зростання" },
                    { 2, "Ірина Мельник", "Technology", "Компанії частіше впроваджують Zero Trust, посилюють контроль доступів і аудит привілеїв, а також тренують персонал.", new DateTime(2025, 12, 19, 14, 0, 0, 0, DateTimeKind.Utc), "Оновлення підходів до кібербезпеки у бізнесі" },
                    { 3, "Олег Савчук", "Economy", "Інвестори уважно стежать за інфляційними очікуваннями та динамікою ринку праці; на ринках зберігається помірна волатильність.", new DateTime(2025, 12, 18, 8, 30, 0, 0, DateTimeKind.Utc), "Економічний огляд: ключові показники тижня" },
                    { 4, "Лабораторний дайджест", "Science", "Дослідники повідомляють про поліпшення характеристик композитів, що може вплинути на енергоефективність та виробництво.", new DateTime(2025, 12, 17, 11, 10, 0, 0, DateTimeKind.Utc), "Наука: нові результати у дослідженнях матеріалів" },
                    { 5, "Марія Коваль", "Health", "Лікарі нагадують про важливість сну, питного режиму та збалансованого харчування; профілактика починається з рутини.", new DateTime(2025, 12, 16, 7, 45, 0, 0, DateTimeKind.Utc), "Здоров'я: як підтримувати режим у сезон застуд" },
                    { 6, "International Desk", "World", "Тиждень відзначився активізацією переговорів у кількох регіонах та новими заявами щодо торговельних обмежень.", new DateTime(2025, 12, 15, 16, 20, 0, 0, DateTimeKind.Utc), "Світ: підсумки ключових міжнародних подій" },
                    { 7, "Анна Гринь", "Society", "Громадські проєкти частіше залучають бізнес і локальні спільноти, формуючи сталі програми підтримки.", new DateTime(2025, 12, 14, 10, 5, 0, 0, DateTimeKind.Utc), "Суспільство: волонтерські ініціативи набирають обертів" },
                    { 8, "EcoWatch", "Environment", "У громадах з’являються нові пункти збору вторсировини; паралельно запускають інформаційні кампанії для мешканців.", new DateTime(2025, 12, 13, 12, 0, 0, 0, DateTimeKind.Utc), "Екологія: міста розширюють програми сортування" },
                    { 9, "Travel редакція", "Travel", "Поради стосуються безпеки в дорозі, страхування, а також планування маршрутів із запасом на погоду та затримки.", new DateTime(2025, 12, 12, 18, 35, 0, 0, DateTimeKind.Utc), "Подорожі: що врахувати при зимових поїздках" },
                    { 10, "Кіноколонка", "Culture", "Кінорелізи цього місяця роблять ставку на жанрове різноманіття — від драми до наукової фантастики.", new DateTime(2025, 12, 11, 13, 25, 0, 0, DateTimeKind.Utc), "Культура: прем’єри грудня та очікування критиків" },
                    { 11, "EdTech огляд", "Education", "Платформи для тестування, трекінгу прогресу і спільної роботи стають стандартом навіть у невеликих навчальних групах.", new DateTime(2025, 12, 10, 9, 0, 0, 0, DateTimeKind.Utc), "Освіта: цифрові інструменти для навчання" },
                    { 12, "AutoDesk", "Automotive", "ADAS-функції поширюються у масовому сегменті; виробники зосереджуються на точності сенсорів і надійності сценаріїв.", new DateTime(2025, 12, 9, 15, 40, 0, 0, DateTimeKind.Utc), "Авто: тренди у системах допомоги водієві" },
                    { 13, "Sports редакція", "Sport", "Команди демонструють щільну конкуренцію; ключовим фактором стає глибина складу та дисципліна у захисті.", new DateTime(2025, 12, 8, 20, 10, 0, 0, DateTimeKind.Utc), "Спорт: підсумки ігрового тижня" },
                    { 14, "Політичний огляд", "Politics", "У фокусі — бюджетні рішення, соціальні програми та регуляторні зміни, що можуть вплинути на бізнес-процеси.", new DateTime(2025, 12, 7, 10, 30, 0, 0, DateTimeKind.Utc), "Політика: пріоритети порядку денного на місяць" },
                    { 15, "Lifestyle Desk", "Lifestyle", "Експерти радять дробити цілі на короткі спринти, вимірювати прогрес та залишати простір для відновлення.", new DateTime(2025, 12, 6, 9, 50, 0, 0, DateTimeKind.Utc), "Лайфстайл: як планувати цілі без вигорання" },
                    { 16, "Tech огляд", "Technology", "Найчастіше обирають поєднання .NET/Java/Node для бекенду та React/Vue/Angular для фронтенду — залежно від команди і домену.", new DateTime(2025, 12, 5, 12, 15, 0, 0, DateTimeKind.Utc), "Технології: огляд популярних стеків для веб" },
                    { 17, "Фінансовий відділ", "Economy", "Серед підходів — перегляд портфеля проєктів, автоматизація операцій та перехід на більш прогнозовані контракти.", new DateTime(2025, 12, 4, 8, 5, 0, 0, DateTimeKind.Utc), "Економіка: як бізнес оптимізує витрати у 2025" },
                    { 18, "Space Digest", "Science", "Команди аналізують дані спостережень і уточнюють моделі, що пояснюють еволюцію об’єктів у далеких системах.", new DateTime(2025, 12, 3, 17, 0, 0, 0, DateTimeKind.Utc), "Наука: космічні місії та нові відкриття" },
                    { 19, "Марія Коваль", "Health", "Короткі перерви, контроль кофеїну та регулярний прийом їжі без переїдання — прості правила, які працюють у більшості випадків.", new DateTime(2025, 12, 2, 7, 20, 0, 0, DateTimeKind.Utc), "Здоров'я: звички, що підтримують енергію протягом дня" },
                    { 20, "Анна Гринь", "Society", "Міські команди запускають проєкти з оновлення просторів, освітлення та доступності інфраструктури.", new DateTime(2025, 12, 1, 13, 10, 0, 0, DateTimeKind.Utc), "Суспільство: локальні ініціативи з благоустрою" },
                    { 21, "EcoWatch", "Environment", "Більше компаній переходять на структуровані звіти, збирають дані про викиди та оцінюють ланцюги постачання.", new DateTime(2025, 11, 30, 9, 0, 0, 0, DateTimeKind.Utc), "Екологія: як бізнес звітує про сталий розвиток" },
                    { 22, "Travel редакція", "Travel", "Рекомендації включають гнучкі тарифи, порівняння умов скасування та планування поза піковими датами.", new DateTime(2025, 11, 29, 18, 0, 0, 0, DateTimeKind.Utc), "Подорожі: як економити на бронюваннях без ризику" },
                    { 23, "Літогляд", "Culture", "У добірках критиків домінують нон-фікшн і сучасна проза, а також переклади, що відкривають нові імена.", new DateTime(2025, 11, 28, 11, 30, 0, 0, DateTimeKind.Utc), "Культура: книжкові підсумки року" },
                    { 24, "EdTech огляд", "Education", "Планування по темах, регулярні повторення й короткі тести допомагають зменшити стрес і підвищити якість запам’ятовування.", new DateTime(2025, 11, 27, 9, 45, 0, 0, DateTimeKind.Utc), "Освіта: як підготуватись до сесії системно" },
                    { 25, "AutoDesk", "Automotive", "Ринок зростає разом із кількістю зарядних станцій; користувачі очікують прозорих тарифів і стабільної доступності.", new DateTime(2025, 11, 26, 16, 10, 0, 0, DateTimeKind.Utc), "Авто: електрифікація і інфраструктура зарядок" },
                    { 26, "Sports редакція", "Sport", "Команди активніше використовують трекінг навантажень, відеоаналіз і метрики відновлення, щоб зменшити травматизм.", new DateTime(2025, 11, 25, 19, 0, 0, 0, DateTimeKind.Utc), "Спорт: аналітика та дані в тренувальному процесі" },
                    { 27, "Політичний огляд", "Politics", "Публічні звіти та пояснення змін у регуляціях знижують напругу і підвищують довіру до процесів.", new DateTime(2025, 11, 24, 10, 0, 0, 0, DateTimeKind.Utc), "Політика: комунікації та прозорість рішень" },
                    { 28, "Lifestyle Desk", "Lifestyle", "Найкраще працюють маленькі кроки, прив’язка до існуючого ритуалу та простий трекінг без перфекціонізму.", new DateTime(2025, 11, 23, 8, 10, 0, 0, DateTimeKind.Utc), "Лайфстайл: як сформувати корисну звичку за 30 днів" },
                    { 29, "International Desk", "World", "Ринки по-різному реагують на монетарну політику та логістичні ризики; бізнес диверсифікує постачання.", new DateTime(2025, 11, 22, 15, 30, 0, 0, DateTimeKind.Utc), "Світ: огляд економічних трендів у різних регіонах" },
                    { 30, null, "Other", "Зібрали найпомітніші новини та оновлення з різних сфер, щоб швидко зорієнтуватися в подіях.", new DateTime(2025, 11, 21, 9, 0, 0, 0, DateTimeKind.Utc), "Інше: короткий дайджест дня" }
                });

            migrationBuilder.InsertData(
                table: "NewsImages",
                columns: new[] { "Id", "NewsId", "Url" },
                values: new object[,]
                {
                    { 1, 1, "https://picsum.photos/seed/news-1/800/450" },
                    { 2, 2, "https://picsum.photos/seed/news-2/800/450" },
                    { 3, 3, "https://picsum.photos/seed/news-3/800/450" },
                    { 4, 4, "https://picsum.photos/seed/news-4/800/450" },
                    { 5, 5, "https://picsum.photos/seed/news-5/800/450" },
                    { 6, 6, "https://picsum.photos/seed/news-6/800/450" },
                    { 7, 7, "https://picsum.photos/seed/news-7/800/450" },
                    { 8, 8, "https://picsum.photos/seed/news-8/800/450" },
                    { 9, 9, "https://picsum.photos/seed/news-9/800/450" },
                    { 10, 10, "https://picsum.photos/seed/news-10/800/450" },
                    { 11, 11, "https://picsum.photos/seed/news-11/800/450" },
                    { 12, 12, "https://picsum.photos/seed/news-12/800/450" },
                    { 13, 13, "https://picsum.photos/seed/news-13/800/450" },
                    { 14, 14, "https://picsum.photos/seed/news-14/800/450" },
                    { 15, 15, "https://picsum.photos/seed/news-15/800/450" },
                    { 16, 16, "https://picsum.photos/seed/news-16/800/450" },
                    { 17, 17, "https://picsum.photos/seed/news-17/800/450" },
                    { 18, 18, "https://picsum.photos/seed/news-18/800/450" },
                    { 19, 19, "https://picsum.photos/seed/news-19/800/450" },
                    { 20, 20, "https://picsum.photos/seed/news-20/800/450" },
                    { 21, 21, "https://picsum.photos/seed/news-21/800/450" },
                    { 22, 22, "https://picsum.photos/seed/news-22/800/450" },
                    { 23, 23, "https://picsum.photos/seed/news-23/800/450" },
                    { 24, 24, "https://picsum.photos/seed/news-24/800/450" },
                    { 25, 25, "https://picsum.photos/seed/news-25/800/450" },
                    { 26, 26, "https://picsum.photos/seed/news-26/800/450" },
                    { 27, 27, "https://picsum.photos/seed/news-27/800/450" },
                    { 28, 28, "https://picsum.photos/seed/news-28/800/450" },
                    { 29, 29, "https://picsum.photos/seed/news-29/800/450" },
                    { 30, 30, "https://picsum.photos/seed/news-30/800/450" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "NewsImages",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "NewsItems",
                keyColumn: "Id",
                keyValue: 30);
        }
    }
}
