# AviaTickets
 Приложение поиска дешевых авиабилетов
 
 Пользовательское приложение WPF, реализован паттерн MVVM, принципы SOLID, конфигурирование, логгирование, внедрение зависимостей, splash window, работа с БД SQLite посредством Entity Framework Core, работа со сторонним API.
 
 ## СТРУКТУРА ПРОЕКТА: 
 
 MainWindow.xaml - Представление (главное окно приложения)
 
 TicketForm.xaml - Представление (элемент пользовательского контроля)
 
 appsettings.json - файл первоначальных настроек приложения
 
 mydb.db - БД SQLite
 
 Program.cs - настройки приложения
 
 Resources - ресурсы (*.jpg)
 
 DB - контекст данных
 
 Migrations - миграции
 
 Splash - Splash Window
 
 ViewModel - модель представления.
 
 Controller - контроллер
 
 Processes - процессы
 
 Scheduler - планировщик
 
 Models - модели данных
 
 Validator - Валидатор
 
 Converters - конверторы json сериализатора
 
## ОПИСАНИЕ:
 
При запуске приложения происходит его первоначальная настройка в Program.cs и запуск представления. Через представление пользователь взаимодействует с приложением. Данные из представления, согласно паттерну MVVM, через Binding и Command передаются в модель представления. Модель представления, реагируя на действия пользователя, передает соответствующие команды Контроллеру. Контроллер осуществляет запуск необходимых процессов. При запуске процесса, вызывается планировщик, который регулирует последовательность вызова методов в рамках процесса. Процессы взаимодействуют с моделями данных и представлением. В приложении осуществляется работа с БД SQLite посредством Entity Framework Core. Обновление БД осуществляется раз в неделю посредством http - запросов к стороннему API. Список авиабилетов формируется по результатам http - запросов к стороннему API.  

Проект спроектирован согласно паттерну MVVM и принципам SOLID. 

Все этапы выполнения программы логгируются в файл логов посредством Serilog. Валидация осуществляется FluentValidation. 

Зависимости между абстракциями и классами устанавливаются через DI контейнеры Microsoft.Extensions.DependencyInjection

#### Влияние использования библиотеки EFCore.BulkExtensions на время сохранения данных в БД : 

00:01.79  - SaveChanges();

00:01.89  - SaveChangesAsync();

00:00.23  - BulkInsertAsync();

