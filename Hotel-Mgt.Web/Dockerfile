FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Hotel-Mgt.Web/Hotel-Mgt.Web.csproj", "Hotel-Mgt.Web/"]
COPY ["Hotel-Mgt.Model/Hotel-Mgt.Model.csproj", "Hotel-Mgt.Model/"]
RUN dotnet restore "Hotel-Mgt.Web/Hotel-Mgt.Web.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/Hotel-Mgt.Web"
RUN dotnet publish "Hotel-Mgt.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Setup the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY Hotel-Mgt.Web/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

# Expose port 8080 for the ASP.NET Core app (default in .NET 8+)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["/app/entrypoint.sh"]
