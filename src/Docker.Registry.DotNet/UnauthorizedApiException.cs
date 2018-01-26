namespace Docker.Registry.DotNet
{
    public class UnauthorizedApiException : RegistryApiException
    {
        internal UnauthorizedApiException(RegistryApiResponse response) : base(response)
        {
        }
    }
}