# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code and publish the application
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build-env /app/out ./

# Expose port 80 for HTTP traffic
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "QiECommerceAPI.dll"]
