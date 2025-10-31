using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChitChatDesktop.Pages;

public class LoginPageData : INotifyPropertyChanged
{
    private string _username = "";
    private string _password = "";
    private bool _remember;
    private string _usernameError = "";
    private string _passwordError = "";

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

        if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9_]+$"))
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
    private readonly LoginPageData _loginData = new LoginPageData { Username = "", Password = "", Remember = false };

    public LoginPage()
    {
        InitializeComponent();
        DataContext = _loginData;
    }

    private void BOk_OnClick(object? sender, RoutedEventArgs e)
    {
        _loginData.ValidateUsername();
        _loginData.ValidatePassword();

        if (_loginData.HasErrors)
        {
            return;
        }

        _loginData.UsernameError = "Invalid Username";
        Console.WriteLine(new { _loginData.Username, _loginData.Password, _loginData.Remember });
    }

    private void BCancel_OnClick(object? sender, RoutedEventArgs e)
    {
        App.loginWindow?.Close();
    }
}