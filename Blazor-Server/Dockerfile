FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# Create dir in image
RUN mkdir /ImageBed
#RUN mkdir /ImageBed/Data
#RUN mkdir /ImageBed/Data/Logs
#RUN mkdir /ImageBed/Data/Database
#RUN mkdir /ImageBed/Data/Resources
#RUN mkdir /ImageBed/Data/Resources/Images

# Redefine the expose port
ENV ASPNETCORE_URLS=http://+:12121

# Copy files to ImageBed dir
COPY ./ /ImageBed
EXPOSE 12121
WORKDIR /ImageBed
CMD ["/bin/bash","-c","./ImageBed"]