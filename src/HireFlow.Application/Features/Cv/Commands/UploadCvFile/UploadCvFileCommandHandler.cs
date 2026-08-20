using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Features.Cv.Commands.CreateCv;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Commands.UploadCvFile;

public class UploadCvFileCommandHandler : IRequestHandler<UploadCvFileCommand, CvDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UploadCvFileCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CvDto> Handle(
		UploadCvFileCommand command, CancellationToken ct)
	{
		var userId = _currentUser.UserId;

		var hasAnyCv = await _db.FreelancerCvs
			.AnyAsync(c => c.UserId == userId, ct);

		var cv = new FreelancerCv
		{
			UserId = userId,
			Title = command.Title,
			FileUrl = command.FileUrl,  
			IsDefault = !hasAnyCv,     
			CreatedAt = DateTime.UtcNow
		};

		_db.FreelancerCvs.Add(cv);
		await _db.SaveChangesAsync(ct);

		return CreateCvCommandHandler.MapToDto(cv);
	}
}