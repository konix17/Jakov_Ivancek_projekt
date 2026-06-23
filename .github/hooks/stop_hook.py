import json
import os
import sys
from datetime import datetime

hook_dir = os.path.dirname(os.path.abspath(__file__))
workspace_root = os.path.abspath(os.path.join(hook_dir, '..', '..'))
log_dir = os.path.join(workspace_root, 'agent_logs')
os.makedirs(log_dir, exist_ok=True)

raw = sys.stdin.read()
try:
    with open(os.path.join(log_dir, 'hook_debug.log'), 'a', encoding='utf-8') as f:
        f.write(f"--- Hook triggered at {datetime.now()} ---\n")
        f.write(raw)
        f.write("\n\n")
except Exception:
    pass

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

def clean_user_content(content):
    if not content:
        return ""
    # Try to extract content inside <USER_REQUEST>...</USER_REQUEST>
    start_tag = "<USER_REQUEST>"
    end_tag = "</USER_REQUEST>"
    if start_tag in content and end_tag in content:
        start_idx = content.find(start_tag) + len(start_tag)
        end_idx = content.find(end_tag)
        return content[start_idx:end_idx].strip()
    
    # Otherwise, clean up additional metadata blocks
    if "<ADDITIONAL_METADATA>" in content:
        content = content.split("<ADDITIONAL_METADATA>")[0]
    if "<USER_SETTINGS_CHANGE>" in content:
        content = content.split("<USER_SETTINGS_CHANGE>")[0]
    return content.strip()

TOOL_TYPE_MAP = {
    'RUN_COMMAND': 'run_command',
    'VIEW_FILE': 'view_file',
    'LIST_DIRECTORY': 'list_dir',
    'GREP_SEARCH': 'grep_search',
    'REPLACE_FILE_CONTENT': 'replace_file_content',
    'WRITE_TO_FILE': 'write_to_file',
    'ASK_PERMISSION': 'ask_permission',
    'ASK_QUESTION': 'ask_question',
    'MULTI_REPLACE_FILE_CONTENT': 'multi_replace_file_content',
    'BROWSER_SUBAGENT': 'browser_subagent',
    'GENERATE_IMAGE': 'generate_image',
    'READ_URL_CONTENT': 'read_url_content',
    'SEARCH_WEB': 'search_web',
    'SCHEDULE': 'schedule',
    'MANAGE_TASK': 'manage_task'
}

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

                # Parse timestamp
                timestamp = entry.get('timestamp') or entry.get('created_at')
                if timestamp:
                    try:
                        # Handle ISO format strings
                        parsed = datetime.fromisoformat(timestamp.replace('Z', '+00:00'))
                        time_label = parsed.astimezone().strftime('%H:%M:%S')
                    except Exception:
                        time_label = timestamp[11:19] if len(timestamp) >= 19 else '??:??:??'
                else:
                    time_label = '??:??:??'

                entry_type = entry.get('type')
                
                # Check for Old Format (VS Code / Copilot)
                if entry_type == 'user.message':
                    writer.write(f'\n## User [{time_label}]\n\n')
                    writer.write(f"{entry.get('data', {}).get('content', '')}\n")
                elif entry_type == 'assistant.message':
                    writer.write(f'\n## AI Assistant [{time_label}]\n\n')
                    writer.write(f"{entry.get('data', {}).get('content', '')}\n")
                elif entry_type == 'tool.execution_start':
                    writer.write('\n- **Tool start:** ' + entry.get('data', {}).get('toolName', '') + '\n')
                elif entry_type == 'tool.execution_end':
                    writer.write('- **Tool end:** ' + entry.get('data', {}).get('toolName', '') + '\n')
                
                # Check for New Format (Antigravity IDE / Gemini)
                elif entry_type == 'USER_INPUT':
                    cleaned_content = clean_user_content(entry.get('content', ''))
                    writer.write(f'\n## User [{time_label}]\n\n')
                    writer.write(f"{cleaned_content}\n")
                elif entry_type == 'PLANNER_RESPONSE':
                    # Log Assistant response if it has content
                    content = entry.get('content')
                    if content:
                        writer.write(f'\n## AI Assistant [{time_label}]\n\n')
                        writer.write(f"{content}\n")
                    
                    # Log tool starts
                    tool_calls = entry.get('tool_calls')
                    if tool_calls:
                        for tool_call in tool_calls:
                            tool_name = tool_call.get('name')
                            if tool_name:
                                writer.write(f'\n- **Tool start:** {tool_name}\n')
                elif entry_type in TOOL_TYPE_MAP:
                    writer.write(f'- **Tool end:** {TOOL_TYPE_MAP[entry_type]}\n')

    print(json.dumps({'continue': True}))
except Exception:
    print(json.dumps({'continue': True}))
    sys.exit(0)
