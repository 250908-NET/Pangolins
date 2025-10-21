docker build -t pangolivia-api:latest -f ../../Pangolivia.API/Dockerfile ../../Pangolivia.API
docker build -t pangolivia-frontend:latest --build-arg VITE_API_URL=http://localhost:3001/api/ -f ../../Pangolivia.Frontend/Dockerfile ../../Pangolivia.Frontend
if [ ! -f .env ]; then
    cat > .env <<'EOF'
OPENAI_API_KEY=
ConnectionString__Connection=
EOF
fi
echo "Add openai_api_key (optional) and a connection string to your .env file"
