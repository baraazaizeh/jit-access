namespace jit_access
{
    public class AccessRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Reason { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime? ApprovalTime { get; set; }
        public DateTime? ExpirationTime { get; set; }

        public AccessRequestStatus Status { get; set; }

        public enum AccessRequestStatus
        {
            Pending,
            Approved,
            Denied,
            Expired
        }

    }
}
