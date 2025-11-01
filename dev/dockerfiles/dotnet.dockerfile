FROM alpine as proj-env

WORKDIR /app

COPY . .

# On supprime les tests car les docker ignore c'est de la daube
RUN rm -rf tests

# On supprime tous les fichiers excepté les csproj
RUN find . ! -name '*.csproj' -type f -exec rm -f {} +

# On supprime les dossiers qui servent plus à rien
RUN find . -type d -empty -delete

# ----------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:9.0


#install debugger for NET Core
RUN apt update && apt install unzip
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

ARG PROJECT
ARG POST_RESTORE
ARG PRE_BUILD

WORKDIR /app/$PROJECT

COPY --from=proj-env /app /app

RUN dotnet restore
RUN $POST_RESTORE

COPY . /app

RUN $PRE_BUILD

RUN dotnet build --no-restore

ENTRYPOINT dotnet run --no-build --no-restore