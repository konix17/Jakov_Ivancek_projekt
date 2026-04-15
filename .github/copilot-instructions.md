## UI/UX Delegation Rule — MANDATORY

This project has a specialized **UX Agent** subagent for all frontend work.

**EVERY TIME** a task involves ANY of the following, you MUST delegate to UX Agent instead of doing it yourself:
- Creating or editing any `.cshtml` file
- Any changes to `wwwroot/css/`
- Any changes to `_Layout.cshtml` or any partial view in `Views/Shared/`
- HTML structure, layout, navigation, sidebar, breadcrumbs
- Visual design, styling, color, typography
- Components: cards, tables, badges, buttons, KDA display, win/loss badges

**How to delegate:**
Invoke `UX Agent` and pass it:
1. What needs to be done (full task description)
2. Which entity or view is involved
3. Relevant file paths to read for context

**You handle directly** (do NOT delegate):
- `Controllers/*.cs`
- `Models/*.cs`
- `Services/*.cs`
- `Repositories/*.cs`
- `Program.cs`
- `appsettings.json`

> When generating UI code, spawn the UX sub-agent and keep the session transcript logable via the workspace hook.
