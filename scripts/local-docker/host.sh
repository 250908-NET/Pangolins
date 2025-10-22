#!/bin/bash

function print_usage() {
    echo "Usage: ./pangolivia.sh [build|start|stop]"
    echo "  build    Build Docker images and create .env file if needed"
    echo "  start    Start Docker containers"
    echo "  stop     Stop Docker containers"
}

case "$1" in
    "build")
        docker build -t pangolivia-api:latest -f ../../Pangolivia.API/Dockerfile ../../Pangolivia.API
        docker build -t pangolivia-frontend:latest --build-arg VITE_API_URL=http://localhost:3001/api/ -f ../../Pangolivia.Frontend/Dockerfile ../../Pangolivia.Frontend
        if [ ! -f .env ]; then
            cat > .env <<'EOF'
OPENAI_API_KEY=
ConnectionString__Connection=
EOF
            echo "Add openai_api_key (optional) and a connection string to your .env file"
        fi
        ;;
    "start")
        docker compose up -d
        echo "Pangolivia: http://localhost:3000"
        ;;
    "stop")
        docker compose down
        ;;
    *)
        print_usage
        exit 1
        ;;
esac