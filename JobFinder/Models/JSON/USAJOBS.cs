namespace JobFinder.Models.JSON
{
	public record class USAJOBSResult(USAJOBSSearchResult SearchResult);

	public record class USAJOBSSearchResult(List<USAJOBSSearchResultItem> SearchResultItems);

	public record class USAJOBSSearchResultItem(
		USAJOBSMatchedObjectDescriptor MatchedObjectDescriptor);

	public record class USAJOBSMatchedObjectDescriptor(
		string PositionTitle,
		string PositionURI,
		string PositionLocationDisplay,
		string OrganizationName,
		List<USAJOBSPositionRemuneration> PositionRemuneration,
		DateTime PublicationStartDate,
		string QualificationSummary
		);

	public record class USAJOBSPositionRemuneration(
		string MinimumRange,
		string MaximumRange);
}
