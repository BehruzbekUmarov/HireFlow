using HireFlow.Domain.Enums;

namespace HireFlow.Application.DTOs.JobApplication.Requests;

public class ChangeStatusRequest
{
	public ApplicationStatus NewStatus { get; set; }
}
