using MediatR;

namespace HireFlow.Application.Features.Cv.Queries.DownloadCv;

public record DownloadCvQuery(long CvId) : IRequest<CvDownloadResult>;

public class CvDownloadResult
{
	public byte[]? PdfBytes { get; set; }    
	public string? FileUrl { get; set; }     
	public string FileName { get; set; } = string.Empty;
}
