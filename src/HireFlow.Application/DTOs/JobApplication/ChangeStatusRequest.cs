using HireFlow.Domain.Enums;

namespace HireFlow.Application.DTOs.JobApplication;

public class ChangeStatusRequest
{
	public ApplicationStatus NewStatus { get; set; }
}
