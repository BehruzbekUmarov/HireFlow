using HireFlow.Application.DTOs.Cv.Responses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HireFlow.Infrastructure.Implementations.Documents;

public class CvDocument : IDocument
{
	private readonly CvDto _cv;
	private readonly string _fullName;
	private readonly string _email;
	private readonly string? _phone;
	private readonly string? _portfolioUrl;

	private const string PrimaryColor = "#2C3E6B";
	private const string AccentColor = "#6B9FD4";
	private const string SidebarBg = "#2C3E6B";
	private const string LightBg = "#F0F4FA";

	public CvDocument(CvDto cv, string fullName, string email,
					  string? phone, string? portfolioUrl)
	{
		_cv = cv;
		_fullName = fullName;
		_email = email;
		_phone = phone;
		_portfolioUrl = portfolioUrl;
	}

	public DocumentMetadata GetMetadata() => new()
	{
		Title = $"{_fullName} — CV",
		Author = _fullName,
		Creator = "HireFlow"
	};

	public void Compose(IDocumentContainer container)
	{
		container.Page(page =>
		{
			page.Size(PageSizes.A4);
			page.Margin(0);
			page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

			page.Content().Row(row =>
			{
				row.ConstantItem(175)
				   .Background(SidebarBg)
				   .Padding(20)
				   .Column(ComposeSidebar);

				row.RelativeItem()
				   .Padding(24)
				   .Column(ComposeMain);
			});
		});
	}

	private void ComposeSidebar(ColumnDescriptor col)
	{
		col.Item().AlignCenter()
		   .Width(70).Height(70)
		   .Background("#4A6FA5")
		   .AlignCenter().AlignMiddle()
		   .Text(GetInitials(_fullName))
		   .FontSize(24).Bold().FontColor(Colors.White);

		col.Item().Height(12);

		col.Item().AlignCenter()
		   .Text(_fullName)
		   .FontSize(12).Bold().FontColor(Colors.White);

		col.Item().Height(4);

		col.Item().AlignCenter()
		   .Text(_cv.Title)
		   .FontSize(9).FontColor("#A8BEDD");

		col.Item().Height(20);

		SidebarSection(col, "CONTACT");
		SidebarItem(col, _email);
		SidebarItem(col, "Tashkent, Uzbekistan");
		if (_phone is not null) SidebarItem(col, _phone);
		if (_portfolioUrl is not null) SidebarItem(col, _portfolioUrl);
		col.Item().Height(12);

		if (_cv.Skills is not null)
		{
			SidebarSection(col, "SKILLS");
			foreach (var skill in _cv.Skills.Split(',').Select(s => s.Trim()))
			{
				col.Item().PaddingBottom(4)
				   .Text($"• {skill}")
				   .FontSize(9).FontColor("#D0DCF0");
			}
			col.Item().Height(8);
		}

		if (_cv.Languages is not null)
		{
			SidebarSection(col, "LANGUAGES");
			foreach (var lang in _cv.Languages.Split(',').Select(l => l.Trim()))
			{
				col.Item().PaddingBottom(4)
				   .Text(lang)
				   .FontSize(9).FontColor("#D0DCF0");
			}
			col.Item().Height(8);
		}

		if (_cv.YearsOfExperience.HasValue)
		{
			SidebarSection(col, "EXPERIENCE");
			col.Item()
			   .Text($"{_cv.YearsOfExperience} year(s)")
			   .FontSize(9).FontColor("#D0DCF0");
		}
	}

	private void ComposeMain(ColumnDescriptor col)
	{
		col.Item()
		   .Text(_fullName)
		   .FontSize(20).Bold().FontColor(PrimaryColor);

		col.Item().Height(2);

		col.Item().PaddingBottom(10).BorderBottom(2).BorderColor(PrimaryColor)
		   .Text(_cv.Title)
		   .FontSize(11).Bold().FontColor(AccentColor);

		col.Item().Height(14);

		if (_cv.Summary is not null)
		{
			MainSection(col, "PROFILE");
			col.Item()
			   .Background(LightBg)
			   .BorderLeft(3).BorderColor(PrimaryColor)
			   .Padding(8)
			   .Text(_cv.Summary)
			   .FontSize(10).FontColor("#444444").LineHeight(1.6f);
			col.Item().Height(14);
		}

		if (_cv.Experience is not null)
		{
			MainSection(col, "EXPERIENCE");
			col.Item()
			   .Text(_cv.Experience)
			   .FontSize(10).FontColor("#444444").LineHeight(1.6f);
			col.Item().Height(14);
		}

		if (_cv.Education is not null)
		{
			MainSection(col, "EDUCATION");
			col.Item()
			   .Text(_cv.Education)
			   .FontSize(10).FontColor("#444444").LineHeight(1.6f);
			col.Item().Height(14);
		}

		if (_cv.Projects is not null)
		{
			MainSection(col, "PROJECTS");
			col.Item()
			   .Text(_cv.Projects)
			   .FontSize(10).FontColor("#444444").LineHeight(1.6f);
			col.Item().Height(14);
		}

		if (_cv.Skills is not null)
		{
			MainSection(col, "TECHNOLOGIES");

			var skills = _cv.Skills.Split(',')
								   .Select(s => s.Trim())
								   .ToList();

			col.Item().Column(tagsCol =>
			{
				for (int i = 0; i < skills.Count; i += 4)
				{
					var chunk = skills.Skip(i).Take(4).ToList();

					tagsCol.Item().PaddingBottom(4).Row(r =>
					{
						foreach (var skill in chunk)
						{
							r.AutoItem()
							 .Padding(2)
							 .Element(tag =>
							 {
								 tag.Border(1).BorderColor("#D0DCF0")
									.Background("#EEF2FA")
									.PaddingHorizontal(8).PaddingVertical(3)
									.Text(skill)
									.FontSize(9)
									.FontColor(PrimaryColor);
							 });
						}
					});
				}
			});
		}
	}

	private static void SidebarSection(ColumnDescriptor col, string title)
	{
		col.Item().PaddingBottom(5)
		   .BorderBottom(1).BorderColor("#4A6FA5")
		   .Text(title)
		   .FontSize(8).Bold().FontColor("#A8BEDD");

		col.Item().Height(8);
	}

	private static void SidebarItem(ColumnDescriptor col, string text)
	{
		col.Item().PaddingBottom(6)
		   .Text(text)
		   .FontSize(9).FontColor("#D0DCF0").LineHeight(1.4f);
	}

	private static void MainSection(ColumnDescriptor col, string title)
	{
		col.Item().PaddingBottom(8).Row(r =>
		{
			r.AutoItem()
			 .Text(title)
			 .FontSize(10).Bold().FontColor(PrimaryColor);

			r.RelativeItem()
			 .PaddingLeft(8).PaddingTop(5)
			 .BorderTop(1).BorderColor("#E0E7F0");
		});
	}

	private static string GetInitials(string name)
	{
		var parts = name.Trim().Split(' ');
		return parts.Length >= 2
			? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
			: name.Length >= 2 ? name[..2].ToUpper() : name.ToUpper();
	}
}