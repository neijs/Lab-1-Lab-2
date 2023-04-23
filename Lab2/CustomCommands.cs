using System.Windows.Input;

namespace SpringLab2
{
    public static class CustomCommands
    {
        public static RoutedCommand FromControlsCommand = new("FromControlsCommand", typeof(SpringLab2.CustomCommands));
        public static RoutedCommand FromFileCommand = new("FromFileCommand", typeof(SpringLab2.CustomCommands));
        public static RoutedCommand ChartsCommand = new("ChartsCommand", typeof(SpringLab2.CustomCommands));
    }
}
