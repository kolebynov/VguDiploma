using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Diploma.Api.Shared.Dto
{
	[DataContract]
	public class ApiResult
	{
		[DataMember]
		public bool Success { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ApiError[] Errors { get; set; }

		public static ApiResult SuccessResult { get; } = new ApiResult
		{
			Success = true
		};

		public static ApiResult ErrorResult(params string[] errorMessages) => new ApiResult
		{
			Success = false,
			Errors = errorMessages.Select(x => new ApiError { Message = x }).ToArray()
		};

		public static ApiResult<TData> SuccessResultWithData<TData>(TData data) =>
			new ApiResult<TData>
			{
				Data = data,
				Success = true
			};

		public static PaginationApiResult<TData> SuccessPaginationResult<TData>(TData data, PaginationData pagination)
		{
			if (pagination == null)
			{
				throw new ArgumentNullException(nameof(pagination));
			}

			return new PaginationApiResult<TData>
			{
				Success = true,
				Pagination = pagination,
				Data = data
			};
		}
	}

	[DataContract]
	public class ApiResult<TData> : ApiResult
	{
		[DataMember(EmitDefaultValue = false)]
		public TData Data { get; set; }
	}
}
