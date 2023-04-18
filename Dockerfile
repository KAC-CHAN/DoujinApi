FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DoujinApi/DoujinApi.csproj", "DoujinApi/"]
RUN dotnet restore "DoujinApi/DoujinApi.csproj"
COPY . .
WORKDIR "/src/DoujinApi"
RUN dotnet build "DoujinApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoujinApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt update && apt install wget -y
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoujinApi.dll"]
