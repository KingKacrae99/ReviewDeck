using ReviewDeck.Models;
using ReviewDeck.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;


namespace ReviewDeck.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly ReviewDeckContext _context;
    private readonly CurrentUserService _currentUserService;
    public JobApplicationService(ReviewDeckContext context, CurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<JobApplication>> GetJobsAsync()
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId <= 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.", nameof(userId));
        }

        return await _context.JobApplications
            .Where(job => job.UserId == userId)
            .OrderByDescending(job => job.AppliedAt)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetJobByIdAsync(int id)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (id <= 0 || userId <= 0)
            return null;

        return await _context.JobApplications
            .FirstOrDefaultAsync(job =>
                job.Id == id && job.UserId == userId);
    }
    
    public async Task<JobApplication> AddJobsAsync(JobApplication job)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (job == null)
            throw new ArgumentNullException(nameof(job));

        if (userId <= 0)
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(job.JobTitle) ||
            string.IsNullOrWhiteSpace(job.Company) ||
            string.IsNullOrWhiteSpace(job.JobUrl))
        {
            throw new ArgumentException(
                "Job title, company and job URL are required.");
        }

        job.JobTitle = job.JobTitle.Trim();
        job.Company = job.Company.Trim();
        job.JobUrl = job.JobUrl.Trim();

        var jobExists = await _context.JobApplications.AnyAsync(j =>
            j.UserId == userId &&
            j.JobTitle.ToLower() == job.JobTitle.ToLower() &&
            j.Company.ToLower() == job.Company.ToLower() &&
            j.JobUrl.ToLower() == job.JobUrl.ToLower());

        if (jobExists)
            throw new InvalidOperationException(
                "You have already added this job application.");

        // Assign ownership using the trusted user ID.
        job.UserId = userId.Value;

        job.SetForCreation();

        job.AppliedAt = DateTime.SpecifyKind(
            job.AppliedAt.Date,
            DateTimeKind.Utc
        );

        await _context.JobApplications.AddAsync(job);
        await _context.SaveChangesAsync();

        return job;
    }


    
    public async Task<JobApplication?> UpdateJobAsync(JobApplication job)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (job == null)
            throw new ArgumentNullException(nameof(job));

        if (job.Id <= 0 || userId <= 0)
            throw new ArgumentException(
                "A valid job ID and user ID are required.");

        var existingJob = await _context.JobApplications
            .FirstOrDefaultAsync(j =>
                j.Id == job.Id && j.UserId == userId);

        if (existingJob == null)
            return null;

        if (string.IsNullOrWhiteSpace(job.JobTitle) ||
            string.IsNullOrWhiteSpace(job.Company) ||
            string.IsNullOrWhiteSpace(job.JobUrl))
        {
            throw new ArgumentException(
                "Job title, company and job URL are required.");
        }

        existingJob.JobTitle = job.JobTitle.Trim();
        existingJob.Company = job.Company.Trim();
        existingJob.JobUrl = job.JobUrl.Trim();
        existingJob.Location = job.Location?.Trim();
        existingJob.Description = job.Description?.Trim();
        existingJob.AppliedAt = job.AppliedAt;
        existingJob.JobMode = job.JobMode;
        existingJob.JobType = job.JobType;
        existingJob.Platform = job.Platform;
        existingJob.Status = job.Status;
        existingJob.SetForUpdate();

        existingJob.AppliedAt = DateTime.SpecifyKind(
            existingJob.AppliedAt.Date,
            DateTimeKind.Utc
        );

        await _context.SaveChangesAsync();

        return existingJob;
    }


    public async Task<bool> DeleteJobAsync(int id)
    {
        var currentUserId = await _currentUserService.GetUserIdAsync();
        if (id <= 0 || currentUserId <= 0)
        {
            throw new ArgumentException(
                "A valid job ID and user ID are required.");
        }

        var job = await _context.JobApplications
            .FirstOrDefaultAsync(j =>
                j.Id == id && j.UserId == currentUserId);

        if (job == null)
            return false;

        _context.JobApplications.Remove(job);
        await _context.SaveChangesAsync();

        return true;
    }
}

