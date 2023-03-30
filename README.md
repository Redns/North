<p align="center" style="font-size:60px; font-weight:700; color:#23BFDF">
   <img width="240" src="img/snow.svg" alt="Logo">
    North
</p>

![version: v1.1.4 (shields.io)](https://img.shields.io/badge/release-v2.2.13-green) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/.net-6.0-orange) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/License-MIT-blue)

> 抱歉朋友们，由于毕设赶进度，暂缓推进该项目。感谢各位朋友的关注与支持，该项目不会停止开发，后续将先更新一个不包含插件系统的版本。

## Recent Progress

1. 用户登录授权系统

   ![image-20221202212357589](img/登录页面.png)

2. 全新风格 UI

   ![image-20221202212640017](img/主页面.png)

3. 插件化开发

   ![image-20221202212734313](img/插件化_图片上传流程.png)

   ![](img/插件上下文架构.png)

4. 其他

   - 支持 docker 部署
   - 可视化安装、迁移
   - 支持用户组策略、图片权限设置
   - 多数据库支持（MySql、SQL Server、Oracle、Sqlite、PostgreSQL、……）
   - ……

## Background

图床可以把图片转为链接，从而方便我们书写、分享博客，目前图床主要分为以下几类

- 利用 `Git` 仓库存储
- 对象存储（OSS、COS、七牛云等）
- 免费公共图床（SM.MS、聚合图床、ImgTP、Postimage等）

但上述图床都有些不尽人意

- 公共图床的稳定性未知，往往会开启审查机制、限制上传图片的尺寸
- `Github` 国内访问速度堪忧，并且官方明令禁止将 `Github` 和 `Gitee` 仓库作为图床
- 对象存储的容量和访问速度还不错，但流量较贵

图床服务并不需要高性能的服务器去承载，现在云服务器的价格也并不昂贵，因此搭建个人图床也许是不错的选择。出于这样的想法，作者设计了 `ImageBed` 以供大家快速搭建个人图床服务。

## Feature

- 剪贴板图片上传、链接自动复制
- 可视化图片管理
- 安全可靠，图片完全存储在主机
- 无图片尺寸、数量、带宽限制（取决于环境）
- 跨平台，可在 `windows`、`Linux`、`MacOS` 部署

## Install

#### Requirement

- [.NET Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime) (必需)
  - [在 Windows 上安装 ](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-6.0.4-windows-hosting-bundle-installer)
  - [在 Linux 上安装 .NET](https://docs.microsoft.com/zh-cn/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)
  - [在 macOS 上安装 .NET](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-6.0.4-macos-x64-installer)
- [Nginx](https://nginx.org/en/) (可选)

#### All Release

> 在这里可快速选择适合自己的版本，表格中的格式为 "最小版本/ 推荐版本"

|     功 能      |                        ImageBed 版本                         |                picgo-plugin-imagebed版本                 |
| :------------: | :----------------------------------------------------------: | :------------------------------------------------------: |
| 图片上传与下载 | [v1.0.0](https://github.com/Redns/ImageBed/releases/tag/v1.0.0) / [v1.1.4](https://github.com/Redns/ImageBed/releases/tag/v1.1.4) | [v1.1.1](https://github.com/Redns/picgo-plugin-imagebed) |
|    图库管理    | [v2.0.0](https://github.com/Redns/ImageBed/releases/tag/v2.0.0) / [v2.2.13](https://github.com/Redns/ImageBed/releases/tag/v2.2.13) |                                                          |
|    自动更新    | [v2.3.14](https://github.com/Redns/ImageBed/releases/tag/v2.3.14) / [v2.3.14](https://github.com/Redns/ImageBed/releases/tag/v2.3.14) |                                                          |

#### 本地搭建

> 版本:  Windows 11 家庭中文版 (21H2)

1. 前往 [ImageBed主页](https://github.com/Redns/ImageBed/releases) 下载资源包

   ![image-20220506144854125](https://jing-image.oss-cn-chengdu.aliyuncs.com/image-20220506144854125.png)

2. 解压资源包 (图片存储路径为 `Data/Resources/Images`)

   ![Z62J08FN](https://jing-image.oss-cn-chengdu.aliyuncs.com/Z62J08FN.png)

3. 双击 `ImageBed.exe` 运行服务

   ![V6X4644N](https://jing-image.oss-cn-chengdu.aliyuncs.com/V6X4644N.png)

4. 浏览器地址栏输入 `localhost:12121`

   ![8PD0X4VR](https://jing-image.oss-cn-chengdu.aliyuncs.com/8PD0X4VR.png)

5. 点击上传图片

   ![RJ6V26VJ](https://jing-image.oss-cn-chengdu.aliyuncs.com/RJ6V26VJ.png)

   ![F4TNFTTD](https://jing-image.oss-cn-chengdu.aliyuncs.com/F4TNFTTD.png)

6. 安装完成


#### 云服务器搭建

> 版本: Ubuntu 18.04.4 LTS（GNU/Linux 4.15.0-159-generic x86_64）

1. 将资源包解压后上传至云服务器

   ![08HLTD88](https://jing-image.oss-cn-chengdu.aliyuncs.com/08HLTD88.png)

2. 进入 `ImageBed` 文件夹，运行 `ImageBed.dll`

   ```shell
   nohup dotnet ImageBed.dll &
   ```

   该命令会在后台运行 `ImageBed.dll`，若要关闭 `ImageBed` 服务，需要先查询 `ImageBed` 服务的 `pid`，之后用 `kill` 命令关闭

   ```shell
   # 查询ImageBed服务pid
   ps -ef | grep dotnet
   
   # pid为4363
   ubuntu    4363  1636  0 19:32 pts/0    00:00:01 dotnet ImageBed.dll
   ubuntu   31389 30239  0 21:47 pts/2    00:00:00 grep --color=auto dotnet
   
   # 关闭ImageBed服务
   ubuntu@VM-0-16-ubuntu:~$ sudo kill 4363
   ```

3. 浏览器地址栏输入 `{ip}:12121` 即可访问图床服务，其中 `{ip}` 为云服务器公网 `ip`

4. 安装完成


#### Nginx 反向代理

反向代理概念、原理、功能请 [移步](https://juejin.cn/post/6958987684383555592)，这里不再赘述。下面主要叙述如何在 `Linux` 上搭建 `Nginx` 反相代理

1. 安装 `nginx`

   ```shell
   sudo apt-get install nginx
   ```

2. 打开 `/etc/nginx/nginx.conf`，修改相关设置

   ```nginx
   user www-data;
   worker_processes auto;
   pid /run/nginx.pid;
   include /etc/nginx/modules-enabled/*.conf;
   
   events {
   	  worker_connections 768;
   }
   
   http {
      sendfile on;
      tcp_nopush on;
      tcp_nodelay on;
      keepalive_timeout 65;
      types_hash_max_size 20480;
   
      include /etc/nginx/mime.types;
      default_type application/octet-stream;
   
      ssl_protocols TLSv1 TLSv1.1 TLSv1.2; # Dropping SSLv3, ref: POODLE
      ssl_prefer_server_ciphers on;
   
      access_log /var/log/nginx/access.log;
      error_log /var/log/nginx/error.log;
   
      gzip on;
   
      map $http_connection $connection_upgrade {
        "~*Upgrade" $http_connection;
        default keep-alive;
      }
   
      # ========================== 重点看这里========================
      server {
          listen 80;
          server_name xxx.xxx.xxx.xxx;				                    # 云服务器公网ip (或域名)     
   
          location / {
              client_max_body_size 100m;                          		# html报文尺寸限制
              
              proxy_pass http://127.0.0.1:12121;
   
              # Configuration for WebSockets
              proxy_set_header Upgrade $http_upgrade;
                    proxy_set_header Connection $connection_upgrade;
                    proxy_cache off;
               # WebSockets were implemented after http/1.0
              proxy_http_version 1.1;
   
              # Configuration for ServerSentEvents
              proxy_buffering off;
   
              # Configuration for LongPolling or if your KeepAliveInterval is longer than 60 seconds
              proxy_read_timeout 100s;
   
              proxy_set_header Host $host;
              proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
              proxy_set_header X-Forwarded-Proto $scheme;
          }
      }
   
   	  include /etc/nginx/conf.d/*.conf;
   	  include /etc/nginx/sites-enabled/*;
   }
   ```

3. 检查 `nginx.conf` 语法是否正确

   ```shell
   sudo nginx -t
   ```

   若格式正确则输出

   ```sh
   nginx: the configuration file /etc/nginx/nginx.conf syntax is ok
   nginx: configuration file /etc/nginx/nginx.conf test is successful
   ```

4. 关闭防火墙并重新加载 `nginx`

   ```shell
   sudo systemctl stop firewalld
   sudo systemctl start girewalld
   sudo firewall-cmd --reload
   sudo nginx -s reload
   ```


## Usage

#### 上传

**web 界面上传**

![web界面上传](img/上传图片.gif)

**剪贴板上传**

![剪贴板上传](img/剪贴板上传.gif)

#### 删除

![删除图片](img/删除图片.gif)

#### 导入导出

**导入图片**

![导入图片](img/导入图片.gif)

**导出图片**

![导出图片](img/导出图片.gif)

#### 视图切换

![视图切换](img/视图切换.gif)

## API

该图床服务器包含三个API（上传、下载、删除），控制器为 `Controllers/ImageController`

### 上传图片

`HTTP` `POST` /api/image

> Body 请求参数 `Form-data`

| 参数名 |  类型  | 必填 |   说明   |
| :----: | :----: | :--: | :------: |
|   *    | [file] |  是  | 图片文件 |

> 返回参数 `JSON` `最外层结构为: Object`

|   参数名   |   类型   | 必含 |   说明   |
| :--------: | :------: | :--: | :------: |
| statusCode |  [int]   |  是  |  状态码  |
|  message   | [string] |  是  | 提示信息 |
|    res     | [array]  |  是  | 图片链接 |

### 下载图片

`HTTP` `GET` /api/image/{imageName}

> REST参数

|  参数名   |   类型   | 必填 |   说明   |
| :-------: | :------: | :--: | :------: |
| imageName | [string] |  是  | 图片名称 |

> 返回参数 `Binary`

| 图片文件 |
| -------- |

### 删除图片

`HTTP` `DELETE` /api/image/{imageName}

> REST参数

|  参数名   |   类型   | 必填 |   说明   |
| :-------: | :------: | :--: | :------: |
| imageName | [string] |  是  | 图片名称 |

> 返回参数 `JSON` `最外层结构为: Object`

|   参数名   |   类型   | 必含 |   说明   |
| :--------: | :------: | :--: | :------: |
| statusCode |  [int]   |  是  |  状态码  |
|  message   | [string] |  是  | 提示信息 |
|    res     | [object] |  是  | 恒为null |

## Q & A

### S1. 局域网用户如何上传图片？

局域网内用户在浏览器输入 `{ip}:12121` 即可访问图床服务器，其中 `{ip}` 为服务器的 `局域网IP`

### S2. 如何对接Picgo?

[PicGo](https://picgo.github.io/PicGo-Doc/zh/) 是一款图床管理软件，支持多种图床。使用 `picgo` 可大大简化我们上传图片的流程，笔者在此开发了 `picgo` 插件 [picgo-plugin-imagebed](https://github.com/Redns/picgo-plugin-imagebed) 以供大家使用

### S3. 导入图片压缩包无响应？

请尝试直接压缩图片，而不是包含图片的文件夹

### S4. 仪表盘界面未出现统计数据

仪表盘正常显示时的界面如下

![2F4TD6N2](https://jing-image.oss-cn-chengdu.aliyuncs.com/2F4TD6N2.png)

下方的统计曲线默认每天统计一次昨日的数据，统计时间可在设置中选择，第一天安装没有数据曲线是正常的。若长时间未显示曲线请确保程序版本高于 `2.1.4`，仍未解决请在 [Issue](https://github.com/Redns/ImageBed/issues) 提问并附上日志 (路径为 `Data/Logs/imagebed.log`)。

### S5. 通知设置中的邮箱授权码如何获取？

请移步 [这里](https://service.mail.qq.com/cgi-bin/help?subtype=1&&no=1001256&&id=28)

### S6. 修改页脚后页面未改变？

请尝试刷新页面

[![Star History Chart](https://api.star-history.com/svg?repos=Redns/ImageBed&type=Date)](https://star-history.com/#Redns/ImageBed&Date)
