import json
import os
import sys
from datetime import datetime, timezone

hook_dir = os.path.dirname(os.path.abspath(__file__))
workspace_root = os.path.abspath(os.path.join(hook_dir, '..', '..'))
log_dir = os.path.join(workspace_root, 'agent_logs')
os.makedirs(log_dir, exist_ok=True)

raw = sys.stdin.read()
try:
    payload = json.loads(raw)
except json.JSONDecodeError:
    print(json.dumps({'continue': True}))
    sys.exit(0)

transcript_path = payload.get('transcript_path')
session_id = payload.get('session_id')
if not transcript_path or not os.path.exists(transcript_path):
    print(json.dumps({'continue': True}))
    sys.exit(0)

try:
    date_stamp = datetime.now().strftime('%Y-%m-%d')
    time_stamp = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    out_file = os.path.join(log_dir, f'chat_{date_stamp}_{session_id}.md')

    with open(out_file, 'w', encoding='utf-8') as writer:
        writer.write('# AI Session Log\n\n')
        writer.write(f'- session: {session_id}\n')
        writer.write(f'- saved: {time_stamp}\n')
        writer.write(f'- date: {date_stamp}\n\n')

        with open(transcript_path, 'r', encoding='utf-8') as transcript_file:
            for line in transcript_file:
                line = line.strip()
                if not line:
                    continue
                try:
                    entry = json.loads(line)
                except json.JSONDecodeError:
                    continue

                timestamp = entry.get('timestamp')
                if timestamp:
                    try:
                        parsed = datetime.fromisoformat(timestamp.replace('Z', '+00:00'))
                        time_label = parsed.astimezone().strftime('%H:%M:%S')
                    except Exception:
                        time_label = timestamp[11:19] if len(timestamp) >= 19 else '??:??:??'
                else:
                    time_label = '??:??:??'

                if entry.get('type') == 'user.message':
                    writer.write(f'\n## User [{time_label}]\n\n')
                    writer.write(f"{entry.get('data', {}).get('content', '')}\n")
                elif entry.get('type') == 'assistant.message':
                    writer.write(f'\n## AI Assistant [{time_label}]\n\n')
                    writer.write(f"{entry.get('data', {}).get('content', '')}\n")
                elif entry.get('type') == 'tool.execution_start':
                    writer.write('\n- **Tool start:** ' + entry.get('data', {}).get('toolName', '') + '\n')
                elif entry.get('type') == 'tool.execution_end':
                    writer.write('- **Tool end:** ' + entry.get('data', {}).get('toolName', '') + '\n')

    print(json.dumps({'continue': True}))
except Exception:
    print(json.dumps({'continue': True}))
    sys.exit(0)
