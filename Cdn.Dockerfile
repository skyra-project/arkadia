FROM mcr.microsoft.com/dotnet/sdk:6.0 AS BUILDER

COPY --from=ghcr.io/skyra-project/grpc-protofiles:latest /skyra /skyra

WORKDIR /skyra/app

COPY sources .
COPY Directory.Build.props .

RUN dotnet publish Cdn -p:PublishTrimmed=true -p:PublishSingleFile=true -p:DebugType=None -r linux-x64 --self-contained -c Release -o out

# ================ #
#   Runner Stage   #
# ================ #

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0

WORKDIR /skyra/app

COPY --from=BUILDER /skyra/app/out/Cdn .

ENTRYPOINT ["/skyra/app/Cdn"]
