
#!/usr/bin/env bash
# build.sh - build two docker images with an API URL build arg
# Usage: ./build.sh --api_url <url> [--tag <tag>] [--backend-dir <dir>] [--frontend-dir <dir>] [--prefix <name-prefix>]
set -euo pipefail

API_URL=""
BACKEND_DIR="."
FRONTEND_DIR="."

print_usage() {
    cat <<EOF
Usage: $0 --api_url <url> [--backend-dir <dir>] [--frontend-dir <dir>]

Only these three args are recognized:
    --api_url       (required) API base URL to bake into images
    --backend-dir   (optional) path to backend build context (default: .)
    --frontend-dir  (optional) path to frontend build context (default: .)

Example:
    $0 --api_url https://api.example.com --backend-dir ../api --frontend-dir ../web
EOF
}

# simple arg parsing (only three recognized args)
while [[ $# -gt 0 ]]; do
    case "$1" in
        --api_url|-a) API_URL="$2"; shift 2 ;;
        --backend-dir) BACKEND_DIR="$2"; shift 2 ;;
        --frontend-dir) FRONTEND_DIR="$2"; shift 2 ;;
        --help|-h) print_usage; exit 0 ;;
        *) echo "Unknown argument: $1"; print_usage; exit 1 ;;
    esac
done

if [[ -z "${API_URL}" ]]; then
    echo "Error: --api_url is required"
    print_usage
    exit 1
fi

echo "Building backend image from '${BACKEND_DIR}' with API_URL='${API_URL}'..."
docker build \
    -t "pangolivia-api:latest \
    "${BACKEND_DIR}"

echo "Building frontend image from '${FRONTEND_DIR}' with API_URL='${API_URL}'..."
docker build \
    --build-arg VITE_API_URL="${API_URL}" \
    -t "pangolivia-frontend:latest" \
    "${FRONTEND_DIR}"

echo "Builds complete: pangolivia-api:latest, pangolivia-frontend:latest"
