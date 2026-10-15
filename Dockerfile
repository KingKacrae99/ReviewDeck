# Use the official Microsoft .NET SDK image to build the app
FROM ://microsoft.com AS build
WORKDIR /src

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/publish

# Use the ASP.NET runtime image to run the app
FROM ://microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (Render's default port)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ReviewDeck.dll"]
