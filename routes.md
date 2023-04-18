# API Routes

## Base URL : `http://{hostname}:{port}/api/v1`

Quick and dirty API docs.

## Authentication

If the API key middleware is enabled, you will have to pass the API key in the header.

`X-Api-Key: {API_KEY}`

### /doujins

#### Schema

```csharp
enum Source
{
    ExHentai = 0,
    EHentai = 1
}
``
```


```json
{
  "doujinId": "string",
  "title": "string",
  "rating": "string",
  "url": "string",
  "posted": 0,
  "category": "string",
  "fileCount": 0,
  "fileName": "string",
  "tags": [
    "string"
  ],
  "thumbnail": "string",
  "telegraphUrl": "string",
  "imageUrls": [
    "string"
  ],
  "source": 0 
}
```

#### Endpoints

`GET /doujins/` - Get all doujins.

`GET /doujins/{id}` - Get doujin by `_id` (Document id)

`GET /doujins/doujinId/{id}` - Get doujin by doujin id.

`POST /doujins/` - Create a new doujin

`POST /doujins/fetch` - Fetch doujin by URL in body.

```json
"https://exhentai.org/g/1234567/84fe951716"
```

`POST /doujins/random` - Get random doujin. (Refer to bot README for tag info). Tags in body (optional).

```json
"#tag1 #tag_2 (#tag3)"
```
`GET /doujins/zip/{id}` - Get doujin zip by `_id` (Document id). (Returns a zip file)


`GET /doujins/count` - Get the doujins count.

`GET /doujins/views/{url}` - Get the views count for a doujin by Telegraph URL.

`PUT /doujins/{id}` - Update a doujin by `_id` (Document id). **Replace the entire document.**

`DELETE /doujins/{id}` - Delete doujin by `_id` (Document id).

### /users

#### Schema

```json
{
  "id": "string",
  "userId": 0,
  "username": "string",
  "name": {
    "first": "string",
    "last": "string"
  },
  "favorites": [
    0
  ],
  "doujins": [
    0
  ],
  "usage": 0,
  "dailyUse": 0,
  "dailyUseDate": "2023-04-18T03:48:39.018Z"
}
```

#### Endpoints

`GET /users/` - Get all users.

`GET /users/count` - Get user count.

`GET /users/{id}` - Get user by `_id` (Document id)

`GET /users/userId/{id}` - Get user by telegram id.

`POST /users/` - Create new user. Put the new values in the body. Below are the minimum required fields.

`PUT /users/:id` - Update user by `_id`, replace the entire document.

`DELETE /users/:id` - Delete user by `_id` or telegram id.

### /settings

#### Schema

```json

{
  "id": "string",
  "name": "string",
  "ownerId": 0,
  "whitelistUsers": [
    0
  ],
  "whitelistGroups": [
    0
  ],
  "loadingMessages": [
    "string"
  ],
  "loadingGifs": [
    "string"
  ],
  "maxDailyUse": 0,
  "maxFiles": 0,
  "allowedCommandsGroups": {
    "additionalProp1": [
      "string"
    ],
    "additionalProp2": [
      "string"
    ],
    "additionalProp3": [
      "string"
    ]
  },
  "cookies": {
    "[source]": "string"
  }
}


```

`GET /settings/` - Get settings , they are created automatically if they don't exist. There can only be one settings document.

`DELETE /settings/{id}` - Delete settings by `_id`. 

`PUT /settings` - Update settings. Replace the entire document.

### /logs

#### Schema

```csharp
enum LogLevel {
  NotSet = 0,
  Debug = 1,
  Info = 2,
  Warning = 3,
  Error = 4,
  Fatal = 5,
  None = 6  
}
```


```json
{
  "level": 0,
  "message": "string",
  "timestamp": 0
}
```

#### Endpoints

`GET /logs` - Get all logs.

`GET /logs/:id` - Get log by `_id`.

`GET /logs/count` - Get log count.

`POST /logs` - Create new log. Put the new values in the body.

`DELETE /logs/:id` - Delete log by `_id`.

`DELETE /logs` - Delete all logs.


### /stats

#### Schema

```json
{
  "id": "string",
  "name": "string",
  "tags": {
    "positive": {
      "[tag]": 0
    },
    "negative": {
      "[tag]": 0 
    }
  },
  "usePerSource": {
    "[source]": 0 
  },
  "totalUse": 0,
  "randomUse": 0,
  "fetchUse": 0
}
```

#### Endpoints

`GET /stats` - Get stats. They are created automatically if they don't exist. There can only be one stats document.

`DELETE /stats/:id` - Delete stats by `_id`.

`PUT /stats` - Update stats. Replace the entire document.


