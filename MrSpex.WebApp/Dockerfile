#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MrSpex.WebApp/MrSpex.WebApp.csproj", "MrSpex.WebApp/"]
COPY ["MrSpex.AppServices/MrSpex.AppServices.csproj", "MrSpex.AppServices/"]
COPY ["MrSpex.SharedKernel/MrSpex.SharedKernel.csproj", "MrSpex.SharedKernel/"]
COPY ["MrSpex.Domain/MrSpex.Domain.csproj", "MrSpex.Domain/"]
COPY ["MrSpex.Data/MrSpex.Data.csproj", "MrSpex.Data/"]
RUN dotnet restore "MrSpex.WebApp/MrSpex.WebApp.csproj"
COPY . .
WORKDIR "/src/MrSpex.WebApp"
RUN dotnet build "MrSpex.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MrSpex.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrSpex.WebApp.dll"]
