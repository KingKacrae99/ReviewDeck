# Use Docker Hub aliases to bypass the mcr link parsing issue
FROM docker.io/library/buildpack-deps:bookworm-curl AS build-env

# Download and install the official .NET 8 SDK
RUN apt-get update && apt-get install -y wget \
    && wget https://dot.net -O dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Final runtime environment using a generic base image
FROM docker.io/library/debian:bookworm-slim AS final
RUN apt-get update && apt-get install -y libicu-dev libssl-dev && rm -rf /var/lib/apt/lists/*

# Copy the manually installed .NET runtime components from the build stage
COPY --from=build-env /usr/share/dotnet /usr/share/dotnet
LN -s /usr/share/dotnet/dotnet /usr/bin/dotnet

WORKDIR /app
COPY --from=build-env /app/publish .

# Expose port 8080 (Render's default port)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ReviewDeck.dll"]