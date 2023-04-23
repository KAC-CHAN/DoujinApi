# Doujin Api

This is an API that will fetch doujins from exhentai/e-hentai , store them in a database and post them as articles on telegraph.
The reason behind posting to telegra.ph is because of the instant view feature it offers for Telegram, this is the main use of this API for now.

**Note : If you plan on using my Telegram Bot, this is the API you need in order to use it.**

**Note 2 : Posting to telegraph is kind of problematic at times. If it fails once don't even try again, some doujins just don't want to be on there apparently lol, i need to find an alternative. For now I will work on decoupling the /fetch and /random commands from telegraph**

## Features

- [x] Scrape doujins from exhentai/e-hentai by URL
- [x] Scrape random doujins from exhentai/e-hentai
- [x] Manage settings via API calls
- [x] Manage users via API calls
- [x] Manage doujins via API calls
- [x] Access logs via API calls
- [x] Keep track of stats.

## Possible new features

- [ ] Alternative doujin sources
- [ ] More in-depth stats
- [ ] Other posting sources (other than telegra.ph) <--- Telegra.ph causes more problems than it solves tbh.

## Dependencies

- [dotnet](https://dotnet.microsoft.com/download)
- [mongodb](https://www.mongodb.com/)
- [visual studio](https://visualstudio.microsoft.com/) (optional)

## Environment variables

The 'environement variables' are stored inside the `appsettings.json` file. You can change them there.

- `"DoujinApiDatabase:ConnectionString"` - MongoDB URI -> You will have to create a MongoDB Instance.
- `"Authentication:ApiKey"` - The API key you want. If you don't want to use an API key, just remove the ApiKey middleware.

```csharp
   app.UseMiddleware<ApiKeyAuthMiddleware>();
```

- `"Telegraph:AccessToken"` - Telegraph access token. You can get one [here](https://telegra.ph/api#createAccount)
- `"Telegraph:AuthorName"` - Telegraph author name.
- `"Telegraph:AuthorUrl"` - Telegraph author url.

## Routes

See [routes.md](routes.md)

## How to use

### Settings

The settings are stored in the database. You can change them via the API, to use exhentai you will have to set cookies in the settings. Using the following format:
Normaly these settings are set by the Telegram bot I made for this API, but you can set them manually if you want.

`igneous=YOUR_IGNEOUS_COOKIE; ipb_member_id=YOUR_IPB_MEMBER_ID; ipb_pass_hash=YOUR_IPB_PASS_HASH; sk=YOUR_SK_COOKIE;`

```json
{
  "name": "settings",
  "ownerId": 0,
  "whitelistUsers": [],
  "whitelistGroups": [],
  "loadingMessages": [
    "Loading..."
  ],
  "loadingGifs": [
    "https://i.pinimg.com/originals/64/0f/da/640fda7bcdf69371d0d3ee65e17974f0.gif"
  ],
  "maxDailyUse": 20,
  "maxFiles": 65,
  "allowedCommandsGroups": {},
  "cookies": {
    "Exhentai": "YOUR_COOKIES_HERE"
  }
}

```

### Local machine (Dev or testing)

- Clone the repository
- Open the solution in visual studio (or your IDE of choice)
- Build the solution
- Run the solution
- Fill in the environment variables in the `appsettings.json` file.
- You can now use the API

### Docker

- Clone the repository
- Change the environment variables in the `appsettings.json` file
- Build the docker image with `docker build -t {image_name} .`
- Run the docker image with `docker run -d -p {port}:80 {image_name}`
