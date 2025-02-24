using System.Threading.Tasks;

public class MockRepository
{
    public async Task<string> GetDataAsync()
    {
        await Task.Delay(1); // Simulate 1ms delay
        return "MockData";
    }
}
