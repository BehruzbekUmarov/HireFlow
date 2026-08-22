using HireFlow.Application.DTOs.Cv.Responses;

namespace HireFlow.Application.Services.Interfaces;

public interface ICvPdfService
{
	byte[] Generate(CvDto cv, string fullName, string email,
					string? phone, string? portfolioUrl);
}