# Build stage
FROM node:20-alpine AS deps
WORKDIR /app
# Only copy the files needed to install dependencies for better caching
COPY package*.json ./
RUN npm ci --no-audit --no-fund

FROM node:20-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NODE_ENV=production
ARG VITE_API_URL=https://pangolivia-api-bpdehvd0b4bmcwhj.canadacentral-01.azurewebsites.net/api/
ENV VITE_API_URL=${VITE_API_URL}
# Build the Vite app (runs tsc -b && vite build per package.json)
RUN npm run build

# Runtime stage
FROM nginx:1.27-alpine AS runner
# Copy built assets
COPY --from=build /app/dist /usr/share/nginx/html

# Configure Nginx for SPA routing and sensible caching
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
