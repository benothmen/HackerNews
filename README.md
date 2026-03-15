# Hacker News Best Stories API (ASP.NET Core)

REST API that returns the best **n** stories from the Hacker News API, ordered by score descending.

**Author:** Zied Ben Othman

## Requirements

- .NET SDK 10

## Run locally

From the repository root:

```bash
dotnet run --project src/Api/Api.csproj
```

The API will be available (see `src/Api/Properties/launchSettings.json` for exact ports).

Example:

```bash
curl "http://localhost:5045/api/stories/best?n=10"
```

## API

### `GET /api/stories/best?n={n}`

- `n` (required): integer from **1** to **500**

Response: `200 OK`

```json
[
  {
    "title": "...",
    "uri": "...",
    "postedBy": "...",
    "time": "2019-10-12T13:43:01+00:00",
    "score": 1716,
    "commentCount": 572
  }
]
```

If the parameter is missing/invalid, the API returns `400 Bad Request` with validation details.

## OpenAPI / Scalar

- OpenAPI is enabled in Development.
- Scalar UI is configured via the launch profile `API` (`launchUrl`: `scalar`).

## Architecture

This solution follows a Clean Architecture-style layering:

- `src/Domain`: core domain model (no dependencies on other projects)
- `src/Application`: use-cases/services + interfaces/DTOs (depends on `Domain`)
- `src/Infrastructure`: external concerns (Hacker News HTTP client, DI wiring) (depends on `Application`)
- `src/Api`: ASP.NET Core host (controllers, OpenAPI/Scalar, composition root) (depends on `Application` + references `Infrastructure` for registration)

Architecture rules are enforced by `tests/Architecture.Tests`.

## Caching & performance notes

To avoid overloading the Hacker News API under load:

- Best story IDs are cached for a short time (`HackerNews:BestStoryIds`).
- Individual story details are cached per ID (`HackerNews:Story:{id}`).
- Cache entries are created using `IMemoryCache.GetOrCreateAsync` to reduce cache stampedes.
- Story fetch is bounded by a concurrency limit (`MaxConcurrency`).

## Tests

Run all tests:

```bash
dotnet test
```

Test projects:

- `tests/Application.Tests`: unit tests for `Application` services
- `tests/Api.Tests`: functional/integration tests for API endpoints
- `tests/Architecture.Tests`: architecture rules (layer dependency tests)

## Docker

Build:

```bash
docker build -t hackernews-api .
```

Run:

```bash
docker run --rm -p 8080:8080 hackernews-api
```

Then:

```bash
curl "http://localhost:8080/api/stories/best?n=10"
```

## Assumptions

- `n` is constrained to `1..500`.
- The service currently computes the top `n` by fetching all best story items (bounded by concurrency). This guarantees correctness but can be expensive on cold cache.

## Possible enhancements

- Add retry/backoff and timeouts for Hacker News HTTP calls.
- Add observability (structured logs + metrics).
