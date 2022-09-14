# SCHEDULER
Библиотека планировщика выполнения задач.

## ДОКУМЕНТАЦИЯ:
### Class SchedulerFactory

<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/1.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/3.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/4.png" alt="drawing" width="800"/> 
 
### Interface IMessage

<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/5.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/6.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/7.png" alt="drawing" width="800"/>  
 
## ПРИМЕР ИСПОЛЬЗОВАНИЯ: 

#### Формирование списка задач

using Microsoft.Extensions.Logging;

ILogger<ISchedulerFactory> logger = default;
ISchedulerFactory scheduler = new SchedulerFactory(logger);
scheduler.Create()
         .Do(Some_Func_1)
         .Do(Some_Func_2)
         .Do(Some_Func_3)
         .Start(Some_IMessage)
        


