using System.Reflection;

namespace SIS.Framework
{
    public class MvcContext
    {
        private static MvcContext Instance;

        public MvcContext()
        {
            
        }

        public static MvcContext Get => Instance ?? (Instance = new MvcContext());

        public string AssemblyName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;

        public string ControllersFolder { get; set; } = "Controllers";

        public string ControllersSuffix { get; set; } = "Controller";

        public string ViewsFolder { get; set; } = "Views";

        public string ModelsFolder { get; set; } = "Models";
    }
}
