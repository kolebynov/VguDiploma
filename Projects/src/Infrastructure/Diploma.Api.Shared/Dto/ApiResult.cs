using System.Runtime.Serialization;

namespace Diploma.Api.Shared.Dto
{
	[DataContract]
	public class ApiResult
	{
		[DataMember]
		public bool Success { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ApiError Error { get; set; }

		public static ApiResult SuccessResult { get; } = new ApiResult
		{
			Success = true
		};

		public static ApiResult ErrorResult(string errorMessage) => new ApiResult
		{
			Success = false,
			Error = new ApiError { Message = errorMessage }
		};

		public static ApiResult<TData> SuccessResultWithData<TData>(TData data) =>
			new ApiResult<TData>
			{
				Data = data,
				Success = true
			};
	}

	[DataContract]
	public class ApiResult<TData> : ApiResult
	{
		[DataMember(EmitDefaultValue = false)]
		public TData Data { get; set; }
	}
}
