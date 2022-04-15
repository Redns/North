# 使用运行时镜像
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

# 设置工作目录
WORKDIR /app

# 把目录下的内容都复制到当前目录下
COPY . .

# 运行镜像入口命令和可执行文件名称
ENTRYPOINT ["dotnet", "ImageBed.dll"]