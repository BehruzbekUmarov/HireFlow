using HireFlow.Application.DTOs.Cv.Responses;
using MediatR;

namespace HireFlow.Application.Features.Cv.Commands.UploadCvFile;

public record UploadCvFileCommand(string FileUrl, string Title) : IRequest<CvDto>;
