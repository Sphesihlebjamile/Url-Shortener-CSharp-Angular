namespace Backend.Application.Capabilities;

public interface ICapability<TRequest, TResponse>
{
    public TResponse Execute(TRequest request);
}
