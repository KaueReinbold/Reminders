ARG NODE_IMAGE

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

# Build the app
RUN npm run build && ls /app

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
