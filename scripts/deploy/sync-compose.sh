#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Uso: $0 <staging|production>"
  exit 1
fi

ENVIRONMENT="$1"
case "$ENVIRONMENT" in
  staging)
    HOST="${STAGING_SSH_HOST:?Defina STAGING_SSH_HOST}"
    USER="${STAGING_SSH_USER:?Defina STAGING_SSH_USER}"
    DEST="${STAGING_WORKDIR:?Defina STAGING_WORKDIR}"
    ENV_FILE="deploy/staging/.env.staging"
    ;;
  production)
    HOST="${PRODUCTION_SSH_HOST:?Defina PRODUCTION_SSH_HOST}"
    USER="${PRODUCTION_SSH_USER:?Defina PRODUCTION_SSH_USER}"
    DEST="${PRODUCTION_WORKDIR:?Defina PRODUCTION_WORKDIR}"
    ENV_FILE="deploy/production/.env.production"
    ;;
  *)
    echo "Ambiente inválido: $ENVIRONMENT"
    exit 1
    ;;
esac

SSH_PORT="${SSH_PORT:-22}"
SSH_KEY="${SSH_KEY_PATH:-$HOME/.ssh/id_rsa}"

echo "Enviando arquivos para $USER@$HOST:$DEST (porta $SSH_PORT)"

scp -P "$SSH_PORT" -i "$SSH_KEY" docker-compose.yml "$USER@$HOST:$DEST/docker-compose.yml"

if [[ -f "$ENV_FILE" ]]; then
  scp -P "$SSH_PORT" -i "$SSH_KEY" "$ENV_FILE" "$USER@$HOST:$DEST/.env"
else
  echo "Arquivo $ENV_FILE não encontrado; crie-o a partir do template correspondente antes do deploy."
fi
