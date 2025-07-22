# QueueSystem

Welcome to QueueSystem!
[Project Introduction Document (Chinese)](https://github.com/chen365409389/QueueSystem/blob/master/Doc/%E6%8E%92%E9%98%9F%E5%8F%96%E5%8F%B7%E7%B3%BB%E7%BB%9F%E5%9F%B9%E8%AE%AD%E8%B5%84%E6%96%99.pdf)

QueueSystem is a robust, production-proven solution for intelligent queue management and customer flow optimization. Designed for high-traffic environments such as government service centers, hospitals, banks, and corporate offices, QueueSystem streamlines the entire queuing process from ticketing to service evaluation.

## 🌟 Key Features
- Smart queuing and calling with support for appointments, on-site ticketing, WeChat integration, green channel, service evaluation, and comprehensive data analytics
- Unified management of multiple hardware and software terminals: ticketing kiosks, hardware and software callers (hardware caller devices for confidential environments, not networked), LED displays, integrated screens, messaging and voice services
- Modular architecture: core modules use C/S architecture for performance and reliability; data services leverage Remoting (compatible with WebAPI); messaging supports Socket and WebSocket for flexible integration
- Data maintenance module uses B/S architecture with responsive MVC design, accessible on PC, tablet, and mobile devices
- Integrated display terminal uses a native Android app with embedded B/S for real-time updates and easy style management
- Highly extensible: supports custom business logic and hardware integration via open messaging protocols
- Real-time monitoring and analytics dashboards for operational insights

## 📸 Real-World Deployments
QueueSystem has been successfully deployed in a variety of real-world scenarios. Below are some on-site photos:
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/1.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/2.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/3.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/4.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/5.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/WeChat.png)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/9.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/6.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/7.jpg)
![](https://github.com/chen365409389/QueueSystem/blob/master/Img/8.jpg)

## 🏗️ Project Structure
- **Bin** - Compiled assemblies
- **CallClient** - Standalone calling client (software caller)
- **CallSystem** - Integrated calling and evaluation system (includes hardware caller, not networked for confidential use)
- **Chloe-master** - Database access layer
- **LEDDisplay** - LED display program
- **Lib** - External libraries
- **MessageService** - Messaging service
- **QueueClient** - Queue and calling terminal (touchscreen)
- **RateService** - Evaluation service
- **RemotingConfig** - Remoting configuration
- **SQL** - SQL updates
- **ScreenDisplay** - Integrated display screen
- **ScreenDisplay_APP** - Android version of integrated display
- **ScreenDisplay_Service** - Display screen service
- **Service** - Data service
- **SoundPlayer** - Voice terminal
- **SystemConfig** - System configuration
- **WeChatService** - WeChat messaging service

## 📈 Typical Workflow
1. Customers can make appointments online or take a ticket on-site via self-service kiosks or WeChat.
2. The system manages the queue, calls customers to service counters, and displays real-time status on LED and integrated screens.
3. After service, customers can provide feedback and evaluation, which is collected for quality improvement.
4. Administrators access real-time analytics and reports to optimize operations and resource allocation.

## 💡 Why Choose QueueSystem?
- Proven stability and scalability in high-demand environments
- Flexible integration with existing hardware and business systems
- User-friendly interfaces for both customers and administrators
- Comprehensive support for customization and future expansion

---
For more information, [Contact Us](http://host.eastasia.cloudapp.azure.com)
