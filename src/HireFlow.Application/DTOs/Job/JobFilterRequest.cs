namespace HireFlow.Application.DTOs.Job;

public class JobFilterRequest
{
	public string? Keyword { get; set; }
	public string? Category { get; set; }
	public string? Location { get; set; }
	public decimal? MinSalary { get; set; }
	public decimal? MaxSalary { get; set; }
	public string SortBy { get; set; } = "CreatedAt";
	public string SortOrder { get; set; } = "desc";

	public int PageNumber { get; set; } = 1;
	private int _pageSize = 10;
	public int PageSize
	{
		get => _pageSize;
		set => _pageSize = value > 50 ? 50 : value; 
	}
}
