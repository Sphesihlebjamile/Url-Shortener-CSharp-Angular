namespace Backend.Application.Plans;

public interface IPlan<IRequest, IResponse>
    where IRequest : class where IResponse : class
{
    Task<IResponse> ExecuteAsync(IRequest request);
}
