using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Features.Cv.Dtos;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Queries.GetCvById;

public class GetCvByIdQueryHandler : IRequestHandler<GetCvByIdQuery, CvDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetCvByIdQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CvDto> Handle(
		GetCvByIdQuery query, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var cv = await _db.FreelancerCvs
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == query.CvId, cancellationToken)
			?? throw new NotFoundException("CV", query.CvId);

		if (cv.UserId != userId)
			throw new ForbiddenException("You can only view your own CVs.");

		return CvMapper.MapToDto(cv);
	}
}
