namespace idp_api.Api.Configuration
{
    public class UserAccessDetail
    {
        public string User { get; set; }

        public bool IsAncillary { get; set; }

        public bool IsMedical { get; set; }

        public bool IsHospital { get; set; }
    }
}