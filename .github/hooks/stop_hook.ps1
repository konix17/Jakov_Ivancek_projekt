$hookDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$workspaceRoot = Resolve-Path -Path (Join-Path $hookDir "..\..")
$logDir = Join-Path $workspaceRoot "agent_logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$raw = [Console]::In.ReadToEnd()
try {
    $json = $raw | ConvertFrom-Json
} catch {
    Write-Output '{"continue":true}'
    exit
}

$transcriptPath = $json.transcript_path
$sessionId = $json.session_id
if (-not $transcriptPath -or -not (Test-Path $transcriptPath)) {
    Write-Output '{"continue":true}'
    exit
}

function Clean-UserContent($content) {
    if (-not $content) { return "" }
    $startTag = "<USER_REQUEST>"
    $endTag = "</USER_REQUEST>"
    if ($content.Contains($startTag) -and $content.Contains($endTag)) {
        $startIdx = $content.IndexOf($startTag) + $startTag.Length
        $endIdx = $content.IndexOf($endTag)
        return $content.Substring($startIdx, $endIdx - $startIdx).Trim()
    }
    if ($content.Contains("<ADDITIONAL_METADATA>")) {
        $content = $content.Split("<ADDITIONAL_METADATA>")[0]
    }
    if ($content.Contains("<USER_SETTINGS_CHANGE>")) {
        $content = $content.Split("<USER_SETTINGS_CHANGE>")[0]
    }
    return $content.Trim()
}

$toolTypeMap = @{
    'RUN_COMMAND' = 'run_command'
    'VIEW_FILE' = 'view_file'
    'LIST_DIRECTORY' = 'list_dir'
    'GREP_SEARCH' = 'grep_search'
    'REPLACE_FILE_CONTENT' = 'replace_file_content'
    'WRITE_TO_FILE' = 'write_to_file'
    'ASK_PERMISSION' = 'ask_permission'
    'ASK_QUESTION' = 'ask_question'
    'MULTI_REPLACE_FILE_CONTENT' = 'multi_replace_file_content'
    'BROWSER_SUBAGENT' = 'browser_subagent'
    'GENERATE_IMAGE' = 'generate_image'
    'READ_URL_CONTENT' = 'read_url_content'
    'SEARCH_WEB' = 'search_web'
    'SCHEDULE' = 'schedule'
    'MANAGE_TASK' = 'manage_task'
}

$dateStamp = Get-Date -Format "yyyy-MM-dd"
$timeStamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$outFile = Join-Path $logDir "chat_${dateStamp}_${sessionId}.md"

"# AI Session Log" | Set-Content $outFile
"" | Add-Content $outFile
"- session: $sessionId" | Add-Content $outFile
"- saved: $timeStamp" | Add-Content $outFile
"- date: $dateStamp" | Add-Content $outFile
"" | Add-Content $outFile

Get-Content $transcriptPath | ForEach-Object {
    try {
        $entry = $_ | ConvertFrom-Json
        $ts = if ($entry.timestamp) { $entry.timestamp } elseif ($entry.created_at) { $entry.created_at } else { $null }
        $time = if ($ts) {
            try {
                ([DateTimeOffset]::Parse($ts)).ToLocalTime().ToString('HH:mm:ss')
            } catch {
                $ts.Substring(11, 8)
            }
        } else { "??:??:??" }

        switch ($entry.type) {
            "user.message" {
                "" | Add-Content $outFile
                "## User [$time]" | Add-Content $outFile
                "" | Add-Content $outFile
                "$($entry.data.content)" | Add-Content $outFile
            }
            "assistant.message" {
                "" | Add-Content $outFile
                "## AI Assistant [$time]" | Add-Content $outFile
                "" | Add-Content $outFile
                "$($entry.data.content)" | Add-Content $outFile
            }
            "tool.execution_start" {
                "" | Add-Content $outFile
                "- **Tool start:** $($entry.data.toolName)" | Add-Content $outFile
            }
            "tool.execution_end" {
                "- **Tool end:** $($entry.data.toolName)" | Add-Content $outFile
            }
            "USER_INPUT" {
                "" | Add-Content $outFile
                "## User [$time]" | Add-Content $outFile
                "" | Add-Content $outFile
                $cleaned = Clean-UserContent $entry.content
                "$cleaned" | Add-Content $outFile
            }
            "PLANNER_RESPONSE" {
                if ($entry.content) {
                    "" | Add-Content $outFile
                    "## AI Assistant [$time]" | Add-Content $outFile
                    "" | Add-Content $outFile
                    "$($entry.content)" | Add-Content $outFile
                }
                if ($entry.tool_calls) {
                    foreach ($tc in $entry.tool_calls) {
                        if ($tc.name) {
                            "" | Add-Content $outFile
                            "- **Tool start:** $($tc.name)" | Add-Content $outFile
                        }
                    }
                }
            }
            Default {
                if ($toolTypeMap.ContainsKey($entry.type)) {
                    "- **Tool end:** $($toolTypeMap[$entry.type])" | Add-Content $outFile
                }
            }
        }
    } catch {
        # ignore malformed lines
    }
}

Write-Output '{"continue":true}'
