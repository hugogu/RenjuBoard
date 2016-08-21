namespace Renju.Infrastructure.Protocols
{
    public class AIInfo
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Author { get; set; }

        public string Country { get; set; }

        public override string ToString()
        {
            return string.Format("name=\"{0}\",version=\"{1}\",author=\"{2}\",country=\"{3}\"", Name, Version, Author, Country);
        }
    }
}
