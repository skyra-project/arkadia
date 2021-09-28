FROM mcr.microsoft.com/dotnet/sdk:6.0 AS BUILDER

COPY --from=ghcr.io/skyra-project/grpc-protofiles:latest . .

WORKDIR /skyra/app

COPY sources ./

RUN dotnet publish Notifications -r linux-x64 -p:PublishSingleFile=true -p:DebugType=None --self-contained true -c Release -o out

# ================ #
#   Runner Stage   #
# ================ #

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /skyra/app

COPY --from=BUILDER /skyra/app/out/Notifications .

ENTRYPOINT ["/skyra/app/Notifications"]
