

namespace eduChain.Models{
    public class FileGroup : List<FileModel>
    {
        public string Name { get; private set; }

        public FileGroup(string name, List<FileModel> filemodels) : base(filemodels)
        {
            Name = name;
        }
    }
}
