namespace TrackerApi.Common.Model.Dtos
{
    public class ServiceResultDto<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
