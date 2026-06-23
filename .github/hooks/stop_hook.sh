#!/usr/bin/env sh
hook_dir="$(cd "$(dirname "$0")" && pwd)"
workspace_root="$(cd "$hook_dir/../.." && pwd)"
log_dir="$workspace_root/agent_logs"
mkdir -p "$log_dir"

if command -v python3 >/dev/null 2>&1; then
  PYTHON=python3
elif command -v python >/dev/null 2>&1; then
  PYTHON=python
else
  echo '{"continue":true}'
  exit 0
fi

exec "$PYTHON" "$hook_dir/stop_hook.py"
