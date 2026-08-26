using HireFlow.Domain.Exceptions;

namespace HireFlow.Api.Common.File;

public static class FileValidator
{
	private static readonly string[] AllowedCvExtensions = [".pdf", ".doc", ".docx"];
	private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

	private const long MaxCvSize = 7 * 1024 * 1024;        
	private const long MaxImageSize = 2 * 1024 * 1024;    

	public static void ValidateCv(IFormFile file)
	{
		if (file.Length == 0)
			throw new InvalidOperationDomainException("File is empty.");

		if (file.Length > MaxCvSize)
			throw new InvalidOperationDomainException(
				"CV file must not exceed 7MB.");

		var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
		if (!AllowedCvExtensions.Contains(extension))
			throw new InvalidOperationDomainException(
				"CV must be a PDF, DOC, or DOCX file.");
	}

	public static void ValidateImage(IFormFile file)
	{
		if (file.Length == 0)
			throw new InvalidOperationDomainException("File is empty.");

		if (file.Length > MaxImageSize)
			throw new InvalidOperationDomainException(
				"Image must not exceed 2MB.");

		var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
		if (!AllowedImageExtensions.Contains(extension))
			throw new InvalidOperationDomainException(
				"Image must be JPG, PNG, or WebP.");
	}
}