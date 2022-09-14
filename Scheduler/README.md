# SCHEDULER
Библиотека планировщика выполнения задач.<br><br>

## ДОКУМЕНТАЦИЯ: <br><br>
### Class SchedulerFactory<br><br>

<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/1.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/3.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/4.png" alt="drawing" width="800"/><br><br> 
 
### Interface IMessage<br><br>

<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/5.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/6.png" alt="drawing" width="800"/>
<img src="https://github.com/Presstomsk/AviaTickets/blob/main/Scheduler/docfx_project/_site/api/DocFX/7.png" alt="drawing" width="800"/><br><br>  
 
## ПРИМЕР ИСПОЛЬЗОВАНИЯ:<br><br> 

#### Формирование списка задач<br><br>

<p>using Microsoft.Extensions.Logging;</p><br>

<p>ILogger<ISchedulerFactory> logger = default;</p>
<p>ISchedulerFactory scheduler = new SchedulerFactory(logger);</p>
<p>scheduler.Create()</p>
<p style="margin-left: 40px">         .Do(Some_Func_1)</p>
<p>         .Do(Some_Func_2)</p>
<p>         .Do(Some_Func_3)</p>
<p>         .Start(Some_IMessage)</p>
        


