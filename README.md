# AviaTickets
 Сервис поиска дешевых авиабилетов
 
 Пользовательское приложение WPF, реализован паттерн MVVM.
 
 СТРУКТУРА ПРОЕКТА: 
 MainWindow.xaml - Представление (главное окно приложения)
 TicketForm.xaml - Представление (элемент пользовательского контроля)
 appsettings.json - файл первоначальных настроек приложения
 Resources - ресурсы (*.jpg)
 ViewModel - модель представления.
 Dispatcher - диспетчер
 Processes - процессы
 Scheduler - планировщик
 Models - модели данных
 Abstractions - абстракции(интерфейсы)
 Converters - конверторы json сериализатора
 
ОПИСАНИЕ:
 
Через представление пользователь взаимодействует с приложением. Данные из представления, согласно паттерну MVVM, через Binding и Command передаются в модель представления. Модель представления, реагируя на действия пользователя, передает соответствующие команды в формате string Диспетчеру. Диспетчер анализирует полученные команды и запускает необходимые процессы в нужной последовательности. При запуске процесса, вызывается планировщик, который регулирует последовательность вызова методов в рамках процесса. Процессы взаимодействуют с моделями данных и представлением. 

Проект спроектирован согласно паттерну MVVM и принципам SOLID. 

Все этапы выполнения программы логгируются в файл логов посредством Serilog. 

Зависимости между абстракциями и классами устанавливаются через DI контейнеры Microsoft.Extensions.DependencyInjection

