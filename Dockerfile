FROM alpine:3

COPY src/GreenCobra.Client/bin/Release/net6.0/linux-x64/publish/ .

ENTRYPOINT ["green-cobra-linux-x64"]