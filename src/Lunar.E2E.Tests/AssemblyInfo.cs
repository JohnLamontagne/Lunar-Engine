using Xunit;

// Each end-to-end test class starts real server and client processes. Running classes in parallel
// doubles the process count on small CI boxes and makes timing-based waits flaky, so the suite is
// serialized. Tests inside a class already run sequentially.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
