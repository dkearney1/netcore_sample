FROM microsoft/dotnet:1.0.1-core

MAINTAINER dkearney1@gmail.com

LABEL	version = "1.0" \
	description = "Received File Event Consumer"

RUN apt-get update \
	&& apt-get install -y --no-install-recommends --force-yes \
		apt-utils \
	&& rm -rf /var/lib/apt/lists/*

WORKDIR /var/com.mitchell.oon

COPY ./appsettings.json receivedfileeventconsumer/
COPY bin/release/netcoreapp1.0/publish receivedfileeventconsumer/

ENTRYPOINT ["dotnet", "./receivedfileeventconsumer/ReceivedFileEventConsumer.dll"]