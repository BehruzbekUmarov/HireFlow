using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Services.Interfaces;
using QuestPDF.Fluent;

namespace HireFlow.Infrastructure.Implementations.Documents;

public class CvPdfService : ICvPdfService
{
	public byte[] Generate(CvDto cv, string fullName, string email,
						   string? phone, string? portfolioUrl)
	{
		var document = new CvDocument(cv, fullName, email, phone, portfolioUrl);
		return document.GeneratePdf();
	}
}
