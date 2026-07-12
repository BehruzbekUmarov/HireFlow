using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Domain.Enums;

namespace HireFlow.Application.Interfaces;

public interface IJobApplicationService
{
	// Freelancer submits an application
	Task<JobApplicationDto> SubmitAsync(long jobId, long userId, SubmitApplicationRequest request);

	// Company changes the status of an application
	Task<JobApplicationDto> ChangeStatusAsync(long applicationId, long companyId, ApplicationStatus newStatus);

	// Company views all applications for one of their jobs
	Task<PagedResult<JobApplicationDto>> GetByJobAsync(long jobId, long companyId, int pageNumber, int pageSize);

	// Freelancer views their own applications
	Task<PagedResult<JobApplicationDto>> GetByUserAsync(long userId, int pageNumber, int pageSize);

	// Get single application detail
	Task<JobApplicationDto?> GetByIdAsync(long applicationId);
}
