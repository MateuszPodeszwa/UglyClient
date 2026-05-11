FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ./UglyClient.sln
RUN dotnet publish ./BeautifulClient.csproj -c Release -f net10.0 -o /app/client --no-restore
RUN dotnet publish ./SensorServer/SensorServer.csproj -c Release -f net10.0 -o /app/server --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates tar \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://github.com/yudai/gotty/releases/download/v1.5.0/gotty_linux_amd64.tar.gz -o /tmp/gotty.tar.gz \
    && tar -xzf /tmp/gotty.tar.gz -C /tmp \
    && mv /tmp/gotty /usr/local/bin/gotty \
    && chmod +x /usr/local/bin/gotty \
    && rm -f /tmp/gotty.tar.gz

COPY --from=build /app/client /app/client
COPY --from=build /app/server /app/server
COPY ./deploy/start-render.sh /app/start-render.sh
COPY ./deploy/run-client-loop.sh /app/run-client-loop.sh

RUN chmod +x /app/start-render.sh /app/run-client-loop.sh

ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["/app/start-render.sh"]
