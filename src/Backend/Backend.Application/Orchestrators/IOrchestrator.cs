namespace Backend.Application.Orchestrators;

public interface IOrchestrator<IRequest, IResult> where IRequest : class where IResult : class
{
    Task<IResult> ExecuteAsync(IRequest request);
}
