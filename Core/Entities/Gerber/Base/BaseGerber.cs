namespace GerberBackend.Core.Entities.Gerber.Base;

public class BaseGerber
{    public int Id { get; init; }
    public string Value { get; set; }
    public bool IsActive { get; set; }

    public int Identity {  get; init; }

    public ICollection<OrderGerber> OrderGerbers { get; init; }
}
