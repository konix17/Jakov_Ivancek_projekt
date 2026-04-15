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
        $time = if ($entry.timestamp) {
            try {
                ([DateTimeOffset]::Parse($entry.timestamp)).ToLocalTime().ToString('HH:mm:ss')
            } catch {
                $entry.timestamp.Substring(11, 8)
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
        }
    } catch {
        # ignore malformed lines
    }
}

Write-Output '{"continue":true}'
