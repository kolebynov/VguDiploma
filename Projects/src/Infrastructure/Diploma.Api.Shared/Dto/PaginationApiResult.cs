namespace Diploma.Api.Shared.Dto
{
	public class PaginationApiResult<T> : ApiResult<T>
	{
		public PaginationData Pagination { get; set; }
	}
}
