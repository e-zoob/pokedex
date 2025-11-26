# Pokedex API

Minimal ASP.NET 10 API to get Pokémon information and translated descriptions.

---

## Features

* `/pokemon/{name}` → Get Pokémon info (habitat, description, legendary flag).
* `/pokemon/translated/{name}` → Get Pokémon info with translated description (Yoda or Shakespeare style).
* Input validation (name rules).
* In-memory caching for repeated requests.
* Global exception handling returning `ProblemDetails`.
* Logging with Serilog.
* Swagger / OpenAPI documentation.

---

## Requirements
* git( for cloning the repository)
* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (only for local run or build).
* Docker & Docker Compose (optional for containerized run).

---

## Running Locally

1. **Clone the repository**:

```bash
git clone https://github.com/e-zoob/pokedex.git
cd pokedex-api
```

2. **Restore dependencies**:

```bash
dotnet restore
```

3. **Run the API**:

```bash
dotnet run
```

4. **Swagger UI** (interactive API docs):

Open your browser at:

```
https://localhost:PORT/swagger
```

5. **Testing endpoints via HTTP clients**:

### `curl`

```bash
# Info
curl http://localhost:PORT/pokemon/pikachu

# Info with translated description
curl http://localhost:PORT/pokemon/translated/pikachu
```

### `httpie`

```bash
# Info
http http://localhost:PORT/pokemon/pikachu

# Response
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Wed, 26 Nov 2025 02:57:53 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
    "description": "When several of these POKéMON gather, their electricity could build and cause lightning storms.",
    "habitat": "forest",
    "isLegendary": false,
    "name": "pikachu"
}

# Info with translated description
http http://localhost:PORT/pokemon/translated/pikachu

# Response
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Wed, 26 Nov 2025 03:01:24 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
    "description": "At which hour several of these pokémon gather,  their electricity couldst buildeth and cause lightning storms.",
    "habitat": "forest",
    "isLegendary": false,
    "name": "pikachu"
}
```

---

## Running with Docker Compose

1. **Build and run**:

```bash
docker-compose up --build
```

2. **Access the API**:

```
http://localhost:5283/pokemon/pikachu
http://localhost:5283/pokemon/translated/pikachu
```

---

## Notes

* If you call a Pokémon that doesn’t exist → returns `404 NotFound` with JSON `ProblemDetails`.
* If the name is invalid → returns `400 BadRequest` with validation info.
* Translations are returned if available; otherwise, the original description is returned.
* Endpoints are cached in-memory for 5 minutes by default.

---

## Testing

Run tests:

```bash
dotnet test
```

---

## Logging

* Serilog logs to console.
* Unhandled exceptions are returned as JSON `ProblemDetails`.

---

## Examples

**Normal Info**:

```json
{
  "name": "pikachu",
  "description": "When several of these Pokémon gather, their electricity could build and cause lightning storms.",
  "habitat": "forest",
  "isLegendary": false
}
```

**Translated Info**:

```json
{
  "name": "pikachu",
  "description": "When several of these Pokémon gather, their electricity could build and cause lightning storms, hmmm.",
  "habitat": "forest",
  "isLegendary": false
}
```
---
