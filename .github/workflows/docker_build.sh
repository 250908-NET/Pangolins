#!/bin/bash

# Set the version tag (you can pass this as an argument or use git tags)
VERSION=${1:-latest}

# Build the API image
echo "Building API Docker image..."
docker build -t pangolivia-api:${VERSION} -f Pangolivia.API/Dockerfile Pangolivia.API/

# Build the Frontend image
echo "Building Frontend Docker image..."
docker build -t pangolivia-frontend:${VERSION} -f Pangolivia.Frontend/Dockerfile Pangolivia.Frontend/

echo "Docker images built successfully!"
echo "API image: pangolivia-api:${VERSION}"
echo "Frontend image: pangolivia-frontend:${VERSION}"


