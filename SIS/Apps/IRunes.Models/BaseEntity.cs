namespace IRunes.Models
{
    public abstract class BaseEntity<TKeyidentifier>
    {
        public TKeyidentifier Id { get; set; }
    }
}
