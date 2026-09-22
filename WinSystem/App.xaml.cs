using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WinSystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // 沙箱/虚拟化环境下强制软件渲染，避免 GPU 着色器缓存写入被拦截导致崩溃
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

        DispatcherUnhandledException += (_, args) =>
        {
            Log("DispatcherUnhandledException", args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Log("AppDomainUnhandledException", args.ExceptionObject as Exception);

        base.OnStartup(e);
    }

    private static void Log(string kind, Exception? ex)
    {
        try
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinSystem");
            Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir, "error.log"),
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {kind}\n{ex}\n\n");
        }
        catch { /* 忽略日志错误 */ }
    }
}