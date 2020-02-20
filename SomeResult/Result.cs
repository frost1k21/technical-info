
namespace SomeResult
{
    public class Result<TSuccess, TError>
    {
        public TSuccess Success { get; set; }
        public TError Error { get; set; }
    }
}
