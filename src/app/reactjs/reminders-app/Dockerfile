# Use an official Node.js runtime as a parent image
ARG NODE_IMAGE=node:18-alpine

# Define the build stage
FROM ${NODE_IMAGE} as build-stage

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY src/app/reactjs/reminders-app/package*.json ./

# Install dependencies
RUN npm install

# Copy project files
COPY src/app/reactjs/reminders-app/. .

# Declare NEXT_PUBLIC_API_BASE_URL as a build argument
ARG NEXT_PUBLIC_API_BASE_URL

# Set NEXT_PUBLIC_API_BASE_URL as an environment variable
ENV NEXT_PUBLIC_API_BASE_URL=${NEXT_PUBLIC_API_BASE_URL}

# Build the app
RUN npm run build

# Define the production stage
FROM ${NODE_IMAGE} as production-stage

# Set the working directory
WORKDIR /app

# Copy the built app from the build stage
COPY --from=build-stage /app/public ./public
COPY --from=build-stage /app/package.json ./package.json
COPY --from=build-stage /app/.next ./.next
COPY --from=build-stage /app/node_modules ./node_modules

# Expose the port
EXPOSE 3000

# Define the command to run the app
CMD ["npm", "start"]
