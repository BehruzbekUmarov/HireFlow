using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job;

namespace HireFlow.Application.Interfaces;

public interface IJobService
{
	Task<JobDetailDto> CreateAsync(long companyId, CreateJobRequest request);
	Task<JobDetailDto> UpdateAsync(long jobId, long companyId, UpdateJobRequest request);
	Task CloseAsync(long jobId, long companyId);
	Task<JobDetailDto?> GetByIdAsync(long jobId);
	Task<PagedResult<JobSummaryDto>> SearchAsync(JobFilterRequest filter);

	// For company dashboard — only their own jobs
	Task<PagedResult<JobSummaryDto>> GetByCompanyAsync(long companyId, int pageNumber, int pageSize);
}
