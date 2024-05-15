namespace MagicCollectors.Core.Model
{
    public class Finish
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<Card> Cards { get; set; }
    }
}