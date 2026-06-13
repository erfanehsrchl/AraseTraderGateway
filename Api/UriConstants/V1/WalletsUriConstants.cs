namespace Api.UriConstants.V1;

public static class WalletsUriConstants
{
    public const string Route = "api/v{version:apiVersion}/wallets";
    public const string GetByCustomerId = "by-customer/{customerId:long}";
    public const string GetTransactionsByWalletId = "{walletId:long}/transactions";
}
