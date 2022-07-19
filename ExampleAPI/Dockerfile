FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExampleAPI.csproj", "ExampleAPI/"]
RUN dotnet restore "ExampleAPI/ExampleAPI.csproj"
COPY . .
WORKDIR "/src/ExampleAPI"
RUN dotnet build "ExampleAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExampleAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExampleAPI.dll"]
