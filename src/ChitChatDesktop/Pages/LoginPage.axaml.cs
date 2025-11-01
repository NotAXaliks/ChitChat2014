using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using ChitChatDesktop.Services;
using MsBox.Avalonia;

namespace ChitChatDesktop.Pages;

public class LoginPageData : INotifyPropertyChanged
{
    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    private string _username = "";
    private string _password = "";
    private bool _remember;
    private string _usernameError = "";
    private string _passwordError = "";
    private string _loginError = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
            if (_username.Length > 0) ValidateUsername();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
            if (_password.Length > 0) ValidatePassword();
        }
    }

    public bool Remember
    {
        get => _remember;
        set
        {
            _remember = value;
            OnPropertyChanged();
        }
    }

    public string UsernameError
    {
        get => _usernameError;
        set
        {
            _usernameError = value;
            OnPropertyChanged();
        }
    }

    public string PasswordError
    {
        get => _passwordError;
        set
        {
            _passwordError = value;
            OnPropertyChanged();
        }
    }

    public string LoginError
    {
        get => _loginError;
        set
        {
            _loginError = value;
            OnPropertyChanged();
        }
    }

    public bool HasErrors => !string.IsNullOrEmpty(UsernameError) || !string.IsNullOrEmpty(PasswordError);

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ValidateUsername()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            UsernameError = "Username is required";
            return;
        }

        switch (Username.Length)
        {
            case < 3:
                UsernameError = "Must be at least 3 characters long";
                return;
            case > 20:
                UsernameError = "Must be less than 20 characters";
                return;
        }

        if (!UsernameRegex.IsMatch(Username))
        {
            UsernameError = "Can only contain a-Z A-Z 0-9 _";
            return;
        }

        UsernameError = "";
    }

    public void ValidatePassword()
    {
        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "Password is required";
            return;
        }

        switch (Password.Length)
        {
            case < 6:
                PasswordError = "Must be at least 6 characters long";
                return;
            case > 50:
                PasswordError = "Must be less than 50 characters";
                return;
            default:
                PasswordError = "";
                break;
        }
    }
}

public partial class LoginPage : UserControl
{
    private readonly Window _loginWindow;
    private readonly LoginPageData _loginData = new LoginPageData { Username = "", Password = "", Remember = false };

    public LoginPage()
    {
        InitializeComponent();
        DataContext = _loginData;
    }

    private async void OnOkClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _loginData.ValidateUsername();
            _loginData.ValidatePassword();

            if (_loginData.HasErrors) return;

            var employeeResponse = await EmployeeApi.Login(_loginData.Username, _loginData.Password);

            if (!string.IsNullOrEmpty(employeeResponse.Error))
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", employeeResponse.Error).ShowAsync();
                return;
            }

            if (!employeeResponse.IsSuccess)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", "Something went wrong.").ShowAsync();
                return;
            }

            // TODO Сделать Remember me

            var currentWindow = VisualRoot as Window;
            currentWindow?.Hide();
            
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var chatWindow = new ChatWindow();
                desktop.MainWindow = chatWindow;
                chatWindow.Show();
                
                currentWindow?.Close();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            await MessageBoxManager.GetMessageBoxStandard("Error", $"Something went wrong. {exception.Message ?? ""}")
                .ShowAsync();
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        (VisualRoot as Window)?.Close();
    }
}