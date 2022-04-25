namespace Demo.Api.Controllers
{
    public class FakeDB
    {
        private readonly static List<BusinessTask> _tasks = new List<BusinessTask>();

        public static readonly FakeDB Instance = new FakeDB();

        public FakeDB()
        {
            _tasks.Add(new BusinessTask(Guid.NewGuid(), "Start an API"));
        }

        public Task<IReadOnlyList<BusinessTask>> GetAllAsync()
        {
            return Task.FromResult<IReadOnlyList<BusinessTask>>(_tasks);
        }

        public Task<BusinessTask> AddTask(BusinessTask task)
        {
            _tasks.Add(task);
            return Task.FromResult(task);
        }

    }
}
