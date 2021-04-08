namespace Ahk.TaskRunner
{
    public struct ImageName
    {
        public ImageName(string name)
            => this.Name = GetCanonicalImageName(name);

        public string Name { get; }


        public static string GetCanonicalImageName(string imageName) => imageName.Contains(':') ? imageName : $"{imageName}:latest";
        public static implicit operator string(ImageName imageName) => imageName.Name;
        public static implicit operator ImageName(string imageName) => new ImageName(imageName);

        public override string ToString() => Name;
    }
}
