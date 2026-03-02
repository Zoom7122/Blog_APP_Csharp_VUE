FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY blogApp/ ./
RUN dotnet restore BlogAPP_API/BlogAPP_API.csproj
RUN dotnet publish BlogAPP_API/BlogAPP_API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__Default="Data Source=/app/data/db.db"

RUN mkdir -p /app/data
COPY blogApp/BlogAPP_DAL/db/db.db /app/data/db.db
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BlogAPP_API.dll"]
