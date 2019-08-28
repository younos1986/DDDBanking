namespace Banking.Domain.IntegrationEvents
{
    public enum EventStateEnum
    {
        None = 0,
        ReadyToPublish = 1,
        InProgress = 2,
        Published = 3,
        PublishedFailed = 4
    }
}