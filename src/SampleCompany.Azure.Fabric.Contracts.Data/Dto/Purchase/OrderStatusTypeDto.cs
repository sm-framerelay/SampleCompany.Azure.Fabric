namespace SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase
{
    public enum OrderStatusTypeDto
    {
        Unknown = 0,
        New = 1,
        Submitted = 2,
        InProcess = 3,
        Backordered = 4,
        Shipped = 5,
        Canceled = 6
    }
}
