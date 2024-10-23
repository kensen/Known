namespace Sample.WinForm;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception);
    }

    static void HandleException(Exception ex)
    {
        string errorMsg = "������һ��δ������쳣��";
        if (ex != null)
        {
            errorMsg += Environment.NewLine + ex.Message;
            // ������Ӹ��������Ϣ
        }

        Dialog.Error(errorMsg);
    }
}