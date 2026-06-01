# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src/telegramBot

RUN apt-get update && \
    apt-get install -y --no-install-recommends clang zlib1g-dev curl && \
    rm -rf /var/lib/apt/lists/*

RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_linux \
    -o /src/yt-dlp && \
    chmod a+rx /src/yt-dlp
# App
COPY --link src/telegramBot/*.csproj .
RUN dotnet restore telegramBot.csproj

# Copy the rest of the source
COPY --link src/telegramBot/. .

# Publish as Native AOT

RUN dotnet publish -c Release -o /app/publish/telegramBot\
                   /p:PublishAot=true \
                   /p:TrimMode=full \
                   /p:PublishTrimmed=true \
                   /p:SelfContained=true \
                   /p:PublishSingleFile=true
# HealthC
WORKDIR /src/hc
COPY --link src/healthc/. .
RUN dotnet publish -c Release -o /app/publish/hc \
                   /p:PublishAot=true \
                   /p:TrimMode=full \
                   /p:InvariantGlobalization=true \
                   /p:PublishTrimmed=true \
                   /p:SelfContained=true \
                   /p:PublishSingleFile=true

# Runtime
FROM gcr.io/distroless/base-debian12:nonroot AS final

WORKDIR /app
COPY --link --from=build --chown=nonroot:nonroot /app/publish/telegramBot .
COPY --link --from=build --chown=nonroot:nonroot /src/yt-dlp .
COPY --link --from=build --chown=nonroot:nonroot /app/publish/hc .
COPY --from=build /usr/lib/x86_64-linux-gnu/libz.so.1 /usr/lib/x86_64-linux-gnu/libz.so.1


# Expose the default ASP.NET port
ENV ASPNETCORE_HTTP_PORTS=80
ENV PATH="/app:${PATH}"
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
    CMD ["/app/healthc"]

USER nonroot:nonroot

ENTRYPOINT ["/app/telegramBot"]
