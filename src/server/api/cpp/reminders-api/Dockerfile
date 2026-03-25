FROM alpine:3.18 AS builder

RUN apk add --no-cache \
    cmake \
    g++ \
    make \
    curl \
    git \
    postgresql-dev \
    util-linux-dev \
    asio-dev

WORKDIR /app

COPY CMakeLists.txt .
COPY src/ src/

RUN cmake -B build -DCMAKE_BUILD_TYPE=Release \
    && cmake --build build --parallel $(nproc)

FROM alpine:3.18

RUN apk add --no-cache \
    libpq \
    libuuid \
    libstdc++ \
    libgcc \
    ca-certificates \
    curl

COPY --from=builder /app/build/reminders-api /reminders-api
COPY wait-for-dotnet.sh /wait-for-dotnet.sh

RUN chmod +x /wait-for-dotnet.sh /reminders-api

ENTRYPOINT ["/wait-for-dotnet.sh", "/reminders-api"]
