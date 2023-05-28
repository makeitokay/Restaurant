### build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore "Restaurant.sln"
RUN dotnet build "Restaurant.sln"

### publish
FROM build as publish
RUN dotnet publish AuthService -c Release -o out/AuthService
RUN dotnet publish RestaurantService -c Release -o out/RestaurantService

### AuthService
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as AuthService
WORKDIR /app
COPY --from=publish /app/out/AuthService .

EXPOSE 80

ENTRYPOINT ["dotnet", "AuthService.dll"]

### RestaurantService
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as RestaurantService
WORKDIR /app
COPY --from=publish /app/out/RestaurantService .

EXPOSE 80

ENTRYPOINT ["dotnet", "RestaurantService.dll"]