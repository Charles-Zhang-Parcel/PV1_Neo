namespace Parcel.Neo.Shared
{
    public class ApplicationConfiguration
    {
        public bool AlwaysLaunchInCLI { get; set; } = true;

        public bool ServerDebugPrint { get; set; } = true;
        public int? ServerPort { get; set; } = 5001;
        public string ServerAddress { get; set; } = "localhost";
        
        public void InitializeDefault()
        {
        }
    }
}