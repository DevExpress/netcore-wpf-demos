using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DevExpress.Internal {
    public class ExceptionHelper {
        static ExceptionHelper() {
            IsEnabled = true;
        }
        public static bool IsEnabled { get; set; }
        public static void Initialize() {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        static UnhandledExceptionEventArgs arguments;
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            try {
                if(Debugger.IsAttached || !IsEnabled)
                    return;
                arguments = e;
                ShowWindow();
            } catch(Exception) { }
        }
        static void ShowWindow() {
            string message = GetMessage();
            var window = new Window() { Width = 600, Height = 400, WindowStyle = WindowStyle.ToolWindow, ShowActivated = true, Title = "Unhandled exception" };
            var grid = new Grid() { Margin = new Thickness(5) };
            var closeButton = new Button() { Content = "Close", Margin = new Thickness(3) };
            closeButton.Click += button_Click;
            var copyButton = new Button() { Content = "Copy error", Margin = new Thickness(3) };
            copyButton.Click += copyButton_Click;
            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            Grid.SetRow(stackPanel, 1);
            stackPanel.Children.Add(closeButton);
            stackPanel.Children.Add(copyButton);
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.Children.Add(stackPanel);
            grid.Children.Add(new TextBox() { Text = message, Margin = new Thickness(5) });
            window.Content = grid;
            window.ShowDialog();
            Environment.Exit(1);
        }
        static string GetMessage() {
            Exception ex = (Exception)arguments.ExceptionObject;
            var result = new StringBuilder();
            if(Assembly.GetEntryAssembly() != null) {
                result.Append("EntryAssembly: ");
                result.Append(Assembly.GetEntryAssembly().Location);
                result.AppendLine();
            }
            result.AppendLine("UnhandledException:");
            PackException(ex, result);
            return result.ToString();
        }
        static void PackException(Exception ex, StringBuilder stringBuilder, int index = 0) {
            AppendLine(stringBuilder, ex.Message, index);
            if(!string.IsNullOrWhiteSpace(ex.StackTrace)) {
                AppendLine(stringBuilder, "StackTrace:", index);
                AppendLine(stringBuilder, ex.StackTrace, index);
            }
            if(ex.InnerException != null) {
                AppendLine(stringBuilder, "InnerException:", index);
                PackException(ex.InnerException, stringBuilder, ++index);
            }
        }
        static void AppendLine(StringBuilder stringBuilder, string text, int index) {
            string tabOffset = new string('\t', index);
            stringBuilder.Append(tabOffset);
            var regex = new Regex("\r\n*\\s");
            stringBuilder.AppendLine(regex.Replace(text, "\r\n" + tabOffset));
        }
        static void copyButton_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(GetMessage());
        }
        static void button_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(1);
        }
    }
}