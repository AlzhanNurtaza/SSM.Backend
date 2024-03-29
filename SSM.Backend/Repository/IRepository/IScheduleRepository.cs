﻿using SSM.Backend.Models;


namespace SSM.Backend.Repository.IRepository
{
    public interface IScheduleRepository: IRepository.IRepository<Schedule>
    {
        Task<List<Schedule>> GetAllScheduleAsync(DateTime? StartDate, DateTime? EndDate, List<ScheduleFilterParam>? where);
        Task<bool> isEnrollmentNewInSchedule(string id);
    }
}
