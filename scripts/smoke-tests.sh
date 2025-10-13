#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${1:-${SMOKE_BASE_URL:-}}"
USERNAME="${SMOKE_USERNAME:-}"
PASSWORD="${SMOKE_PASSWORD:-}"
DEVICE_ID="${SMOKE_DEVICE_ID:-smoke-device}"
ANDROID_ID="${SMOKE_ANDROID_ID:-${DEVICE_ID}}"

if [[ -z "$BASE_URL" || -z "$USERNAME" || -z "$PASSWORD" ]]; then
  echo "Variáveis necessárias não informadas."
  echo "Defina SMOKE_BASE_URL, SMOKE_USERNAME e SMOKE_PASSWORD (ou passe a URL como primeiro argumento)."
  exit 1
fi

login_payload=$(jq -n \
  --arg user "$USERNAME" \
  --arg pass "$PASSWORD" \
  --arg android "$ANDROID_ID" \
  '{ username: $user, password: $pass, androidId: $android }')

response_file=$(mktemp)
status_code=$(curl -sS -o "$response_file" -w "%{http_code}" \
  -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d "$login_payload")

if [[ "$status_code" != "200" ]]; then
  echo "Login falhou (HTTP $status_code):"
  cat "$response_file"
  rm -f "$response_file"
  exit 1
fi

token=$(jq -r '.Token // empty' "$response_file")
if [[ -z "$token" ]]; then
  echo "Não foi possível extrair o token de autenticação:"
  cat "$response_file"
  rm -f "$response_file"
  exit 1
fi

echo "Login bem-sucedido. Token extraído."
rm -f "$response_file"

verify_code=$(curl -sS -o /dev/null -w "%{http_code}" \
  -H "Authorization: Bearer $token" \
  "$BASE_URL/api/Auth/verify")

if [[ "$verify_code" != "200" ]]; then
  echo "Falha ao verificar token (HTTP $verify_code)."
  exit 1
fi

echo "Token verificado com sucesso."

alerts_file=$(mktemp)
alerts_code=$(curl -sS -o "$alerts_file" -w "%{http_code}" \
  -H "Authorization: Bearer $token" \
  -H "x-device-id: $DEVICE_ID" \
  "$BASE_URL/api/Alert/GetRecentAlerts")

if [[ "$alerts_code" != "200" ]]; then
  echo "Falha ao consultar alertas (HTTP $alerts_code)."
  cat "$alerts_file"
  rm -f "$alerts_file"
  exit 1
fi

echo "Consulta de alertas executada (HTTP 200). Resumo:"
jq '.Message // "Lista de alertas retornada"' "$alerts_file" || cat "$alerts_file"
rm -f "$alerts_file"

echo "Smoke tests finalizados com sucesso."
