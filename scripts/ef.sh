#!/usr/bin/env bash
set -euo pipefail

# EF Core helper for Pangolivia.API
# Commands:
#   add <Name>                    Add a new migration
#   update [Migration]            Apply migrations to the database (default: latest)
#   list                          List migrations
#   script [-i] [From] [To]       Generate SQL script (use -i/--idempotent for idempotent)
#   remove                        Remove the last migration (if not applied)
#   drop                          Drop the database (force)
#   dbinfo                        Show DbContext info
# Options:
#   -e, --env <Environment>       ASPNETCORE_ENVIRONMENT (Development/Production/etc.)
#   -c, --context <DbContext>     Override DbContext full name
#   -v, --verbose                 Verbose output
#   -h, --help                    Show usage

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
API_DIR="$ROOT_DIR/Pangolivia.API"
PROJECT="$API_DIR/Pangolivia.API.csproj"
STARTUP="$API_DIR/Pangolivia.API.csproj"
DEFAULT_CONTEXT="Pangolivia.API.Data.PangoliviaDbContext"
MIGRATIONS_DIR="$API_DIR/Migrations"
ARTIFACTS_DIR="$ROOT_DIR/artifacts/migrations"

# Defaults
ENVIRONMENT=""
CONTEXT="$DEFAULT_CONTEXT"
VERBOSE=""

usage() {
  sed -n '1,50p' "$0" | sed 's/^# \{0,1\}//'
}

ensure_prereqs() {
  if ! command -v dotnet >/dev/null 2>&1; then
    echo "Error: dotnet SDK not found. Install .NET SDK: https://dotnet.microsoft.com/download" >&2
    exit 1
  fi
  if ! dotnet ef --help >/dev/null 2>&1; then
    echo "Warning: dotnet-ef tool not found." >&2
    echo "Install globally:  dotnet tool install --global dotnet-ef" >&2
    echo "or add local tool:  dotnet new tool-manifest && dotnet tool install dotnet-ef" >&2
  fi
}

# Parse options
ARGS=()
while [[ ${1:-} ]]; do
  case "$1" in
    -e|--env)
      ENVIRONMENT="${2:-}"; shift 2 ;;
    -c|--context)
      CONTEXT="${2:-}"; shift 2 ;;
    -v|--verbose)
      VERBOSE="--verbose"; shift ;;
    -h|--help)
      usage; exit 0 ;;
    --)
      shift; break ;;
    -*)
      echo "Unknown option: $1" >&2; usage; exit 1 ;;
    *)
      ARGS+=("$1"); shift ;;
  esac
done

set -- "${ARGS[@]}" "$@"

CMD="${1:-}"; shift || true

if [[ -z "$CMD" ]]; then
  usage; exit 1
fi

if [[ ! -f "$PROJECT" ]]; then
  echo "Error: Project not found at $PROJECT" >&2
  exit 1
fi

ensure_prereqs

# Environment
if [[ -n "$ENVIRONMENT" ]]; then
  export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"
fi

common_flags=(
  --project "$PROJECT"
  --startup-project "$STARTUP"
  --context "$CONTEXT"
)

case "$CMD" in
  add)
    NAME="${1:-}"
    if [[ -z "$NAME" ]]; then
      echo "Usage: $0 add <Name> [options]" >&2
      exit 1
    fi
    mkdir -p "$MIGRATIONS_DIR"
    dotnet ef migrations add "$NAME" "${common_flags[@]}" --output-dir "$MIGRATIONS_DIR" $VERBOSE
    ;;

  update)
    TARGET="${1:-}"
    if [[ -n "$TARGET" ]]; then
      dotnet ef database update "$TARGET" "${common_flags[@]}" $VERBOSE
    else
      dotnet ef database update "${common_flags[@]}" $VERBOSE
    fi
    ;;

  list)
    dotnet ef migrations list "${common_flags[@]}" $VERBOSE
    ;;

  script)
    IDEMP=""
    if [[ "${1:-}" == "-i" || "${1:-}" == "--idempotent" ]]; then
      IDEMP="--idempotent"; shift
    fi
    FROM="${1:-}"; TO="${2:-}"
    mkdir -p "$ARTIFACTS_DIR"
    ts=$(date +%Y%m%d_%H%M%S)
    envsfx="${ASPNETCORE_ENVIRONMENT:-Default}"
    out="$ARTIFACTS_DIR/${ts}_${envsfx}.sql"
    if [[ -n "$FROM" && -n "$TO" ]]; then
      dotnet ef migrations script "$FROM" "$TO" "${common_flags[@]}" $IDEMP --output "$out" $VERBOSE
    elif [[ -n "$FROM" ]]; then
      dotnet ef migrations script "$FROM" "${common_flags[@]}" $IDEMP --output "$out" $VERBOSE
    else
      dotnet ef migrations script "${common_flags[@]}" $IDEMP --output "$out" $VERBOSE
    fi
    echo "Script written: $out"
    ;;

  remove)
    dotnet ef migrations remove "${common_flags[@]}" $VERBOSE
    ;;

  drop)
    dotnet ef database drop -f --project "$PROJECT" --startup-project "$STARTUP" $VERBOSE
    ;;

  dbinfo)
    dotnet ef dbcontext info "${common_flags[@]}" $VERBOSE
    ;;

  *)
    echo "Unknown command: $CMD" >&2
    usage
    exit 1
    ;;
 esac
