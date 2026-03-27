# Copilot Instructions

## App Lifecycle

When you **start working** on a task, stop the running YnabReports app to avoid locked-file build errors:

```powershell
Get-Process -Name YnabReports -ErrorAction SilentlyContinue | Stop-Process -Force
```

When you **finish working** on a task (before marking complete), start the app again in the background:

```powershell
dotnet run --project src/YnabReports -- --urls http://localhost:5198
```

Use `detach: true` so the process survives after the session ends.
