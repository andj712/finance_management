# STEP 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0.412 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code and publish app
COPY . ./
RUN dotnet publish -c Release -o out

# STEP 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app from build stage
COPY --from=build /app/out .

# Expose port your app will listen on
EXPOSE 8080

# Set environment variable for ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Create Logs folder and set permissions
RUN mkdir -p /app/Logs

# Use non-root user (recommended)
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --gid 1001 appuser && \
    chown -R appuser:appgroup /app/Logs

USER appuser

# Set the entrypoint to run the app
ENTRYPOINT ["dotnet", "finance_management.dll"]
