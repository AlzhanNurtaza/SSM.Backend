using SSM.Backend.Models;


namespace SSM.Backend.Repository.IRepository
{
    public interface IClassroomRepository: IRepository.IRepository<Classroom>
    {
        Task<List<Classroom>> GetAllClassroomsAsync(int _start = 0, int _end = 1, string? filterFrontEnd="", string? filterMain = "");
    }
}
