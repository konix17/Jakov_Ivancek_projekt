# Model Context Protocol (MCP) Server Setup Guide

The `Hotel-Mgt.Console` project has been updated to support running as an Model Context Protocol (MCP) server. This allows AI coding assistants (like Claude Desktop, Windsurf, Cursor, or standard Gemini agent) to directly connect to the Hotel Management Database and use tools to inspect and modify hotels, rooms, and guests.

## Exposed Tools
1. `list_hotels`: Retrieves the list of all hotels in the SQLite database.
2. `list_rooms`: Retrieves all rooms along with their parent hotel names.
3. `list_guests`: Retrieves the list of all registered guests.
4. `search_guests`: Search for a guest by name or email.
5. `add_hotel`: Creates a new hotel record in the SQLite database.

---

## Configuration

To register this MCP server with your agentic IDE, add the following configuration block:

### 1. Claude Desktop
Add this to your `claude_desktop_config.json` (located at `~/Library/Application Support/Claude/claude_desktop_config.json` on macOS):

```json
{
  "mcpServers": {
    "hotel-mgt-mcp": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/Users/jakovivancek/Desktop/Jakov_Ivancek_projekt/Hotel-Mgt.Console/Hotel-Mgt.Console.csproj",
        "--",
        "--mcp"
      ]
    }
  }
}
```

### 2. Cursor or Windsurf
Go to Settings -> Features -> MCP, click "+ Add New MCP Server", and use the following parameters:
- **Name**: `hotel-mgt-mcp`
- **Type**: `command`
- **Command**: `dotnet run --project /Users/jakovivancek/Desktop/Jakov_Ivancek_projekt/Hotel-Mgt.Console/Hotel-Mgt.Console.csproj -- --mcp`

---

## Verification

To test it manually from your shell, run:
```bash
dotnet run --project Hotel-Mgt.Console -- --mcp
```
Once running, paste this JSON line into standard input to check the tool listing:
```json
{"jsonrpc": "2.0", "id": 1, "method": "tools/list"}
```
You should get a JSON response listing all the available database tools.
