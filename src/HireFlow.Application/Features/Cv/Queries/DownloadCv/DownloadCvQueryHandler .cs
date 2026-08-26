using HireFlow.Application.Features.Cv.Dtos;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Queries.DownloadCv;

public sealed class DownloadCvQueryHandler
	: IRequestHandler<DownloadCvQuery, CvDownloadResult>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;
	private readonly ICvPdfService _pdfService;

	public DownloadCvQueryHandler(
		IAppDbContext db,
		ICurrentUser currentUser,
		ICvPdfService pdfService)
	{
		_db = db;
		_currentUser = currentUser;
		_pdfService = pdfService;
	}

	public async Task<CvDownloadResult> Handle(
		DownloadCvQuery query,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var cv = await _db.FreelancerCvs
			.AsNoTracking()
			.Include(c => c.User)
			.FirstOrDefaultAsync(
				c => c.Id == query.CvId,
				cancellationToken)
			?? throw new NotFoundException(
				"CV",
				query.CvId);

		if (cv.UserId != userId)
		{
			throw new ForbiddenException(
				"You can only download your own CVs.");
		}

		var fileName = $"{cv.User!.FullName.Replace(" ", "_")}_CV.pdf";

		if (!string.IsNullOrEmpty(cv.FileUrl))
		{
			return new CvDownloadResult
			{
				FileUrl = cv.FileUrl,
				FileName = fileName
			};
		}

		var cvDto = CvMapper.MapToDto(cv);

		var pdfBytes = _pdfService.Generate(
			cv: cvDto,
			fullName: cv.User.FullName,
			email: cv.User.Email,
			phone: cv.User.PhoneNumber,
			portfolioUrl: cv.User.PortfolioUrl);

		return new CvDownloadResult
		{
			PdfBytes = pdfBytes,
			FileName = fileName
		};
	}
}