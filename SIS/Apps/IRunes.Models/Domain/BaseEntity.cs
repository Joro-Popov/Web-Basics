namespace IRunes.Models.Domain
{
    public abstract class BaseEntity<TKeyidentifier>
    {
        public TKeyidentifier Id { get; set; }
    }
}
