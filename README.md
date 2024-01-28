# QueueSystem
欢迎了解我们<br>[http://vbp.cn](http://vbp.cn)<br><br>
[项目介绍文档](https://github.com/chen365409389/QueueSystem/blob/master/Doc/排队取号系统培训资料.pdf)<br><br>
已投入使用的排队叫号系统<br>
部分项目现场效果
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/1.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/2.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/3.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/4.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/5.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/WeChat.png)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/9.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/6.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/7.jpg)<br>
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/8.jpg)<br>
#### 系统主要功能包含了智能排队叫号，流程包含预约、取号、微信取号、绿色通道、服务评价、数据统计分析等，整合了多个硬件显示及控制端，包含了取号端，硬件叫号器（涉密机器不联机），软件叫号器，LED条屏端，综合显示屏端，消息服务端及语音端等，主要模块使用C/S架构，数据服务使用Remoting通信(可发布并兼容WebAPI)，消息使用Socket及WebSocket通信,并可基于消息服务自由进行业务扩展。<br>
#### 数据维护模块使用B/S架构，采用MVC模式开发，支持响应式布局（可在PC、平板、手机上浏览及操作）。<br>
#### 综合显示屏端使用安卓原生应用内嵌B/S方式开发，可实时更新及样式维护。<br><br>
### 项目简介<br>
#### Bin - 生成的程序集<br>
#### CallClient - 叫号客户端（独立叫号程序）<br>
#### CallSystem - 叫号系统（集成叫号器和评价器）<br>
#### Chloe-master - 数据库访问类<br>
#### LEDDisplay - LED条屏程序<br>
#### Lib - 外部引用程序集<br>
#### MessageService - 消息服务<br>
#### QueueClient - 排队叫号端(触摸屏上使用)<br>
#### RateService - 评价器服务
#### RemotingConfig - Remoting配置
#### SQL - 更新的SQL
#### ScreenDisplay - 综合显示屏<br>
#### ScreenDisplay_APP - 综合显示屏安卓版<br>
#### ScreenDisplay_Service - 综合显示屏服务<br>
#### Service - 数据服务<br>
#### SoundPlayer - 语音端<br>
#### SystemConfig - 系统配置<br>
#### WeChatService - 微信消息服务
