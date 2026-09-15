# Telegram YouTube Audio Extractor

A Telegram bot that extracts audio from YouTube videos when a user shares a YouTube link in a chat.

---

## Configuration

You can configure the application using either an `appsettings.json` file or environment variables.

### Option 1: `appsettings.json`

Add your bot credentials and endpoint configuration to `appsettings.json`:

```json
{
  "Api": {
    "BaseUrl": "[https://api.telegram.org](https://api.telegram.org)",
    "BotToken": "YOUR_BOT_TOKEN",
    "HookUrl": "https://your-domain.com/updates"
  }
}
```

### Option 2: Environment Variables (.env)

```code
Api__BaseUrl=https://api.telegram.org
Api__BotToken=YOUR_BOT_TOKEN
Api__HookUrl=https://your-domain.com/updates
```
The /updates endpoint is the one that is responsible for msg processing, you can leave it as is in configuration or change it with something like nginx to whatever you want and route to /updates on localhost

## Important! To register webhook call GET /ctrl/setWebhook?enabled=true and enabled=false to clear all registered webhooks

## Configuration Parameters

| Key | Description | Example / Default |
| :--- | :--- | :--- |
| `Api:BaseUrl` | Telegram Bot API base URL | `https://api.telegram.org` |
| `Api:BotToken` | Bot token obtained from [@BotFather](https://t.me/BotFather) | `123456789:ABCdefGhIJK...` |
| `Api:HookUrl` | Public URL where Telegram delivers webhook payloads | `https://your-domain.com/updates` |
