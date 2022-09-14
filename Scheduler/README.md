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
<p>var msg = scheduler.Create().Do(Some_Func_1).Do(Some_Func_2).Do(Some_Func_3).Start(Some_IMessage);</p><br>

#### Формирование положительного сообщения<br><br>

<p>public class Message : IMessage</p><br>
<p>public class Data</p><br>

<p>Data data = new Data{.....};</p>
<p>IMessage msg = new Message(data, data.GetType());</p><br>

#### Формирование отрицательного сообщения с генерацией исключения в коде<br><br>

<p>public class Message : IMessage</p><br>
<p>IMessage msg = new Message(null, null, false, new Exception("error"));</p><br>

#### Формирование отрицательного сообщения без генерации исключения в коде<br><br>

<p>public class Message : IMessage</p><br>
<p>IMessage msg = new Message(null, null, false, new Exception("error"), true);</p><br>



        


