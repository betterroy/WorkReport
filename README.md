# 如果对您有帮助，您可以点右上角 "Star" 支持一下。

公司后台管理系统，汇报日报周报月报后台管理系统WorkReport

# 概述

项目是基于Net6.0+SQL Server进行开发的框架。
前端使用Layui及LAYUI MINI作为框架
整合应用技术包括

## 后端

Asp.NetCore MVC、EF Core、Autofac、Log4Net、AutoMapper。

## 前端

Layui为主。

## 第三方中间件

Redis、RabbitMQ。

# 核心功能
- **登陆鉴权**、
- **异常日志**中间件
- **读写分离**模块、
- **Service自动注入**、
- 公共返回帮助类库等。
- 增加了**Redis缓存中间件**，使用**AOP**在登陆时，进行菜单读取及写入缓存，过期后重新读取功能。
- **Excel文件导入**，**文件上传**，word、excel**在线**打开显示功能。
- 增加**JWT**鉴权授权及**双Token**方式。
- 权限分配功能，读取特性标签，来自动增加菜单及控制器。

# 亮点功能
- 首页日志增删通过RabbitMQ进行消峰写入。
- 首页日志从Redis进行查询，每次增删改，对Redis进行更新。

# 功能图片
## 登录页
![image](https://user-images.githubusercontent.com/39639296/175870976-de92998f-6d81-49be-afc2-21402748ab22.png)
## 登陆首页：
![image](https://user-images.githubusercontent.com/39639296/175872429-dd643182-d0ab-47f7-994f-d9151e457c92.png)
## 菜单管理
![image](https://user-images.githubusercontent.com/39639296/175871116-beb5a54c-cc85-4bae-9189-9dfe7085a069.png)
## 文件管理功能及在线打开：
![image](https://user-images.githubusercontent.com/39639296/175871509-af274205-5ce9-4e2f-89ed-5dbafac75354.png)
![image](https://user-images.githubusercontent.com/39639296/175871538-5a309160-d609-4d65-a196-46ba7de961b5.png)
## 用户模板下载及导入：
![image](https://user-images.githubusercontent.com/39639296/175871644-60b53af5-d535-4a15-9395-266c6cf9cada.png)
## 字典表配置：
![image](https://user-images.githubusercontent.com/39639296/175871704-edc18698-9c8c-4e1e-833d-f4ce5c170987.png)
## 权限管理：
![image](https://user-images.githubusercontent.com/39639296/175871752-285ac645-11bb-433a-bfc5-20736d7cc792.png)
## 邮件管理：
![image](https://user-images.githubusercontent.com/39639296/175871813-24468564-0745-4987-9616-e17ec12070ec.png)

# 后续会对项目进行持续的输出
