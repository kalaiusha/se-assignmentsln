using System.ComponentModel.DataAnnotations;
using RL.Data.DataModels.Common;

namespace RL.Data.DataModels;

public class Procedure : IChangeTrackable
{
    public Procedure() => ProcedureUsers = new List<ProcedureUser>();
    [Key]
    public int ProcedureId { get; set; }
    public string ProcedureTitle { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public virtual ICollection<ProcedureUser> ProcedureUsers { get; set; }
}
