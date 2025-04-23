using Polly;

namespace OrdersService.BusinessLogicLayer.Policies
{
    public interface IUsersMicroservicePolicies
    {       
        IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
    }
}
