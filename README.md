# ImageBed

![version: v1.1.4 (shields.io)](https://img.shields.io/badge/version-v2.0.0-green) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/.net-v6.0-orange) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/License-MIT-blue)

<br>

## Background

我们在写博客时经常会用到图床，它可以把图片转为链接，从而方便分享我们的博客。目前，笔者接触过的图床主要包含

- 免费公共图床（聚合图床、ImgTP、postimage……）
- 利用`Git`仓库存储
- 对象存储（OSS、COS……）

<br>

但上述图床或多或少都有些不完美的地方，例如

- 公共图床往往会开启审查机制，可能造成数据泄露，而且其往往会限制上传图片的尺寸
- 公共图床的稳定性未知，万一哪天站长顶不住压力跑路就凉凉
- `Github`容量虽不做限制，但国内访问速度堪忧。并且，官方明令禁止将`Github`仓库作为图床，说不定哪天就真动手了……
- 对象存储的容量和访问速度还不错，但流量太贵了……说不定哪天博客火了，第二天房子就是阿里的了[滑稽]
- ……

<br>

说到底，图片只有掌握在自己手里才放心。出于这样的想法，笔者设计了 `ImageBed` 以方便大家搭建属于自己的图床服务器，您可以将该服务部署在任何主机上，包括本地主机和云服务器。

<br>

## Feature

- 安全可靠，图片完全存储在主机
- 无图片尺寸、带宽限制（取决于环境）
- 图片上传成功后自动将链接复制到剪贴板
- 可视化图片管理
- 跨平台，可在 `windows`、`Linux`、`MacOS`部署

<br>

## Environment

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - [在 Windows 上安装 ](https://docs.microsoft.com/zh-cn/dotnet/core/install/windows?tabs=net60)
  - [在 macOS 上安装 .NET](https://docs.microsoft.com/zh-cn/dotnet/core/install/macos)
  - [在 Linux 发行版上安装 .NET](https://docs.microsoft.com/zh-cn/dotnet/core/install/linux)
- [Nginx](https://nginx.org/en/)（非必须）

<br>

## Usage

笔者在此将演示如何在 `本地Windows主机` 和 `Linux云服务器` 上搭建 `ImageBed` 服务，系统版本分别如下

- `本地Windows主机`
  - `版本`：Windows 11 家庭中文版（21H2）
  - `操作系统版本`：22000.434
  - `体验`：Windows 功能体验包 1000.22000.434.0
- `Linux云服务器`
  - `版本`：Ubuntu 18.04.4 LTS（GNU/Linux 4.15.0-159-generic x86_64）

<br>

### 本地搭建

1. 前往 [ImageBed主页](https://github.com/Redns/ImageBed/releases/tag/v2.0.0) 下载资源包，请根据主机架构选择

   ![image-20220410201522203](http://imagebed.krins.cloud/api/image/52DEA9A44C69AF7728F9731B2B3BA610.png)

<br>

2. 解压资源包，内部应包含如下文件（图片存储在 Data/Resources/Images 文件夹下）

   ![image-20220410201629481](http://imagebed.krins.cloud/api/image/2A8369FF5A9632F8DAEB6BE3F8C3267E.png)

<br>

3. 双击 `ImageBed.exe` 运行服务

   ![image-20220327212743452](http://imagebed.krins.cloud/api/image/3453F13C3C4085B5C959950F68389446.png)

   ![image-20220327212810352](http://imagebed.krins.cloud/api/image/55D3F488ED98C28008CE34DFC7E9FE0F.png)

<br>

4. 打开浏览器，输入 `localhost:12121`

   ![image-20220410201954327](http://imagebed.krins.cloud/api/image/E6FE45489B288F1956E5EF34BD6F4089.png)



<br>

5. 点击绿色区域即可上传图片，上传完成后会有弹窗提示并将链接复制到剪贴板

   ![image-20220410202028674](http://imagebed.krins.cloud/api/image/CC9CEA389A43DB3B1A8EF62064D8B815.png)

<br>

6. 保持 `ImageBed.exe` 运行即可

	<br>

### 云服务器搭建

这里以 `腾讯云服务器` 为例，其他的云服务器配置流程相似

1. 将资源包解压后上传至云服务器

   ![image-20220327213013364](http://imagebed.krins.cloud/api/image/29519474D012465D3682702CD05B1146.png)

    <br>

2. 安装 `nginx`（若想要直接通过云服务器公网IP、域名访问则需要配置反向代理，否则不必安装）

   ```shell
   sudo apt-get install nginx
   ```

	<br>

3. 打开 `/etc/nginx/nginx.conf`，修改相关设置

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

<br>

4. 检查 `nginx.conf` 格式是否正确

   ```shell
   sudo nginx -t
   ```

   若格式正确则输出

   ```sh
   nginx: the configuration file /etc/nginx/nginx.conf syntax is ok
   nginx: configuration file /etc/nginx/nginx.conf test is successful
   ```

   <br>

5. 关闭防火墙并重新加载 `nginx`

   ```shell
   sudo systemctl stop firewalld
   sudo systemctl start girewalld
   sudo firewall-cmd --reload
   sudo nginx -s reload
   ```

   <br>

6. 进入 `ImageBed` 文件夹，运行 `ImageBed`

   ```shell
   nohup dotnet ImageBed.dll &
   ```

   该命令会在后台运行 `ImageBed`，若要关闭 `ImageBed` 服务，需要先查询 `ImageBed` 服务的 `pid`，之后用 `kill` 命令关闭

   ```shell
   # 查询ImageBed服务pid
   ps -ef | grep dotnet
   
   # pid为4363
   ubuntu    4363  1636  0 19:32 pts/0    00:00:01 dotnet ImageBed.dll
   ubuntu   31389 30239  0 21:47 pts/2    00:00:00 grep --color=auto dotnet
   
   # 关闭ImageBed服务
   ubuntu@VM-0-16-ubuntu:~$ sudo kill 4363
   ```

   <br>

7. 大功告成！

   <br>

## API

该图床服务器包含三个API（上传、下载、删除），大家也可自行修改 `Controllers/ImageController` 中的内容来修改 API

<br>

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

<br>

### 下载图片

`HTTP` `GET` /api/image/{imageName}

> REST参数

|  参数名   |   类型   | 必填 |   说明   |
| :-------: | :------: | :--: | :------: |
| imageName | [string] |  是  | 图片名称 |

> 返回参数 `Binary`

| 图片文件 |
| -------- |

<br>

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

<br>

## Q & A

### S1: 局域网用户如何上传图片？

局域网内用户在浏览器输入 `{ip}:12121` 即可访问图床服务器，其中 `{ip}` 为服务器的 `局域网IP`

<br>

### S2.如何对接Picgo?

[PicGo](https://picgo.github.io/PicGo-Doc/zh/) 是一款图床管理软件，支持多种图床。使用 `picgo` 可大大简化我们上传图片的流程，笔者在此开发了 `picgo` 插件 [picgo-plugin-imagebed](https://github.com/Redns/picgo-plugin-imagebed) 以供大家使用

<br>

[![Star History Chart](https://api.star-history.com/svg?repos=Redns/ImageBed&type=Date)](https://star-history.com/#Redns/ImageBed&Date)
