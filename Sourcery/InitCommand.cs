using Newtonsoft.Json.Linq;

namespace Sourcery
{
    public class InitCommand
    {
        public InitCommand()
        {
            Gateway = new Gateway();
        }


        public string Type { get; set; }
        public Gateway Gateway { get; set; }

        public JArray Arguments { get; set; }
    }
}