using HireFlow.Application.DTOs.Admin;
using HireFlow.Application.DTOs.Common;

namespace HireFlow.Application.Interfaces;

public interface IAdminService
{
	Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(int pageNumber, int pageSize);
	Task<PagedResult<CompanySummaryDto>> GetAllCompaniesAsync(int pageNumber, int pageSize);
	Task ApproveCompanyAsync(long companyId);
	Task SuspendCompanyAsync(long companyId);
}
