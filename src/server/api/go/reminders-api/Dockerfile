ARG GO_IMAGE

# Start from golang base image
FROM ${GO_IMAGE} as builder

# Enable go modules
ENV GO111MODULE=on

# Install git. (alpine image does not have git in it)
RUN apk update && apk add --no-cache git

# Set current working directory
WORKDIR /app

# Note here: To avoid downloading dependencies every time we
# build image. Here, we are caching all the dependencies by
# first copying go.mod and go.sum files and downloading them,
# to be used every time we build the image if the dependencies
# are not changed.

# Copy go mod and sum files
COPY go.mod ./
COPY go.sum ./

# Download all dependencies.
RUN go mod download

# Now, copy the source code
COPY . .

# Note here: CGO_ENABLED is disabled for cross system compilation
# It is also a common best practise.

# Build the application.
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o ./bin/main ./cmd/app/main.go

# Finally our multi-stage to build a small image
# Start a new stage from scratch
FROM alpine:3.18

# Install runtime deps for the wait script (curl)
RUN apk add --no-cache ca-certificates curl

# Copy the Pre-built binary file
COPY --from=builder /app/bin/main /main

# Copy wait script from builder stage so final image can wait for dotnet
COPY --from=builder /app/wait-for-dotnet.sh /wait-for-dotnet.sh

RUN chmod +x /wait-for-dotnet.sh /main

# Make the wait script the container entrypoint so the Go binary starts
# only after the Dotnet API health check passes.
ENTRYPOINT ["/wait-for-dotnet.sh", "/main"]
