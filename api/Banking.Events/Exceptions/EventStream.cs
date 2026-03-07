public class EventStreamConflictException(Guid streamId, int version)
    : Exception(
        $"Stream {streamId} already has an event at version {version}. Optimistic concurrency conflict."
    );
