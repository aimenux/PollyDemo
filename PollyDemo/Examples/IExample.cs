using System.Threading.Tasks;

namespace PollyDemo.Examples
{
    public interface IExample
    {
        public string Name { get; }
        public string Description { get; }
        Task RunAsync();
    }
}
