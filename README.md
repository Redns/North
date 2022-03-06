# ImageBed

![version: v1.0.0 (shields.io)](https://img.shields.io/badge/version-v1.0.0-green) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/.net-v6.0-orange) ![version: v1.0.0 (shields.io)](https://img.shields.io/badge/License-MIT-blue)

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
- `Github`容量虽不做限制，但国内访问速度堪忧。并且，官方命令禁止将`Github`仓库作为图床，说不定哪天就真动手了……
- `Gitee`访问速度虽然较快，但对普通用户的仓库容量做出了限制
- 对象存储的容量和访问速度还不错，但流量太贵了……说不定哪天博客火了，第二天房子就是阿里的了[滑稽]
- ……

<br>

说到底，图片只有掌握在自己手里才放心！出于这样的想法，笔者设计了 `ImageBed` 以方便大家搭建属于自己的图床服务器，您可以将该服务部署在任何主机上，无论是本地主机还是云服务器！

<br>

## Feature

- 安全可靠，图片完全存储在主机
- 无图片尺寸、带宽限制（取决于您的环境）
- 跨平台，可在 `windows`、`Linux`、`MacOS`部署

<br>

## Environment

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - [在 Windows 上安装 ](https://docs.microsoft.com/zh-cn/dotnet/core/install/windows?tabs=net60)
  - [在 macOS 上安装 .NET](https://docs.microsoft.com/zh-cn/dotnet/core/install/macos)
  - [在 Linux 发行版上安装 .NET](https://docs.microsoft.com/zh-cn/dotnet/core/install/linux)

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

1. 前往 [ImageBed主页](https://github.com/Redns/ImageBed/releases/tag/v1.0.0) 下载资源包（或者选择 [蓝奏云](https://wwz.lanzouv.com/i5uy4013bs8h)下载）

![image-20220306202309799](http://jing-image.test.upcdn.net/image-20220306202309799.png)

<br>

2. 解压资源包，内部应包含如下文件

![image-20220306205013617](http://jing-image.test.upcdn.net/image-20220306205013617.png)

<br>

3. 



### 云服务器搭建



## License

[license]
      
