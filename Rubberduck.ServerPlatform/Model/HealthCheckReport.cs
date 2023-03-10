namespace Rubberduck.ServerPlatform.Model
{
    public class HealthCheckReport
    {
        public static HealthCheckReport Success(string name, string message, params Item[] items)
            => new HealthCheckReport
            {
                HealthCheck = name,
                Timestamp = DateTime.UtcNow,
                IsSuccess = true,
                Message = message,
                Items = items
            };

        public static HealthCheckReport Failure(string name, string message, Exception exception, params Item[] items)
            => new HealthCheckReport
            {
                HealthCheck = name,
                Timestamp = DateTime.UtcNow,
                IsSuccess = false,
                Message = message,
                Exception = exception,
                Items = items
            };

        public DateTime Timestamp { get; set; }
        public string HealthCheck { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public IEnumerable<Item> Items { get; set; } = Enumerable.Empty<Item>();

        public class Item
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string ValueDescription { get; set; }
        }
    }
}
