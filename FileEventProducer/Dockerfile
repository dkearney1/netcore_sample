FROM microsoft/dotnet:1.0.1-core

MAINTAINER dkearney1@gmail.com

LABEL	version = "1.0" \
	description = "File Event Producer"

RUN apt-get update \
	&& apt-get install -y --no-install-recommends \
		apt-utils \
	&& rm -rf /var/lib/apt/lists/*

WORKDIR /var/com.mitchell.oon

COPY ./appsettings.json fileeventproducer/
COPY bin/release/netcoreapp1.0/publish fileeventproducer/

ENTRYPOINT ["dotnet", "./fileeventproducer/FileEventProducer.dll"]