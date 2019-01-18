using System.Threading.Tasks;

namespace BackgroundServices
{
    public class TestService
    {
        public void DoTask()
        {
            Task.FromResult(Task.Delay(2000));
        }
    }
}
