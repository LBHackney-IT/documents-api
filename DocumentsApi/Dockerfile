FROM mcr.microsoft.com/dotnet/sdk:6.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

RUN apt-get update \
     && apt-get install curl gnupg -yq \
     && curl -sL https://deb.nodesource.com/setup_14.x | bash \
     && apt-get install nodejs -yq

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./DocumentsApi/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./DocumentsApi ./
COPY ./.editorconfig ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

RUN cd ./V1/Node && npm ci

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/DocumentsApi.dll
