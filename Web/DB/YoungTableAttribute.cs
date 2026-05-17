namespace Web
{
    public class YoungTableAttribute : Attribute
    {
        public string Name { get; set; }

        public YoungTableAttribute(string name)
        {
            Name = name;
        }
    }
}
