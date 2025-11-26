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
* git (for cloning the repository)
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
   * Run this command in the project root folder containing `docker-compose.yml`:
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
# Pokedex API - Production Readiness

This document outlines key considerations for making the Pokedex API ready for production.

---

## 1. HTTPS / TLS
Right now, the API works over HTTP. In production, it should be served over HTTPS to encrypt communication. TLS can be handled directly by Kestrel or terminated at a reverse proxy (e.g., Nginx, Traefik) to simplify certificate management.

---

## 2. Resilience & Fault Handling
External APIs may fail or be slow. Use resilience patterns like retries, timeouts, and circuit breakers (e.g. Polly) to keep your API responsive and prevent cascading failures.

---

## 3. Caching
This API uses in-memory caching, which is suitable for simple scenarios with a single instance. For deployments with multiple instances, use a distributed cache (e.g., Redis) to reduce redundant external calls and maintain consistency across instances.

---

## 4. Load Balancing & Scaling
To scale the service, handle traffic spikes, and guarantee availability, deploy multiple instances behind a load balancer. Using Kubernetes or cloud auto-scaling is recommended for production readiness if demand requires it.

---

## 5. Health Checks
Implement liveness and readiness endpoints so that monitoring systems or administrators can verify the service is running and able to handle requests. These endpoints help detect issues early and enable automated alerts or restarts if needed. Additionally, integrate logging, metrics, and tracing to monitor the health and performance of the service in real time, improving observability and simplifying troubleshooting in production environments.

---

## 6. Security
For production readiness, implement authentication and authorization to protect sensitive endpoints and ensure that only authorized clients or users can access the API. This prevents unauthorized data access and mitigates potential abuse.

---

## 7. Integration and Load Testing

**Integration Testing:**  
To verify end-to-end behavior, consider using integration tests that run against realistic environments. Use **testing containers** (e.g., `Testcontainers for .NET`) to spin up ephemeral services or databases, allowing the API to interact with mocked or real dependencies without affecting production data.

**Load Testing:**  
To ensure the API can handle traffic spikes and meet performance requirements, perform load testing using tools such as **k6**, **Apache JMeter**, or **NBomber**. This helps identify bottlenecks, measure latency, and validate caching, scaling, and resilience strategies under stress.


