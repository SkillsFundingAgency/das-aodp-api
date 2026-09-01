# Rollover performance testing

## Why I looked at this

The funding extension submission can contain around 13,000 records and we have a 30 second request timeout to work within. The two main problems I was seeing were the size of the request going through Azure and the amount of time spent loading and saving the data.

The qualification funding load was taking around 17 seconds in the test environment, and the original tracked EF save was also doing a lot of work in one go. I wanted to get the database work down without adding hand-written SQL, stored procedures or relying on temp table permissions.

## What has changed

For the request size issue, the rollover data is now sent as a JSON file inside multipart form data rather than thousands of separate form fields. This gets around the ASP.NET `MaxModelBindingCollectionSize` problem and also avoids the multipart section issue we were hitting through Azure.

For the processing itself:

- The candidate inputs and qualification fundings are put into dictionaries before applying updates. This avoids repeatedly scanning the full collection for every candidate.
- Qualification fundings are loaded using the qualification version IDs and then matched against the full `(QualificationVersionId, FundingOfferId)` key in memory. The query is no tracking because the persistence step does not use tracked EF updates.
- Candidate and funding changes are written to a permanent `FundingExtensionStaging` table using a bulk insert.
- EF Core `ExecuteUpdateAsync` applies the staged candidate and funding changes as set based updates.
- Discussion history is bulk inserted and workflow candidates are removed using `ExecuteDeleteAsync`.
- Everything is kept in one transaction. The staging rows are removed after a successful run and rollback plus a defensive cleanup handles failures.
- Timing logs have been added around the candidate load, funding load, lookup creation, history creation, persistence stages and the overall request.

## Approaches I compared

I looked at a few different ways of doing this:

- Normal tracked EF and `SaveChangesAsync`. This is the simplest, but it gets expensive when EF has to track and save tens of thousands of candidate, funding and history entities.
- Direct EF Core Bulk Extensions updates. This was quick locally, but the update path uses temp-table/MERGE behaviour on SQL Server and that caused permission problems in Azure.
- A JSON/set based SQL approach. This was the quickest benchmark, but I did not want to introduce hand-written SQL or a stored proc just for this process.
- A temp staging table. This also performed well but needs temp table permissions, which the application does not have and I did not want to add.
- A permanent staging table with bulk insert and EF Core set based updates. This is the approach used now. It keeps most of the speed of the bulk option without hand-written update SQL or temp table permissions.
- An async upload, blob scan and status poller. This is still the safer long-term option if the full request cannot reliably stay under 30 seconds, especially as Defender for Storage means the blob has to be scanned before processing. It is a bigger change, so I have not added it as part of this work.

## Final benchmark results

These were run with BenchmarkDotNet on .NET 10 using an in-memory SQLite relational database. The 13,000 record figures are the useful ones for the current problem.

| Area | Original approach | Changed/tested approach | Original time | New time | Improvement | Original allocation | New allocation |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |
| Discussion history creation | `FirstOrDefault` scan for each candidate | Funding dictionary lookup | 254.49 ms | 9.80 ms | 26x faster / 96% less time | 5.30 MB | 5.00 MB |
| Candidate graph loading | Full graph with `Include` | Lightweight tracked projection | 794.78 ms | 189.56 ms | 4.2x faster / 76% less time | 120.17 MB | 24.98 MB |
| Qualification funding load | Concatenated composite key | Indexed composite-key join | 1,662.99 ms | 63.15 ms | 26x faster / 96% less time | 19.35 MB | 9.92 MB |
| Persistence | Tracked EF and `SaveChangesAsync` | Permanent staging plus EF set updates | 6,686.13 ms | 246.51 ms | 27x faster / 96% less time | 370.48 MB | 41.99 MB |

The permanent staging approach was also slightly quicker than the direct Bulk Extensions update in this run: 246.51 ms compared with 253.27 ms. It allocated around 42 MB compared with around 57 MB.

The quickest persistence result was 134.07 ms using hand-written set based SQL, but that option was deliberately not taken. Saving another roughly 112 ms is not worth bringing manual SQL and its maintenance overhead into this flow.

## What the combined numbers mean

Using the approaches that are actually in the code now, the local benchmark estimate comes down from roughly 9.40 seconds to around 3.90 seconds across candidate loading, funding loading, history creation and persistence. That is around 5.5 seconds saved, or about 59% overall.

The biggest confirmed gain is the save itself. The benchmark reduced that from 6.69 seconds to 247 ms and cut managed allocations by around 89%. The dictionary history lookup is also worth keeping, although on its own it only saves around a quarter of a second at 13,000 records.

The candidate projection and indexed composite-key join show what is possible, but they are not both implemented in the final flow. The candidate load still needs tracked entities because they are used to apply the domain updates. The funding query currently uses the smaller, low-risk version-ID query plus an in-memory key lookup.

There is an important caveat with the funding query. In the local benchmark, the current no-tracking version-ID approach took 2.85 seconds at 13,000 candidates because it loaded all four funding offers for each qualification version. The old concatenated query took 1.66 seconds locally. In Azure the old query was observed at around 17 seconds, most likely because concatenating columns prevents the useful index from being used in the same way. This means the local benchmark is good for comparing allocations and code shape, but it cannot promise the Azure SQL timing. The new Application Insights logs are the actual source of truth after deployment.

## What I expect in Azure

The funding load is still the main thing to watch. If that stays anywhere near the 17 seconds already seen, there should still be enough headroom from the persistence saving to improve the chance of staying below 30 seconds, but it is not guaranteed. I will use the stage timing logs in Application Insights to check the real candidate load, funding load, staging insert, candidate update, funding update, history insert and transaction total.

If the whole request is still getting close to 30 seconds after deployment, I would stop trying to squeeze more work into the HTTP request and move the submit flow to the async blob upload and status polling approach.
