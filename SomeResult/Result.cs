
namespace SomeResult
{
    public class Result<TSucces, TError>
    {
        public TSucces Success { get; set; }
        public TError Error { get; set; }
    }
}
