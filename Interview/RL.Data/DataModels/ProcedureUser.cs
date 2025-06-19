using RL.Data.DataModels.Common;

namespace RL.Data.DataModels
{
    public class ProcedureUser : IChangeTrackable
    {
        public int ProcedureId { get; set; }
        public int UserId { get; set; }
        public virtual Procedure Procedure { get; set; }
        public virtual User User { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
