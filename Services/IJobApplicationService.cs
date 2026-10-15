using ReviewDeck.Models;

namespace ReviewDeck.Services;

public interface IJobApplicationService
{
    //Fetch all Job Application
    Task<List<JobApplication>> GetJobsAsync();
    Task<JobApplication?> GetJobByIdAsync(int id);
    Task<JobApplication> AddJobsAsync(JobApplication job);
    Task<JobApplication?> UpdateJobAsync(JobApplication job);
    Task<bool> DeleteJobAsync(int id);
}